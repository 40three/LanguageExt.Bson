using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace LanguageExt.Bson.Serialization
{
    internal class LstDocument
    {
        public Lst<string> Values { get; set; }
    }

    internal class ListDocument
    {
        public List<string> Values { get; set; }
    }
    
    public class LstSerializerTest : WithBsonSerializerSetup
    {   
        [Fact]
        public void LstSerializer_should_serialize_a_list_of_strings_as_an_array()
        {
            // given
            var strings = new LstDocument
            {
                Values = List.create("hello", "world")
            };

            var comparison = new ListDocument
            {
                Values = new List<string>(new[] {"hello", "world"})
            };
            
            // when
            var bson = strings.ToBsonDocument();
            var bsonComp = comparison.ToBsonDocument();
            
            // then
            bson.Contains("Values").Should().BeTrue();
            bson["Values"].IsBsonArray.Should().BeTrue();
            var bsonValues = bson["Values"].AsBsonArray.ToList();
            bsonValues.Should().Equal(new BsonString("hello"), new BsonString("world"));
            
        }

        [Fact]
        public void LstSerializer_should_deserialize_an_array_of_strings()
        {
            // given
            var doc = new BsonDocument();

            var array = new BsonArray {new BsonString("hello"), new BsonString("world")};

            doc["Values"] = array;
            
            // when

            var deserializedLst = (LstDocument) BsonSerializer.Deserialize(doc, typeof(LstDocument));
            var deserializedList = (ListDocument) BsonSerializer.Deserialize(doc, typeof(ListDocument));
            
            // then

            deserializedLst.Values.Count.Should().Be(2);
            deserializedLst.Values.AsEnumerable().Should().BeEquivalentTo(deserializedList.Values);
        }
    }
}