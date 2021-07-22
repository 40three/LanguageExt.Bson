using LanguageExt.TypeClasses;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serializes internal value of the NewType based objects
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class NewTypeSerializer<NEWTYPE,A,PRED> : SerializerBase<NewType<NEWTYPE,A,PRED>>
        where NEWTYPE : NewType<NEWTYPE,A,PRED>
        where PRED : struct, Pred<A>
    {
        private readonly IBsonSerializer<A> _itemSerializer;

        public NewTypeSerializer(IBsonSerializerRegistry registry) => _itemSerializer = registry.GetSerializer<A>();

        public override NewType<NEWTYPE,A,PRED> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => NewType<NEWTYPE,A,PRED>.New(_itemSerializer.Deserialize(context, GetItemDeserializationArgs(args)));

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NewType<NEWTYPE,A,PRED> value)
            => value.Iter(a => _itemSerializer.Serialize(context, GetItemSerializationArgs(args), a));

        private static BsonDeserializationArgs GetItemDeserializationArgs(BsonDeserializationArgs args)
            => ArgumentHelper.GetSpecificDeserializationArgs(args, 1);

        private static BsonSerializationArgs GetItemSerializationArgs(BsonSerializationArgs args)
            => ArgumentHelper.GetSpecificSerializationArgs(args, 1);
    }
}