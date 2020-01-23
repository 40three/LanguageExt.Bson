using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serializes a Map{A,B} the same way we would serialize a dictionary
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class MapSerializer<A,B> : SerializerBase<Map<A,B>>, IBsonDictionarySerializer
    {

        private const string Key = "k";
        private const string Value = "v";

        private readonly IBsonSerializer<A> _keySerializer;
        private readonly IBsonSerializer<B> _valueSerializer;
        
        public MapSerializer(IBsonSerializerRegistry registry)
        {
            _keySerializer = registry.GetSerializer<A>(); 
            _valueSerializer = registry.GetSerializer<B>();

        }
        
        public override Map<A, B> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => new Map<A, B>(EnumerateTuples(context, args));
        
        private IEnumerable<(A,B)> EnumerateTuples(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;

            var keyDeserializationArgs = ArgumentHelper.GetSpecificDeserializationArgs(args);
            var valueDeserializationArgs = ArgumentHelper.GetSpecificDeserializationArgs(args, 1);
            
            reader.ReadStartArray();

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                A key = default;
                B value = default;

                var keySet = false;
                var valueSet = false;
                
                reader.ReadStartDocument();
                while (reader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var elementName = reader.ReadName();
                    switch (elementName)
                    {
                        case Key:
                            keySet = true;
                            key = _keySerializer.Deserialize(context, keyDeserializationArgs);
                            break;
                        case Value:
                            valueSet = true;
                            value = _valueSerializer.Deserialize(context, valueDeserializationArgs);
                            break;
                        default:
                            throw new BsonSerializationException($"Unknown element name {elementName}");
                    }
                }
                
                reader.ReadEndDocument();
                
                if (!keySet)
                    throw new BsonSerializationException("Missing key");
                
                if (!valueSet)
                    throw new BsonSerializationException("Missing value");
                
                yield return (key, value);
            }
            
            reader.ReadEndArray();
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Map<A, B> value)
        {
            var writer = context.Writer;

            var keySerializationArgs = ArgumentHelper.GetSpecificSerializationArgs(args);
            var valueSerializationArgs = ArgumentHelper.GetSpecificSerializationArgs(args, 1);
            
            writer.WriteStartArray();
            foreach (var (k,v) in value)
            {
                writer.WriteStartDocument();
                writer.WriteName(Key);
                _keySerializer.Serialize(context, keySerializationArgs, k);
                writer.WriteName(Value);
                _valueSerializer.Serialize(context, valueSerializationArgs, v);
                writer.WriteEndDocument();
            }   
            
            writer.WriteEndArray();
        }

        public DictionaryRepresentation DictionaryRepresentation => DictionaryRepresentation.ArrayOfDocuments;
        public IBsonSerializer KeySerializer => _keySerializer;
        public IBsonSerializer ValueSerializer => _valueSerializer;
    }
}