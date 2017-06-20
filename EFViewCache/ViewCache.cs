namespace EFViewCache
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Infrastructure.MappingViews;

    public class ViewCache
    {
        private static ConcurrentDictionary<DbMappingViewCacheFactory, StorageMappingItemCollection> _mappingItemCollection =
            new ConcurrentDictionary<DbMappingViewCacheFactory, StorageMappingItemCollection>();

        public static void SetViewCacheFactory(DbContext context, DbMappingViewCacheFactory viewCacheFactory)
        {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            if (viewCacheFactory == null) {
                throw new ArgumentNullException("viewCacheFactory");
            }

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            SetViewCacheFactory(objectContext, viewCacheFactory);
        }

        public static void SetViewCacheFactory(ObjectContext context, DbMappingViewCacheFactory viewCacheFactory)
        {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            if (viewCacheFactory == null) {
                throw new ArgumentNullException("viewCacheFactory");
            }

            var storageMappingItemCollection = (StorageMappingItemCollection)context.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);

            storageMappingItemCollection.MappingViewCacheFactory = viewCacheFactory;

            _mappingItemCollection.TryAdd(viewCacheFactory, storageMappingItemCollection);
        }

        public static StorageMappingItemCollection GetMappingItemCollection(DbMappingViewCacheFactory viewCacheFactory)
        {
            if (viewCacheFactory == null) {
                throw new ArgumentNullException("viewCacheFactory");
            }

            StorageMappingItemCollection mappingItemCollection;

            if (!_mappingItemCollection.TryGetValue(viewCacheFactory, out mappingItemCollection)) {
                throw new InvalidOperationException("No StorageMappingItemCollection instance found for the provided DbMappingViewCacheFactory.");
            }

            return mappingItemCollection;
        }
    }
}
