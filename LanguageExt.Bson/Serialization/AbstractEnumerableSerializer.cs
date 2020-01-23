using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// As Serializer that serializes lists the same way 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public abstract class AbstractEnumerableSerializer<A,B> : SerializerBase<A>, IBsonArraySerializer
        where A : IEnumerable<B>
    {
        private readonly IBsonSerializer<B> _itemSerializer;
        
        public AbstractEnumerableSerializer(IBsonSerializerRegistry registry)
        {
            _itemSerializer = registry.GetSerializer<B>();
        }
        
        /// <summary>
        /// Deserialize a Lst<A>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override A Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => CreateFromValues(EnumerateValues(context, args));


        private IEnumerable<B> EnumerateValues(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartArray();
            var itemDeserializationArgs = GetItemDeserializationArgs(args);
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var item = _itemSerializer.Deserialize(context, itemDeserializationArgs);
                yield return item;
            }
            reader.ReadEndArray();
        }

        public bool TryGetItemSerializationInfo(out BsonSerializationInfo serializationInfo)
        {
            serializationInfo = new BsonSerializationInfo(null, _itemSerializer, typeof(A));
            return true;
        }

        /// <summary>
        /// Serialize a Lst
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, A value)
        {
            context.Writer.WriteStartArray();

            var itemSerializationArgs = GetItemSerializationArgs(args);
            foreach (var item in value)
            {
                _itemSerializer.Serialize(context, itemSerializationArgs,  item);
            }            
            context.Writer.WriteEndArray();
        }

        protected abstract A CreateFromValues(IEnumerable<B> values);
        
        private static BsonDeserializationArgs GetItemDeserializationArgs(BsonDeserializationArgs args)
            => ArgumentHelper.GetSpecificDeserializationArgs(args);

        private static BsonSerializationArgs GetItemSerializationArgs(BsonSerializationArgs args)
            => ArgumentHelper.GetSpecificSerializationArgs(args);
    }
}