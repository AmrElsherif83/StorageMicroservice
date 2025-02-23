using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using StorageMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.Data
{
    public class MongoDbContext : DbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.DatabaseName);
           
        }

        public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
        public IMongoCollection<FileMetadata> Files => _database.GetCollection<FileMetadata>("Files");
    }
}
