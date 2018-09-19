using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model
{
    public interface IEntity
    {
        ObjectId Id { get; set; }
    }
}