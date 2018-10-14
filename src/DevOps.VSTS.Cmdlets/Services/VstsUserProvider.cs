using System;
using System.Collections.Generic;
using System.Linq;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;

namespace DevOps.VSTS.Cmdlets.Services
{
    public class VstsUserProvider : IVstsUserProvider
    {
        private readonly IVstsConnectionFacade _facade;

        public VstsUserProvider(IVstsConnectionFacade facade)
        {
            _facade = facade;
        }

        public IEnumerable<VstsUser> GetUserIdentities()
        {
            const char entriesSeparator = ',';

            var users = _facade
                .GetUsersList()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(e => e.Split(entriesSeparator))
                .Select(e => new VstsUser
                {
                    DisplayName = string.Join($"{entriesSeparator}", e.Take(e.Length - 4)),
                    UniqueName = e[e.Length - 4],
                    AccessLevel = e[e.Length - 3],
                    LastAccess = e[e.Length - 2],
                    TenantId = _facade.TenantId
                });

            return users;
        }
    }
}