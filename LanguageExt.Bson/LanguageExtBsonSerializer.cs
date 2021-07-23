using LanguageExt.Bson.Serialization;
using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson
{
    /// <summary>
    /// Configure BsonSerializers for usage with MongoDB.Bson
    /// </summary>
    public static class LanguageExtBsonSerializer
    {
        private static readonly object StaticLock = new object();
        private static bool _bsonSerializationInitialized;

        /// <summary>
        /// BSON serialization setup is static and leads to error if the same serializer or serialization providers
        /// have already been set up, so we make sure to set it up only once
        /// </summary>
        public static void Setup()
        {
            lock (StaticLock)
            {
                if (!_bsonSerializationInitialized)
                {
                    BsonSerializer.RegisterSerializationProvider(new LanguageExtCollectionSerializationProvider());
                    BsonSerializer.RegisterSerializationProvider(new NewTypeSerializationProvider());
                    _bsonSerializationInitialized = true;
                }
            }
        }
    }
}