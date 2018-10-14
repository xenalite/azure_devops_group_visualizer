namespace DevOps.VSTS.Cmdlets.Utilities
{
    public static class Helpers
    {
        public static T[] AsArray<T>(params T[] resources)
        {
            return resources;
        }
    }
}