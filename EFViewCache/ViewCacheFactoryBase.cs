namespace EFViewCache
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure.MappingViews;
    using System.Linq;
    using System.Xml.Linq;

    public abstract class ViewCacheFactoryBase : DbMappingViewCacheFactory
    {
        protected abstract XDocument Load(string conceptualModelContainerName, string storeModelContainerName);

        protected abstract void Save(XDocument views);

        public override DbMappingViewCache Create(string conceptualModelContainerName, string storeModelContainerName)
        {
            var mappingItemCollection = this.GetMappingItemCollection();

            if (mappingItemCollection == null) {
                throw new InvalidOperationException("View cache not set for this mapping item collection");
            }

            var viewsXml = this.Load(conceptualModelContainerName, storeModelContainerName);

            var hash = mappingItemCollection.ComputeMappingHashValue(conceptualModelContainerName, storeModelContainerName);

            if (viewsXml != null)
            {
                var viewsForMapping = this.GetViews(viewsXml, conceptualModelContainerName, storeModelContainerName);

                if (viewsForMapping != null && (string)viewsForMapping.Attribute("hash") == hash)
                {
                    return new MappingViewCache(viewsForMapping);
                }
            }

            var views = this.GenerateViews(mappingItemCollection, conceptualModelContainerName, storeModelContainerName);

            viewsXml = this.CreateViews(hash, conceptualModelContainerName, storeModelContainerName, views);

            this.Save(viewsXml);

            return new MappingViewCache(hash, views);
        }

        internal virtual StorageMappingItemCollection GetMappingItemCollection()
        {
            return ViewCache.GetMappingItemCollection(this);
        }

        internal virtual Dictionary<EntitySetBase, DbMappingView> GenerateViews(StorageMappingItemCollection mappingItemCollection, string conceptualModelContainerName, string storeModelContainerName)
        {
            var errors = new List<EdmSchemaError>();

            var views = mappingItemCollection
                .GenerateViews(conceptualModelContainerName, storeModelContainerName, errors);

            if (errors.Count > 0)
                throw new Exception(string.Join(",", errors.Select(a => a.Message)));

            return views;
        }

        internal virtual XDocument CreateViews(string hash, string conceptualModelContainerName, string storeModelContainerName, Dictionary<EntitySetBase, DbMappingView> views)
        {
            var viewsXml = new XDocument(new XElement("views"));

            viewsXml.Root.Add(
               new XElement("mapping-views",
                   new XAttribute("hash", hash),
                   new XAttribute("conceptual-container", conceptualModelContainerName),
                   new XAttribute("store-container", storeModelContainerName),
                   views.Select(
                       definition =>
                           new XElement(
                               "view",
                               new XAttribute("extent", definition.Key),
                               new XCData(definition.Value.EntitySql)))));

            return viewsXml;
        }

        internal virtual XElement GetViews(XDocument viewsXml, string conceptualModelContainerName, string storeModelContainerName)
        {
            return
                viewsXml.Root
                    .Elements("mapping-views")
                    .SingleOrDefault(e => (string)e.Attribute("conceptual-container") == conceptualModelContainerName
                        && (string)e.Attribute("store-container") == storeModelContainerName);
        }
    }
}
