using System;
using System.Management.Automation;
using DevOps.VSTS.Cmdlets.Contracts;
using DevOps.VSTS.Cmdlets.Dtos;
using DevOps.VSTS.Cmdlets.Services;

namespace DevOps.VSTS.Cmdlets.Implementation
{
    [Cmdlet(VerbsCommon.Get, "IxsVstsUserPermissionsReport")]
    public class GetVstsUserPermissionsReport : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string VstsAccountUrl { get; set; }

        [Parameter(Mandatory = true)]
        public string AccessToken { get; set; }

        [Parameter(Mandatory = false)]
        public string TenantId { get; set; }

        [Parameter(Mandatory = false)]
        public string UserPrincipalNameFilter { get; set; } = "*";

        protected override void ProcessRecord()
        {
            try
            {
                using (var facade = CreateFacade())
                {
                    var usersProvider = new VstsUserProvider(facade);
                    var resourceProvider = new VstsResourceProvider(facade);
                    var permissionsProvider = new VstsPermissionsProvider(resourceProvider, facade);
                    var reportProducer = new ReportProducer(permissionsProvider, usersProvider);

                    var report = reportProducer.GetUserPermissionReport(UserPrincipalNameFilter);
                    WriteObject(report, true);
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, e.Message, ErrorCategory.NotSpecified, null));
            }
        }

        private IVstsConnectionFacade CreateFacade()
        {
            var context = new VstsContext
            {
                AccessToken = AccessToken,
                CollectionUrl = VstsAccountUrl,
                TenantId = Guid.Parse(TenantId)
            };

            return new VstsConnectionFacade(context);
        }
    }
}