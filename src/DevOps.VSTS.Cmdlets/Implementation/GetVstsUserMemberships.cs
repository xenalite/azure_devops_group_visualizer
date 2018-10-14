using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Tracing;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using DevOps.VSTS.Cmdlets.Services;

namespace DevOps.VSTS.Cmdlets.Implementation
{
    [OutputType(typeof(IdentityMemberships))]
    [Cmdlet(VerbsCommon.Get, "IxsVstsUserMemberships")]
    public class GetVstsUserMemberships : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string CollectionUrl { get; set; }

        [Parameter(Mandatory = false)]
        public string AccessToken { get; set; }

        [Parameter(Mandatory = false)]
        public string TenantId { get; set; }
        
        protected override void ProcessRecord()
        {
            using (var facade = CreateFacade())
            {
                var resourceProvider = new VstsResourceProvider(facade);
                var permissionsProvider = new VstsPermissionsProvider(resourceProvider, facade);
                var rootIdentityMemberships = permissionsProvider.GetRootIdentityMemberships();

                WriteObject(rootIdentityMemberships);
            }
        }

        private IVstsConnectionFacade CreateFacade()
        {
            var context = new VstsContext
            {
                AccessToken = AccessToken,
                CollectionUrl = CollectionUrl,
                TenantId = Guid.Parse(TenantId)
            };

            return new VstsConnectionFacade(context);
        }
    }
}