using System;
using System.Collections.Generic;
using System.Linq;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using DevOps.VSTS.Cmdlets.Utilities;
using Microsoft.TeamFoundation.Framework.Client;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class VstsPermissionsProvider : IVstsPermissionsProvider
    {
        private readonly IVstsConnectionFacade _facade;
        private readonly IVstsResourceProvider _vstsResourceProvider;

        public VstsPermissionsProvider(IVstsResourceProvider vstsResourceProvider, IVstsConnectionFacade facade)
        {
            _vstsResourceProvider = vstsResourceProvider;
            _facade = facade;
        }

        public IEnumerable<PermissionAssignment> GetPermissionAssignments(VstsUser user)
        {
            var resources = _vstsResourceProvider.GetResourcesPerNamespace();
            var permissions = GetPermissionAssignments(resources, user);
            return permissions;
        }

        public IdentityMemberships GetRootIdentityMemberships()
        {
            var rootIdentity = new IdentityMemberships
            {
                DisplayName = $"{TFIdentityCategory.ProjectCollection}",
                Category = TFIdentityCategory.ProjectCollection,
                NestedIdentities = GetRootNestedIdentities()
            };
            return rootIdentity;
        }

        private IdentityMemberships[] GetRootNestedIdentities()
        {
            var collectionGroups = _facade
                .GetRootGroups()
                .Select(identity => _facade.GetIdentity(identity.Descriptor))
                .Select(identity => ToIdentity(identity, TFIdentityCategory.ProjectCollection))
                .ToArray();

            return collectionGroups;
        }

        private static TFIdentityCategory ToCategory(TeamFoundationIdentity current, TFIdentityCategory parentCategory)
        {
            var name = current.DisplayName;
            var domainGroupToken = "[TEAM FOUNDATION]\\";

            var isAccount = !current.IsContainer;
            if (isAccount)
            {
                var isBuiltInAccount = name.Contains("(TEAM FOUNDATION)") || name.Contains("Microsoft");
                if (isBuiltInAccount)
                    return TFIdentityCategory.TFAccount;

                if (parentCategory == TFIdentityCategory.TFGroup)
                    return TFIdentityCategory.DirectUserAccount;

                return TFIdentityCategory.UserAccount;
            }

            var isBuiltInGroup = current.UniqueName.Contains("vstfs") && !name.StartsWith(domainGroupToken);
            if (isBuiltInGroup)
                return TFIdentityCategory.TFGroup;

            var groupName = name.Replace(domainGroupToken, string.Empty);
            var isValidResourceGroup = groupName.StartsWith("R-VSTS");
            if (isValidResourceGroup)
                return TFIdentityCategory.ResourceGroup;

            var isRoleGroup = groupName.StartsWith("RCSS-") || groupName.StartsWith("IXS-");
            if (isRoleGroup)
                return TFIdentityCategory.RoleGroup;

            return TFIdentityCategory.OtherGroup;
        }

        private IdentityMemberships[] GetNestedIdentities(TeamFoundationIdentity parentIdentity, TFIdentityCategory parentCategory)
        {
            if (!parentIdentity.IsContainer)
                return new IdentityMemberships[0];

            var nestedIdentities = _facade
                .GetIdentities(parentIdentity.Members)
                .Select(identity => ToIdentity(identity, parentCategory))
                .ToArray();

            return nestedIdentities;
        }

        private IdentityMemberships ToIdentity(TeamFoundationIdentity identity, TFIdentityCategory parentCategory)
        {
            var category = ToCategory(identity, parentCategory);
            return new IdentityMemberships
            {
                DisplayName = identity.DisplayName,
                Category = category,
                NestedIdentities = GetNestedIdentities(identity, category)
            };
        }

        private IEnumerable<PermissionAssignment> GetPermissionAssignments(
            IDictionary<string, Resource[]> resources, VstsUser userIdentity)
        {
            return resources.SelectMany(kvp => GetResourcePermissions(userIdentity.Descriptor, kvp.Value, kvp.Key));
        }

        private IEnumerable<PermissionAssignment> GetResourcePermissions(
            IdentityDescriptor identity, IReadOnlyList<Resource> resources, string namespaceName)
        {
            try
            {
                var @namespace = _facade.GetNamespace(namespaceName);
                var tfsIdentity = _facade.GetIdentity(identity);
                var permissions = GetResourcePermissions(resources, @namespace, tfsIdentity);
                return permissions;
            }
            catch (Exception e)
            {
                Log.Warning($"{e.Message}");
                return Enumerable.Empty<PermissionAssignment>();
            }
        }

        private static IEnumerable<PermissionAssignment> GetResourcePermissions(
            IReadOnlyList<Resource> resources,
            SecurityNamespace @namespace,
            TeamFoundationIdentity identity)
        {
            foreach (var descriptionAction in @namespace.Description.Actions)
            {
                var tokens = resources.Select(e => e.Token).ToArray();
                if(!tokens.Any())
                    yield break;

                var hasPermissions = @namespace.HasPermission(
                    tokens,
                    identity.Descriptor,
                    descriptionAction.Bit,
                    true);

                for (var i = 0; i < resources.Count; i++)
                {
                    var resource = resources[i];
                    var hasPermission = hasPermissions[i];
                    if (hasPermission)
                    {
                        yield return new PermissionAssignment
                        {
                            Action = descriptionAction.DisplayName,
                            Resource = resource.Name,
                            Namespace = @namespace.Description.DisplayName,
                            Identity = identity.UniqueName
                        };
                    }
                }
            }
        }
    }
}