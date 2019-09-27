using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// As Serializer that serializes sets as lists 
    /// </summary>
    /// <typeparam name="A">value type</typeparam>
    public class SetSerializer<A> : AbstractEnumerableSerializer<Set<A>,A> {
        protected override Set<A> CreateFromValues(IEnumerable<A> values) => new Set<A>(values);

        public SetSerializer(IBsonSerializerRegistry registry) : base(registry)
        {
        }
    }
}