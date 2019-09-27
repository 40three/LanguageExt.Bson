using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson.Serialization
{
    public static class ArgumentHelper
    {
        public static BsonDeserializationArgs GetSpecificDeserializationArgs(BsonDeserializationArgs serializationArgs, int typeIndex = 0)
        {
            var itemType = serializationArgs.NominalType.GetGenericArguments()[typeIndex];
            return new BsonDeserializationArgs { NominalType = itemType};
        }

        public static BsonSerializationArgs GetSpecificSerializationArgs(BsonSerializationArgs deserializationArgs, int typeIndex = 0)
        {
            var itemType = deserializationArgs.NominalType.GetGenericArguments()[typeIndex];
            return new BsonSerializationArgs
            {
                NominalType = itemType,
                SerializeIdFirst = deserializationArgs.SerializeIdFirst,
                SerializeAsNominalType = deserializationArgs.SerializeAsNominalType
            };
        }
    }
}