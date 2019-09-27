
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serialzes Some() as a direct value, None as null
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class OptionSerializer<A> : SerializerBase<Option<A>> {
        
        private readonly IBsonSerializer<A> _itemSerializer;

        private readonly BsonDeserializationArgs _deserializationArgs = new BsonDeserializationArgs
        {
            NominalType = typeof(A)
        };

        public OptionSerializer(IBsonSerializerRegistry registry)
        {
            _itemSerializer = registry.GetSerializer<A>();
        }
        
        public override Option<A> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return Option<A>.None;
            }

            var value = _itemSerializer.Deserialize(context, GetItemDeserializationArgs(args));
            return Option<A>.Some(value);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Option<A> value)
        {
            value.BiIter(
                a => _itemSerializer.Serialize(context, GetItemSerializationArgs(args), a),
                 () => context.Writer.WriteNull()
            );
        }

        private static BsonDeserializationArgs GetItemDeserializationArgs(BsonDeserializationArgs optionArgs)
            => ArgumentHelper.GetSpecificDeserializationArgs(optionArgs);

        private static BsonSerializationArgs GetItemSerializationArgs(BsonSerializationArgs optionArgs)
            => ArgumentHelper.GetSpecificSerializationArgs(optionArgs);
    }
}