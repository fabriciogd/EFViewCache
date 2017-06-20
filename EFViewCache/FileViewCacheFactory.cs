namespace EFViewCache
{
    using System;
    using System.IO;
    using System.Xml.Linq;

    public class FileViewCacheFactory: ViewCacheFactoryBase
    {
        private readonly string _filePath;

        private readonly object _lockObject = new object();

        public FileViewCacheFactory(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            this._filePath = filePath;
        }

        protected override XDocument Load(string conceptualModelContainerName, string storeModelContainerName)
        {
            if(string.IsNullOrWhiteSpace(conceptualModelContainerName)) {
                throw new ArgumentNullException("conceptualModelContainerName");
            }

            if (string.IsNullOrWhiteSpace(storeModelContainerName)) {
                throw new ArgumentNullException("storeModelContainerName");
            }

            try
            {
                lock (_lockObject)
                {
                    if (File.Exists(_filePath))
                    {
                        return XDocument.Load(_filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }

            return null;
        }

        protected override void Save(XDocument views)
        {
            lock (_lockObject)
            {
                views.Save(_filePath);
            }
        }
    }
}
