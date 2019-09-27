using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serializes a Map<A,B> the same way we would serialize a dictionary
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class MapSerializer<A,B> : SerializerBase<Map<A,B>>, IBsonDictionarySerializer
    {
        public MapSerializer(IBsonSerializerRegistry registry)
        {
            _dictionarySerializer = new DictionaryInterfaceImplementerSerializer<Dictionary<A, B>>(
                DictionaryRepresentation.ArrayOfDocuments, 
                registry.GetSerializer<A>(),
                registry.GetSerializer<B>());

        }

        private readonly DictionaryInterfaceImplementerSerializer<Dictionary<A, B>> _dictionarySerializer;
            
        public override Map<A, B> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var dict = _dictionarySerializer.Deserialize(context, new BsonDeserializationArgs());
            return new Map<A, B>(dict.Map(kv =>(kv.Key, kv.Value)));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Map<A, B> value)
        {
            var dict = new Dictionary<A, B>(value.ToDictionary());
            _dictionarySerializer.Serialize(context, args, dict);
        }

        public DictionaryRepresentation DictionaryRepresentation => _dictionarySerializer.DictionaryRepresentation;
        public IBsonSerializer KeySerializer => _dictionarySerializer.KeySerializer;
        public IBsonSerializer ValueSerializer => _dictionarySerializer.ValueSerializer;
    }
}