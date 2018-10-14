using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface IVstsConnectionFacade : IDisposable
    {
        Guid TenantId { get; }
        string GetUsersList();
        TeamFoundationIdentity GetIdentity(IdentityDescriptor identity);
        SecurityNamespace GetNamespace(string name);
        IEnumerable<TeamProjectReference> GetProjects();
        WorkItemClassificationNode GetClassificationNode(string teamProjectName, TreeStructureGroup group);
        IEnumerable<DefinitionReference> GetBuildDefinitions(Guid projectId);
        IEnumerable<GitRepository> GetGitRepositories(Guid projectId);
        IEnumerable<GitRef> GetGitBranches(Guid gitRepositoryId);
        IEnumerable<TeamFoundationIdentity> GetIdentities(IdentityDescriptor[] identities);
        IEnumerable<TeamFoundationIdentity> GetRootGroups();
    }
}