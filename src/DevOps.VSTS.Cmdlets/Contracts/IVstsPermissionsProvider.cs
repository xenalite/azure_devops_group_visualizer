using System.Collections.Generic;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface IVstsPermissionsProvider
    {
        IEnumerable<PermissionAssignment> GetPermissionAssignments(VstsUser user);
        IdentityMemberships GetRootIdentityMemberships();
    }
}