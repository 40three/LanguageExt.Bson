using System;
using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Bson.Serialization
{
    public class MapSerializerTest : WithBsonSerializerSetup
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] {Map<object,object>.Empty, new BsonDocument()};
                yield return new object[] {Map<object,object>((9, 8), (7, "bar")), new BsonDocument
                    {
                        {"foo", "bar"},
                        {"hallo", 8}
                    }
                };
                yield return new object[] {Map<object, object>((Guid.Empty, 109)), new BsonDocument {{"id", 109}}};
            }
        }
        
        [Theory]
        [MemberData(nameof(TestData))]
        public void MapSerializerWorks(Map<object, object> map, BsonDocument document)
        {

            var serialized = map.ToBsonDocument();
            var deserialized = BsonSerializer.Deserialize<Map<object, object>>(document);

            serialized.Should().Equal(document);
            deserialized.Should<(object,object)>().Equal(map);
        }
        
    }
}