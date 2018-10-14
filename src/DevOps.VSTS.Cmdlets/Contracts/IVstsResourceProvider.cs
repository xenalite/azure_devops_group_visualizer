using System.Collections.Generic;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Contracts
{
    public interface IVstsResourceProvider
    {
        IDictionary<string, Resource[]> GetResourcesPerNamespace();
    }
}