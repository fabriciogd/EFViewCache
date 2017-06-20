namespace EFViewCache
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure.MappingViews;
    using System.Linq;
    using System.Xml.Linq;

    public class MappingViewCache : DbMappingViewCache
    {
        private readonly string _hash;
        private readonly Dictionary<string, DbMappingView> _views;

        public MappingViewCache(XElement views)
        {
            if (views == null) {
                throw new ArgumentNullException("views");
            }

            _hash = (string)views.Attribute("hash");

            if (string.IsNullOrWhiteSpace(_hash)) {
                throw new InvalidOperationException("The hash of the mapping cannot be null or empty string.");
            }

            _views = views
                .Elements("view")
                .ToDictionary(v => (string)v.Attribute("extent"), v => new DbMappingView((string)v.Value));
        }

        public MappingViewCache(string hash, Dictionary<EntitySetBase, DbMappingView> views)
        {
            if (string.IsNullOrWhiteSpace(hash)) {
                throw new ArgumentNullException("hash");
            }

            if (views == null) {
                throw new ArgumentNullException("views");
            }

            _hash = hash;
            _views = views.ToDictionary(kvp => Utils.GetExtentFullName(kvp.Key), kvp => kvp.Value);
        }

        public override DbMappingView GetView(EntitySetBase entitySet)
        {
            DbMappingView mappingView;

            _views.TryGetValue(Utils.GetExtentFullName(entitySet), out mappingView);

            return mappingView;
        }

        public override string MappingHashValue
        {
            get { return _hash; }
        }
    }
}
