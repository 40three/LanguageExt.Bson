using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson.Serialization
{
    public class LanguageExtCollectionSerializationProvider : BsonSerializationProviderBase
    {
        
        public override IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry registry)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(Lst<>))
                {
                    return CreateGenericSerializer(typeof(LstSerializer<>), type.GetGenericArguments(), registry);
                }

                if (genericTypeDefinition == typeof(Set<>))
                {
                    return CreateGenericSerializer(typeof(SetSerializer<>), type.GetGenericArguments(), registry);
                }

                if (genericTypeDefinition == typeof(Option<>))
                {
                    return CreateGenericSerializer(typeof(OptionSerializer<>), type.GetGenericArguments(), registry);
                }

                if (genericTypeDefinition == typeof(Map<,>))
                {
                    var types = type.GenericTypeArguments;
                    if (types[0] == typeof(string))
                    {
                        return CreateGenericSerializer(typeof(StringMapSerializer<>), types[1]);
                    }
                    
                    return CreateGenericSerializer(typeof(MapSerializer<,>), type.GetGenericArguments(), registry);
                }
            }
            
            // fall back to default mongo serialization providers
            return null;
        }
    }
}