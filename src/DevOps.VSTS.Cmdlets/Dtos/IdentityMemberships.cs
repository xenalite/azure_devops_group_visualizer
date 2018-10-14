namespace DevOps.VSTS.Cmdlets.Dtos
{
    public class IdentityMemberships
    {
        public string DisplayName { get; set; }
        public TFIdentityCategory Category { get; set; }
        public IdentityMemberships[] NestedIdentities { get; set; }
    }
}