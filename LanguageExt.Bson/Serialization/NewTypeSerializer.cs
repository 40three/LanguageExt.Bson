using System;
using System.Reflection;
using LanguageExt.TypeClasses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// Serializes internal value of the NewType based objects with provided representation
    /// </summary>
    /// <typeparam name="NEWTYPE"></typeparam>
    public class NewTypeSerializer<NEWTYPE> :
        SealedClassSerializerBase<NEWTYPE>,
        IRepresentationConfigurable<NewTypeSerializer<NEWTYPE>>
        where NEWTYPE : class
    {
        private readonly IBsonSerializer _internalSerializer;
        public BsonType Representation { get; }

        public NewTypeSerializer(BsonType representation)
        {
            Representation = representation;
            if (!typeof(NEWTYPE).IsSubclassOfGeneric(typeof(NewType<,,,>), out var newType)) throw
                new ArgumentException("NewTypeSerializer could only be used with subclasses of the NewType<,,,> class");
            var genericArguments = newType.GetGenericArguments();
            var valueType = genericArguments[1];
            var internalSerializerType = typeof(NewTypeSerializer<,,,>).MakeGenericType(genericArguments);
            var typeInfo = internalSerializerType.GetTypeInfo();
            var constructor = typeInfo.GetConstructor(new[] {typeof(IBsonSerializer<>).MakeGenericType(valueType)});
            if (constructor == null)
                throw new ApplicationException("No constructor with IBsonSerializer<A> parameter found");
            var valueSerializer = BsonSerializer.LookupSerializer(valueType);
            _internalSerializer = (IBsonSerializer) constructor.Invoke(new object[]
            {
                valueSerializer is IRepresentationConfigurable representationConfigurableSerializer
                    ? representationConfigurableSerializer.WithRepresentation(representation)
                    : valueSerializer
            });
        }

        public override NEWTYPE Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => (NEWTYPE)_internalSerializer.Deserialize(context, args);

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NEWTYPE value)
            => _internalSerializer.Serialize(context, args, value);

        public NewTypeSerializer<NEWTYPE> WithRepresentation(BsonType representation) =>
            representation == Representation ? this : new NewTypeSerializer<NEWTYPE>(representation);
        
        IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation) =>
            WithRepresentation(representation);
    }
    
    /// <summary>
    /// Serializes internal value of the NewType based objects
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class NewTypeSerializer<NEWTYPE, A, PRED, ORD> : SealedClassSerializerBase<NEWTYPE>
        where NEWTYPE : NewType<NEWTYPE, A, PRED, ORD>
        where PRED : struct, Pred<A>
        where ORD: struct, Ord<A>
    {
        private readonly IBsonSerializer<A> _itemSerializer;

        public NewTypeSerializer(IBsonSerializerRegistry registry): this(registry.GetSerializer<A>()) {}
        
        public NewTypeSerializer(IBsonSerializer<A> itemSerializer) => _itemSerializer = itemSerializer;

        public override NEWTYPE Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => NewType<NEWTYPE, A, PRED, ORD>.New(_itemSerializer.Deserialize(context, args));

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NEWTYPE value)
            => value.Iter(a => _itemSerializer.Serialize(context, args, a));
    }
}