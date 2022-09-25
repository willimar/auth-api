using DataCore.Domain.Interfaces;
using DataCore.Provider.MongoDb;
using MongoDB.Driver;

namespace System
{
    public static class DataProviderExtensions
    {
        public static string MongoWriterSection { get; set; } = "MongoWriterSection";
        public static string MongoReaderSection { get; set; } = "MongoReaderSection";
        public static string MongoWriterDataBase { get; set; } = "MongoWriterDataBase";
        public static string MongoReaderDataBase { get; set; } = "MongoReaderDataBase";
        public static string ReplicationSet { get; set; } = "ReplicationSet";
        public static string UserName { get; set; } = "UserName";
        public static string UserPassword { get; set; } = "UserPassword";
        public static string HostUrl { get; set; } = "HostUrl";

        private class DataProviderWriter : DataProvider, IDataProviderWrite
        {
            public DataProviderWriter(IMongoClient context, string dataBase) : base(context, dataBase)
            {
            }
        }

        private class DataProviderReader : DataProvider, IDataProviderRead
        {
            public DataProviderReader(IMongoClient context, string dataBase) : base(context, dataBase)
            {
            }
        }

        public static IDataProviderWrite GetDataProviderWrite(this IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var dataBaseName = configuration.ReadConfig<string>(MongoWriterSection, MongoWriterDataBase);
            IMongoClient mongoClient = configuration.GetMongoClient(MongoWriterSection);
            IDataProviderWrite response = new DataProviderWriter(mongoClient, dataBaseName) { };

            return response;
        }

        public static IDataProviderRead GetDataProviderReader(this IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var dataBaseName = configuration.ReadConfig<string>(MongoReaderSection, MongoReaderDataBase);
            IMongoClient mongoClient = configuration.GetMongoClient(MongoReaderSection);
            IDataProviderRead response = new DataProviderReader(mongoClient, dataBaseName) { };

            return response;
        }

        private static IMongoClient GetMongoClient(this IConfiguration configuration, string sectionName)
        {
            var connectionString = configuration.GetConnectionString(sectionName);
            var mongoClient = new MongoClient(connectionString);

            return mongoClient;
        }

        private static string GetConnectionString(this IConfiguration configuration, string sectionName)
        {
            string connectionString;

            var replicationSet = configuration.ReadConfig<string>(sectionName, ReplicationSet);
            var userName = configuration.ReadConfig<string>(sectionName, UserName);
            var userPassword = configuration.ReadConfig<string>(sectionName, UserPassword);
            var hostUrl = configuration.ReadConfig<string>(sectionName, HostUrl);

            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userPassword))
            {
                return $"mongodb://{hostUrl}";
            }
            else if (!string.IsNullOrWhiteSpace(replicationSet))
            {
                connectionString = $"mongodb+srv://{userName}:{userPassword}@{hostUrl}/?ssl=true&replicaSet={replicationSet}&authSource=admin&retryWrites=true&w=majority";
                return connectionString;
            }

            connectionString = $"mongodb+srv://{userName}:{userPassword}@{hostUrl}/?retryWrites=true&w=majority";
            return connectionString;
        }
    }
}
