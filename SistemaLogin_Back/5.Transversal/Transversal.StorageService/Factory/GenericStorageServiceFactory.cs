using System;
using System.Collections.Generic;
using System.Text;
using Transversal.StorageService.Configurations;
using Transversal.StorageService.Contracts;

namespace Transversal.StorageService.Factory
{
    public class GenericStorageServiceFactory : IGenericStorageServiceFactory
    {
        private readonly IFileSystemStorageService _fileSystemStorageService;
        private readonly IGoogleCloudStorageService _googlecloudStorageService;
        private readonly GenericStorageConfiguration _storageConfiguration;

        private readonly Dictionary<string, GenericStorageTypeEnum> _types;

        public GenericStorageServiceFactory(
            IFileSystemStorageService fileSystemStorageService,
            IGoogleCloudStorageService cloudStorageService,
            GenericStorageConfiguration storageConfiguration)
        {
            _fileSystemStorageService = fileSystemStorageService;
            _googlecloudStorageService = cloudStorageService;
            _storageConfiguration = storageConfiguration;

            _types = new Dictionary<string, GenericStorageTypeEnum>
            {
                { "FSS", GenericStorageTypeEnum.FSS },
                { "GCS", GenericStorageTypeEnum.GCS }
            };
        }

        public IGenericStorageService GetDefault() => Get(_types[_storageConfiguration.Type]);

        public IGenericStorageService Get(GenericStorageTypeEnum storageType) => storageType switch
        {
            GenericStorageTypeEnum.FSS => _fileSystemStorageService,
            GenericStorageTypeEnum.GCS => _googlecloudStorageService,
            _ => null,
        };

        public IGenericStorageService Get(string storageType) => Get(_types[storageType]);
    }
}
