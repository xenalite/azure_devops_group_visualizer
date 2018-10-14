using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class VstsConnectionFacade : IVstsConnectionFacade
    {
        private readonly BuildHttpClient _buildHttpClient;
        private readonly TfsTeamProjectCollection _connection;
        private readonly VstsContext _context;
        private readonly GitHttpClient _gitHttpClient;
        private readonly HttpClient _httpClient;
        private readonly ProcessHttpClient _processHttpClient;

        private readonly ProjectHttpClient _projectHttpClient;
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;
        private readonly IIdentityManagementService _identityService;
        private readonly ISecurityService _securityService;

        public VstsConnectionFacade(VstsContext context)
        {
            _context = context;
            var uri = _context.CollectionUri;
            
            _connection = context.AccessToken == null 
                ? new TfsTeamProjectCollection(uri)
                : new TfsTeamProjectCollection(uri, new VssBasicCredential(string.Empty, context.AccessToken));

            _httpClient = GetHttpClient();
            _projectHttpClient = _connection.GetClient<ProjectHttpClient>();
            _workItemTrackingHttpClient = _connection.GetClient<WorkItemTrackingHttpClient>();
            _gitHttpClient = _connection.GetClient<GitHttpClient>();
            _buildHttpClient = _connection.GetClient<BuildHttpClient>();
            _processHttpClient = _connection.GetClient<ProcessHttpClient>();
            _identityService = _connection.GetService<IIdentityManagementService>();
            _securityService = _connection.GetService<ISecurityService>();
        }

        public Guid TenantId => _context.TenantId;

        public string GetUsersList()
        {
            var extensionBaseUrl = $"https://{_context.VstsInstanceName}.vsaex.visualstudio.com";
            var requestUri = $"{extensionBaseUrl}/_apis/MEMInternal/Export?api-version=3.1-preview";
            var content = HttpGet(requestUri);
            return content;
        }

        public TeamFoundationIdentity GetIdentity(IdentityDescriptor identity)
        {
            var tfsIdentity = _identityService.ReadIdentity(identity, MembershipQuery.Direct, ReadIdentityOptions.None);
            if (tfsIdentity != null)
                return tfsIdentity;

            var adIdentity = _identityService.ReadIdentity(identity, MembershipQuery.Direct, ReadIdentityOptions.IncludeReadFromSource);
            if (adIdentity != null)
                return adIdentity;

            throw new Exception($"Identity not found: {identity}");
        }

        public SecurityNamespace GetNamespace(string name)
        {
            var namespaces = _securityService.GetSecurityNamespaces();
            var @namespace = namespaces.Single(e => e.Description.Name == name);
            return @namespace;
        }

        public IEnumerable<TeamProjectReference> GetProjects()
        {
            return _projectHttpClient.GetProjects().Result;
        }

        public WorkItemClassificationNode GetClassificationNode(string teamProjectName, TreeStructureGroup group)
        {
            return _workItemTrackingHttpClient.GetClassificationNodeAsync(teamProjectName, group, depth: 999).Result;
        }

        public IEnumerable<DefinitionReference> GetBuildDefinitions(Guid projectId)
        {
            return _buildHttpClient.GetDefinitionsAsync(projectId).Result;
        }

        public IEnumerable<GitRepository> GetGitRepositories(Guid projectId)
        {
            return _gitHttpClient.GetRepositoriesAsync(projectId).Result;
        }

        public IEnumerable<GitRef> GetGitBranches(Guid gitRepositoryId)
        {
            return _gitHttpClient.GetBranchRefsAsync(gitRepositoryId).Result;
        }

        public IEnumerable<TeamFoundationIdentity> GetIdentities(IdentityDescriptor[] identities)
        {
            return _identityService.ReadIdentities(identities, MembershipQuery.Direct, ReadIdentityOptions.None);
        }

        public IEnumerable<TeamFoundationIdentity> GetRootGroups()
        {
            return _identityService.ListApplicationGroups(null, ReadIdentityOptions.None);
        }

        public void Dispose()
        {
            _connection.Dispose();
            _projectHttpClient.Dispose();
            _workItemTrackingHttpClient.Dispose();
            _gitHttpClient.Dispose();
            _processHttpClient.Dispose();
            _buildHttpClient.Dispose();
            _httpClient.Dispose();
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", _context.AuthorizationToken);
            return httpClient;
        }

        private string HttpGet(string requestUri)
        {
            var response = _httpClient.GetAsync(requestUri).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}