using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Bson.Serialization
{
    public class StringMapSerializerTest : WithBsonSerializerSetup
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] {Map<string,object>.Empty, new BsonDocument()};
                yield return new object[] {Map<string,object>(("hallo", 8), ("foo", "bar")), new BsonDocument
                    {
                        {"foo", "bar"},
                        {"hallo", 8}
                    }
                };
                yield return new object[] {Map<string, object>(("id", 109)), new BsonDocument {{"id", 109}}};
            }
        }
        
        [Theory]
        [MemberData(nameof(TestData))]
        public void StringSerializerWorks(Map<string, object> map, BsonDocument document)
        {

            var serialized = map.ToBsonDocument();
            var deserialized = BsonSerializer.Deserialize<Map<string, object>>(document);

            serialized.Should().Equal(document);
            deserialized.Should<(string,object)>().Equal(map);
        }

        [Fact]
        public void MapSerializationWorksWithMoreSpecificTypes()
        {
            var map = Map<string, long>(("id", 109));
            var doc = new BsonDocument {{"id", 109}};
            
            var serialized = map.ToBsonDocument();
            var deserialized = BsonSerializer.Deserialize<Map<string, long>>(doc);
            
            serialized.Should().Equal(doc);
            deserialized.Should<(string,long)>().Equal(map);
        }
    }
}