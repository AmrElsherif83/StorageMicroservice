using Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Domain.Entities
{
    [AdaptTo("FileMetadataDto"), GenerateMapper]
    [BsonIgnoreExtraElements]
    
    public class FileMetadata
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("FileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("FileType")]
        public string FileType { get; set; } = string.Empty;

        [BsonElement("FileSize")]
        public long FileSize { get; set; }

        [BsonElement("StorageProvider")]
        public string StorageProvider { get; set; } = string.Empty;

        [BsonElement("Url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

}
