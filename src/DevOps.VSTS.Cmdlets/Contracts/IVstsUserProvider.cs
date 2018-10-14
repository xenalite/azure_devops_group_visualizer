using System.Collections.Generic;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface IVstsUserProvider
    {
        IEnumerable<VstsUser> GetUserIdentities();
    }
}