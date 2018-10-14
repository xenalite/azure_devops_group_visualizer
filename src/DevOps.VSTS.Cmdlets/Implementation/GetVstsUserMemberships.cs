using System;
using System.Management.Automation;
using DevOps.Cmdlets.Common.Implementation;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using DevOps.VSTS.Cmdlets.Services;

namespace DevOps.VSTS.Cmdlets.Implementation
{
    [Cmdlet(VerbsCommon.Get, "IxsVstsUserMemberships")]
    public class GetVstsUserMemberships : CmdletBase
    {
        [Parameter(Mandatory = true)]
        public string CollectionUrl { get; set; }

        [Parameter(Mandatory = false)]
        public string AccessToken { get; set; }

        [Parameter(Mandatory = false)]
        public string TenantId { get; set; } = "76e3921f-489b-4b7e-9547-9ea297add9b5";

        protected override void Execute()
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