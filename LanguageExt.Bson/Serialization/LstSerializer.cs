using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LanguageExt.Bson.Serialization
{
    /// <summary>
    /// As Serializer that serializes lists the same way 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class LstSerializer<A> : AbstractEnumerableSerializer<Lst<A>, A>
    {
        protected override Lst<A> CreateFromValues(IEnumerable<A> values) => new Lst<A>(values);

        public LstSerializer(IBsonSerializerRegistry registry) : base(registry)
        {
        }
    }
}