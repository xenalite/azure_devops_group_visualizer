using System;
using System.Text;

namespace DevOps.VSTS.Cmdlets.Dtos
{
    public class VstsContext
    {
        public string CollectionUrl { get; set; }
        public string AccessToken { get; set; }
        public Guid TenantId { get; set; }
        public Uri CollectionUri => new Uri(CollectionUrl);
        public string VstsInstanceName => CollectionUri.Host.Split('.')[0];
        public string AuthorizationToken => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($":{AccessToken}"))}";
    }
}