using System;
using System.Collections.Generic;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serializes a string-keyed Map as a BsonDocument
    /// </summary>
    public class StringMapSerializer<A> : SerializerBase<Map<string, A>>, IBsonDocumentSerializer
    {
        private readonly IBsonSerializer<A> _valueSerializer;
        
        public StringMapSerializer(IBsonSerializerRegistry registry)
        {
            _valueSerializer = registry.GetSerializer<A>();
        }

        public override Map<string, A> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();

            var deserializationArgs = GetValueDeserializationArgs(args);
            var values = new List<ValueTuple<string, A>>();

            context.Reader.ReadBsonType();
            while (context.Reader.State != BsonReaderState.EndOfDocument)
            {
                var name = context.Reader.ReadName();
                var value = _valueSerializer.Deserialize(context, deserializationArgs);

                values.Add((name, value));
                context.Reader.ReadBsonType();
            }

            context.Reader.ReadEndDocument();

            return new Map<string, A>(values);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Map<string, A> value)
        {
            context.Writer.WriteStartDocument();
            var serializationArgs = GetValueSerializationArgs(args);
            foreach (var (k,v) in value)
            {
                context.Writer.WriteName(k);
                _valueSerializer.Serialize(context, serializationArgs, v);
            }
            
            context.Writer.WriteEndDocument();
        }
        
        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            serializationInfo = new BsonSerializationInfo(memberName, _valueSerializer, _valueSerializer.ValueType);
            return true;
        }
        
        private static BsonDeserializationArgs GetValueDeserializationArgs(BsonDeserializationArgs args)
            => ArgumentHelper.GetSpecificDeserializationArgs(args, 1);

        private static BsonSerializationArgs GetValueSerializationArgs(BsonSerializationArgs args)
            => ArgumentHelper.GetSpecificSerializationArgs(args, 1);
    }
}