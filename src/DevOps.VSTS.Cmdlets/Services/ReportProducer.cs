using System.Collections.Generic;
using System.Linq;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class ReportProducer : IReportProducer
    {
        private readonly IVstsPermissionsProvider _permissionsProvider;
        private readonly IVstsUserProvider _userProvider;

        public ReportProducer(IVstsPermissionsProvider permissionsProvider, IVstsUserProvider userProvider)
        {
            _permissionsProvider = permissionsProvider;
            _userProvider = userProvider;
        }

        public IEnumerable<AccessLevelEntry> GetUserAccessLevelReport()
        {
            var users = _userProvider.GetUserIdentities();
            var userEntries = users.Select(user => new AccessLevelEntry
            {
                AccessLevel = user.AccessLevel,
                LastAccessDate = user.LastAccess,
                Username = user.UniqueName
            });
            return userEntries;
        }

        public IEnumerable<PermissionEntry> GetUserPermissionReport(string userPrincipalNameFilter)
        {
            return _userProvider.GetUserIdentities()
                .AsParallel()
                .Where(user => true) // TODO: Implement UPN filtering
                .Select(user =>
                {
                    return _permissionsProvider.GetPermissionAssignments(user)
                        .Select(entry => new PermissionEntry
                        {
                            Action = entry.Action,
                            Resource = entry.Resource,
                            SecurityCategory = entry.Namespace,
                            Username = entry.Identity
                        });
                })
                .SelectMany(e => e);
        }
    }
}