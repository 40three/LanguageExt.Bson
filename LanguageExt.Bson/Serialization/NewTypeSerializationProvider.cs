using System;
using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson.Serialization
{
    public class NewTypeSerializationProvider : BsonSerializationProviderBase
    {
        public override IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry registry) =>
            type.IsSubclassOfGeneric(typeof(NewType<,,,>), out var newType)
                ? CreateGenericSerializer(typeof(NewTypeSerializer<,,,>), newType.GetGenericArguments(), registry)
                : null;
    }
}