using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace LanguageExt.Bson.Serialization
{   
    internal class SetDocument
    {
        public Set<string> Values { get; set; }
    }
    
    public class SetSerializerTest : WithBsonSerializerSetup
    {
        
        private List<string> data = new List<string>{ "hello", "world"};
        
        [Fact]
        public void SetSerializer_should_serialize_a_list_of_strings_as_an_array()
        {
            // given
            var strings = new SetDocument
            {
                Values = new Set<string>(data, true)
            };
            
            // when
            var bson = strings.ToBsonDocument();
            
            // then
            bson.Contains("Values").Should().BeTrue();
            bson["Values"].IsBsonArray.Should().BeTrue();
            var bsonValues = bson["Values"].AsBsonArray.ToList();
            bsonValues.Should().Equal(new BsonString("hello"), new BsonString("world"));
            
        }

        [Fact]
        public void SetSerializer_should_deserialize_an_array_of_strings()
        {
            // given
            var doc = new BsonDocument();

            var array = new BsonArray {new BsonString("hello"), new BsonString("world")};

            doc["Values"] = array;
            
            // when

            var deserializedLst = BsonSerializer.Deserialize<SetDocument>(doc);
            
            // then

            deserializedLst.Values.Count.Should().Be(2);
            deserializedLst.Values.AsEnumerable().Should().BeEquivalentTo(data);
        }

        [Fact]
        public void SetSerialzier_deserializes_empty_set()
        {
            var doc = new BsonDocument();
            doc["Values"] = new BsonArray();

            var deserialized = BsonSerializer.Deserialize<SetDocument>(doc);

            deserialized.Values.Count.Should().Be(0);
        }
    }
}