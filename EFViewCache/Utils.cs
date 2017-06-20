namespace EFViewCache
{
    using System.Data.Entity.Core.Metadata.Edm;

    public class Utils
    {
        public static string GetExtentFullName(EntitySetBase entitySet)
        {
            return string.Format("{0}.{1}", entitySet.EntityContainer.Name, entitySet.Name);
        }
    }
}
