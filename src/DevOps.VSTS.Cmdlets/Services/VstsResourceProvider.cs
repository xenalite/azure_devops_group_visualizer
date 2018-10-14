using System.Collections.Generic;
using System.Linq;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using static DevOps.Cmdlets.Common.Utilities.Helpers;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class VstsResourceProvider : IVstsResourceProvider
    {
        private const string ProjectCollection = "Project Collection";

        private readonly IVstsConnectionFacade _facade;

        public VstsResourceProvider(IVstsConnectionFacade facade)
        {
            _facade = facade;
        }

        public IDictionary<string, Resource[]> GetResourcesPerNamespace()
        {
            return new Dictionary<string, Resource[]>
            {
                {
                    "Analytics", GetAnalyticsResources().ToArray()//TODO
                },
                {
                    "BlobStoreBlobPrivileges", AsArray(
                        Resource("BlobStoreBlobPrivileges (blobs)", "$/blobs/"),
                        Resource("BlobStoreBlobPrivileges (ivy)", "$/references/ivy"),
                        Resource("BlobStoreBlobPrivileges (maven)", "$/references/maven"),
                        Resource("BlobStoreBlobPrivileges (npm)", "$/references/npm"),
                        Resource("BlobStoreBlobPrivileges (nuget)", "$/references/nuget"),
                        Resource("BlobStoreBlobPrivileges (zip)", "$/references/zip"))//TODO
                },
                {
                    "BlobStoreKeepUntilPrivileges", AsArray(
                        Resource("BlobStoreKeepUntilPrivileges", "MaxKeepUntilSpan"))//TODO
                },
                {
                    "Build", GetBuildResources().ToArray()
                },
                {
                    "BuildAdministration", AsArray(Resource(ProjectCollection, "BuildPrivileges"))
                },
                {
                    "Chat", AsArray(Resource("chatrooms/", "chatrooms/"))
                },
                {
                    "Collection", AsArray(Resource(ProjectCollection, "NAMESPACE:"))
                },
                {
                    "CSS", GetStructureResources(TreeStructureGroup.Areas).ToArray()
                },
                {
                    "DashboardsPrivileges", AsArray(Resource("DashboardPrivileges", ""))
                },
                {
                    "DashboardGroupPrivileges", AsArray(Resource("DashboardGroupPrivileges", "$/DashboardGroup/"))
                },
                {
                    "Discussion Threads", AsArray(Resource("DiscussionThreads", "/"))
                },
                {
                    "DistributedTask", GetDistributedTaskResources().ToArray()
                },
                {
                    "EventSubscriber", AsArray(Resource("EventSubscriber", "$SUBSCRIBER:"))
                },
                {
                    "EventSubscription", AsArray(Resource("EventSubscription", "$SUBSCRIPTION:"))
                },
                {
                    "ExtensionManagement", AsArray(Resource("ExtensionManagement", "/"))//TODO
                },
                {
                    "Feed", AsArray(Resource("Feed", "$/"))//TODO
                },
                {
                    "FeedIndex", AsArray(Resource("FeedIndex", "$/"))//TODO
                },
                {
                    "Git Repositories", GetGitBranchResources().ToArray()
                },
                {
                    "Identity", GetIdentityResources().ToArray()
                },
                {
                    "Iteration", GetStructureResources(TreeStructureGroup.Iterations).ToArray()
                },
                {
                    "Library", GetLibraryResources().ToArray()
                },
                {
                    "Licensing", AsArray(
                        Resource("Entitlements", "/Entitlements/AccountEntitlements/"),
                        Resource("Memberships", "/Memberships/"))//TODO
                },
                {
                    "MetaTask", GetMetaTaskResources().ToArray()
                },
                {
                    "Plan", AsArray(Resource("Plan", "Plan/"))
                },
                {
                    "Process", AsArray(Resource("Process", "$PROCESS:"))
                },
                {
                    "Project", GetProjectResources().ToArray()
                },
                {
                    "ReleaseManagement", GetReleaseManagementResources().ToArray()//TODO
                },
                {
                    "Server", AsArray(Resource(ProjectCollection, "FrameworkGlobalSecurity"))
                },
                {
                    "ServiceEndpoints", GetServiceEndpointResources().ToArray()
                },
                {
                    "ServiceHooks", GetServiceHooksResources().ToArray()
                },
                {
                    "StrongBox", AsArray(Resource("StrongBox", "StrongBox/"))
                },
                {
                    "Tagging", GetTaggingResources().ToArray()
                },
                {
                    "TeamLabSecurity", GetTeamLabSecurityResources().ToArray()
                },
                {
                    "VersionControlItems2", GetVersionControlItemResources().ToArray()
                },
                {
                    "VersionControlPrivileges", AsArray(Resource("VersionControlIPrivileges", "Global"))
                },
                {
                    "WorkItemQueryFolders", AsArray(Resource("WorkItemQueryFolders", "$/"))
                },
                {
                    "WorkItemTrackingAdministration", AsArray(
                        Resource("WorkItemTrackingAdministration", "WorkItemTrackingPrivileges"))
                },
                {
                    "WorkItemTrackingProvision", GetWorkItemTrackingProvisionResources().ToArray()
                },
                {
                    "Workspaces", AsArray(Resource("Workspaces", "/"))
                }
            };
        }

        private IEnumerable<Resource> GetTeamLabSecurityResources()
        {
            yield return Resource("TeamLabSecurity", "$/");
            foreach (var project in GetProjects())
            {
                yield return Resource($"TeamProjectHostGroup: {project.Name}", $"$/{project.Id}_TeamProjectHostGroup/");
                yield return Resource($"TeamProjectLibraryShare: {project.Name}", $"$/{project.Id}_TeamProjectLibraryShare/");
            }
        }

        private IEnumerable<Resource> GetTaggingResources()
        {
            yield return Resource("Tagging", "/");
            foreach (var project in GetProjects())
            {
                yield return Resource($"Tagging: {project.Name}", $"/{project.Id}/");
            }
        }

        private IEnumerable<Resource> GetServiceHooksResources()
        {
            yield return Resource("PublisherSecurity", "PublisherSecurity/");
            foreach (var project in GetProjects())
            {
                yield return Resource($"PublisherSecurity: {project.Name}", $"PublisherSecurity/{project.Id}");
            }
        }

        private IEnumerable<Resource> GetServiceEndpointResources()
        {
            return GetProjects()
                .Select(project => Resource(
                    $"ServiceEndpoint: {project.Name}",
                    $"endpoints/{project.Id}/"));
        }

        private IEnumerable<Resource> GetReleaseManagementResources()
        {
            return GetProjects().Select(project => Resource($"ReleaseManagement: {project.Name}", $"{project.Id}/"));
        }

        private IEnumerable<Resource> GetMetaTaskResources()
        {
            return GetProjects().Select(project => Resource($"MetaTask: {project.Name}", $"{project.Id}/"));
        }

        private IEnumerable<Resource> GetLibraryResources()
        {
            return GetProjects().Select(project => Resource($"Library: {project.Name}", $"Library/{project.Id}/"));
        }

        private IEnumerable<Resource> GetIdentityResources()
        {
            return GetProjects().Select(project => Resource($"Identity: {project.Name}", $"{project.Id}\\"));
        }

        private IEnumerable<Resource> GetDistributedTaskResources()
        {
            foreach (var project in GetProjects())
            {
                yield return Resource($"AgentQueues: {project.Name}", $"AgentQueues/{project.Id}/");
                yield return Resource($"MachineGroups: {project.Name}", $"MachineGroups/{project.Id}/");
            }
        }

        private IEnumerable<Resource> GetAnalyticsResources()
        {
            return GetProjects()
                .Select(project => Resource(
                    $"Analytics: {project.Name}",
                    $"$/{project.Id}"));
        }

        private static Resource Resource(string name, string token)
        {
            return new Resource {Name = name, Token = token};
        }

        private IEnumerable<TeamProjectReference> GetProjects()
        {
            return _facade.GetProjects();
        }

        private IEnumerable<Resource> GetStructureResources(TreeStructureGroup treeStructureGroup)
        {
            return GetProjects().SelectMany(teamProject => ToProjectNodes(treeStructureGroup, teamProject));
        }

        private IEnumerable<Resource> ToProjectNodes(
            TreeStructureGroup treeStructureGroup, TeamProjectReference teamProject)
        {
            var rootAreaNode = GetNode(treeStructureGroup, teamProject);
            return GetStructureResources("", rootAreaNode);
        }

        private WorkItemClassificationNode GetNode(
            TreeStructureGroup treeStructureGroup, TeamProjectReference teamProject)
        {
            return _facade.GetClassificationNode(teamProject.Name, treeStructureGroup);
        }

        private static IEnumerable<Resource> GetStructureResources(
            string parentName, WorkItemClassificationNode node)
        {
            var resourceName = $"{parentName}\\{node.Name}";
            yield return Resource(resourceName, $"vstfs:///Classification/Node/{node.Identifier}");

            var children = node.Children ?? Enumerable.Empty<WorkItemClassificationNode>();
            foreach (var child in children.SelectMany(childNode => GetStructureResources(resourceName, childNode)))
                yield return child;
        }

        private IEnumerable<Resource> GetProjectResources()
        {
            return GetProjects().Select(teamProject => Resource(teamProject.Name, ToProjectToken(teamProject)));
        }

        private static string ToProjectToken(TeamProjectReference teamProject)
        {
            return $"$PROJECT:vstfs:///Classification/TeamProject/{teamProject.Id}";
        }

        private IEnumerable<Resource> GetGitBranchResources()
        {
            yield return Resource("repoV2/", "repoV2/");
            foreach (var project in GetProjects())
            {
                yield return Resource(
                    $"repoV2/{project.Name}",
                    $"repoV2/{project.Id}/");

                foreach (var gitRepository in _facade.GetGitRepositories(project.Id))
                {
                    yield return Resource(
                        $"repoV2/{project.Name}/{gitRepository.Name}",
                        $"repoV2/{project.Id}/{gitRepository.Id}/");

                    foreach (var gitRef in _facade.GetGitBranches(gitRepository.Id))
                        yield return Resource(
                            $"{project.Name}/{gitRepository.Name}/{gitRef.Name}",
                            $"repoV2/{project.Id}/{gitRepository.Id}/refs^heads^{gitRef.Name}/");
                }
            }
        }

        private IEnumerable<Resource> GetWorkItemTrackingProvisionResources()
        {
            yield return Resource("WorkItemTrackingProvision", "$/");
            foreach (var teamProject in GetProjects())
                yield return Resource($"WorkItemTrackingProvision: {teamProject.Name}", $"$/{teamProject.Id}/");
        }

        private IEnumerable<Resource> GetVersionControlItemResources()
        {
            yield return Resource("VersionControlItems2", "$/");
            foreach (var teamProject in GetProjects())
            {
                yield return Resource($"$/{teamProject.Name}", $"$/{teamProject.Name}");
            }
        }

        private IEnumerable<Resource> GetBuildResources()
        {
            foreach (var project in GetProjects())
            {
                yield return Resource($"{project.Name}/", $"{project.Id}/");

                foreach (var buildDefinition in _facade.GetBuildDefinitions(project.Id))
                    yield return Resource(
                        $"{project.Name}/{buildDefinition.Name}",
                        $"{project.Id}/{buildDefinition.Id}");
            }
        }
    }
}