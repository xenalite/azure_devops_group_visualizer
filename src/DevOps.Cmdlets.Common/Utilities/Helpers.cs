namespace DevOps.Cmdlets.Common.Utilities
{
    public static class Helpers
    {
        public static T[] AsArray<T>(params T[] resources)
        {
            return resources;
        }
    }
}