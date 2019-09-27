using System;
using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Bson.Serialization
{
    internal class WrapperDoc
    {
        public Map<object, object> Data { get; set; }
    }
    
    public class MapSerializerTest : WithBsonSerializerSetup
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] {Map<object,object>.Empty, new BsonArray()};
                yield return new object[] {Map<object,object>((9, 8), (7, "bar")), new BsonArray 
                    {
                        new BsonDocument {
                            {"k", 7},
                            {"v", "bar"}
                        },
                        new BsonDocument {
                            {"k", 9},
                            {"v", 8}
                        }
                    }
                };
                yield return new object[] {Map<object, object>((Guid.Empty, 109)), new BsonArray
                {
                    new BsonDocument{ {"k", Guid.Empty}, {"v", 109}}
                }};
            }
        }
        
        [Theory]
        [MemberData(nameof(TestData))]
        public void MapSerializerWorksWhenWrappedInADocument(Map<object, object> map, BsonArray array)
        {

            var wrapper = new WrapperDoc {Data = map};
            var document = new BsonDocument {{"Data", array}};
            
            var serialized = wrapper.ToBsonDocument();
            var deserialized = BsonSerializer.Deserialize<WrapperDoc>(document);

            serialized.Should().Equal(document);
            deserialized.Data.Should<(object,object)>().Equal(map);
        }
        
    }
}