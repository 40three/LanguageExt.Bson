using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// As Serializer that serializes lists the same way 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class SeqSerializer<A> : AbstractEnumerableSerializer<Seq<A>, A> {
        protected override Seq<A> CreateFromValues(IEnumerable<A> values) => new Seq<A>(values).Strict();

        public SeqSerializer(IBsonSerializerRegistry registry) : base(registry)
        {
        }
    }
}