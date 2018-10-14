namespace DevOps.VSTS.Cmdlets.Dtos
{
    public class PermissionEntry
    {
        public string Username { get; set; }
        public string SecurityCategory { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
    }
}