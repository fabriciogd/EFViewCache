namespace EFViewCache.Tests
{
    using Context;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void CreateViews()
        {
            using (var ctx = new SimpleContext())
            {
                ViewCache
                    .SetViewCacheFactory(
                        ctx,
                        new FileViewCacheFactory(@"C:\MyViews.views"));

                ctx.Entities.Count();
            }
        }
    }
}
