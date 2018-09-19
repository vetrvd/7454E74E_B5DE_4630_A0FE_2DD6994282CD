using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model
{
    public class Note : IEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public User Author { get; set; }
        public IEnumerable<string> Tag { get; set; }
        public string Text { get; set; }
        
    }
}