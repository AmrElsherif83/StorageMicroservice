using MongoDB.Driver;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.Repository
{
    public class FileRepository : IRepository<FileMetadata>
    {
        private readonly IMongoCollection<FileMetadata> _collection;

        public FileRepository(MongoDbContext context)
        {
            _collection = context.Files;
        }

        public async Task<IEnumerable<FileMetadata>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();

        public async Task<FileMetadata> GetByIdAsync(Guid id) => await _collection.Find(f => f.Id == id).FirstOrDefaultAsync();

        public async Task AddAsync(FileMetadata entity) => await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(FileMetadata entity) => await _collection.ReplaceOneAsync(f => f.Id == entity.Id, entity);

        public async Task DeleteAsync(Guid id) => await _collection.DeleteOneAsync(f => f.Id == id);
    }
}
