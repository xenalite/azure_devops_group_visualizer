using System;
using Microsoft.TeamFoundation.Framework.Client;

namespace DevOps.VSTS.Cmdlets.Dtos
{
    public class VstsUser
    {
        public string DisplayName { get; set; }
        public string UniqueName { get; set; }
        public string AccessLevel { get; set; }
        public string LastAccess { get; set; }
        public Guid TenantId { get; set; }
        public IdentityDescriptor Descriptor
        {
            get
            {
                const string identityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
                return new IdentityDescriptor(identityType, $"{TenantId}\\{UniqueName}");
            }
        }
    }
}