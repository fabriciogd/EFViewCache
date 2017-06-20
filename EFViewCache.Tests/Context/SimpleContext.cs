namespace EFViewCache.Tests.Context
{
    using System.Data.Entity;

    public class Entity
    {
        public int Id { get; set; }
    }

    public class SimpleContext : DbContext
    {
        public SimpleContext()
            : this(System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString)
        {
            Database.SetInitializer<SimpleContext>(null);
        }

        public SimpleContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        public DbSet<Entity> Entities { get; set; }
    }
}
