using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace LanguageExt.Bson.Serialization
{
    internal class OptDocument
    {
        public Option<string> MaybeString { get; set; } = Option<string>.None;
        public Option<int> MaybeInt { get; set; } = Option<int>.None;
    }
    
    public class OptionSerializerTest : WithBsonSerializerSetup
    {
        private const string hello = "hello";

        private OptDocument SomeDoc => new OptDocument
        {
            MaybeString = hello,
            MaybeInt = 13
        };

        private OptDocument NoneDoc => new OptDocument();

        [Fact]
        public void Some_must_be_serialized_to_string()
        {
            var bson = SomeDoc.ToBsonDocument();

            bson.Contains("MaybeString").Should().BeTrue();
            bson["MaybeString"].AsString.Should().Be(hello);
            bson["MaybeInt"].AsInt32.Should().Be(13);
        }
        
        [Fact]
        public void None_must_be_serialized_to_null()
        {
            var bson = NoneDoc.ToBsonDocument();

            bson.Contains("MaybeString");
            bson["MaybeString"].IsBsonNull.Should().BeTrue();
            bson["MaybeInt"].IsBsonNull.Should().BeTrue();
        }

        [Fact]
        public void String_must_be_deserialized_to_some()
        {
            var bson = new BsonDocument
            {
                ["MaybeString"] = new BsonString(hello),
                ["MaybeInt"] = new BsonInt32(13)
            };

            var doc = (OptDocument) BsonSerializer.Deserialize(bson, typeof(OptDocument));

            doc.Should().BeEquivalentTo(SomeDoc);
        }

        [Fact]
        public void Null_must_be_deserialized_to_none()
        {
            var bson = new BsonDocument {["MaybeString"] = BsonNull.Value, ["MaybeInt"] = BsonNull.Value };

            var doc = (OptDocument) BsonSerializer.Deserialize(bson, typeof(OptDocument));

            doc.Should().BeEquivalentTo(NoneDoc);
        }
        

    }
}