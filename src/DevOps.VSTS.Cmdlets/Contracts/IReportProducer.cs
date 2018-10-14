using System.Collections.Generic;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface IReportProducer
    {
        IEnumerable<PermissionEntry> GetUserPermissionReport(string userPrincipalNameFilter);
    }
}