namespace DevOps.VSTS.Cmdlets.Dtos
{
    public class PermissionAssignment
    {
        public string Identity { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
        public string Namespace { get; set; }
    }
}