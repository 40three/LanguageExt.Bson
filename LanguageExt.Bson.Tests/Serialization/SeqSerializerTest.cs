using System.Collections.Generic;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace LanguageExt.Bson.Serialization
{
    internal class SeqDocument
    {
        public Seq<string> Values { get; set; }
    }

    internal class ListDocumentSeq
    {
        public List<string> Values { get; set; }
    }
    
    public class SeqSerializerTest : WithBsonSerializerSetup
    {   
        [Fact]
        public void SeqSerializer_should_serialize_a_seq_of_strings_as_an_array()
        {
            // given
            var strings = new SeqDocument
            {
                Values = Seq.create("hello", "world")
            };

            var comparison = new ListDocumentSeq
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

            var deserializedSeq = (SeqDocument) BsonSerializer.Deserialize(doc, typeof(SeqDocument));
            var deserializedList = (ListDocumentSeq) BsonSerializer.Deserialize(doc, typeof(ListDocumentSeq));
            
            // then

            deserializedSeq.Values.Count.Should().Be(2);
            deserializedSeq.Values.AsEnumerable().Should().BeEquivalentTo(deserializedList.Values);
        }
    }
}