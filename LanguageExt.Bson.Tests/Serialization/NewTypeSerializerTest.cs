using System;
using FluentAssertions;
using LanguageExt.ClassInstances;
using LanguageExt.ClassInstances.Const;
using LanguageExt.ClassInstances.Pred;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace LanguageExt.Bson.Serialization
{
    internal class ConstrainedString : NewType<ConstrainedString, string, StrLen<I4, I6>>
    {
        internal ConstrainedString(string value) : base(value) { }
    }

    internal class ConstrainedInt : NewType<ConstrainedInt, int, Range<TInt, int, I100, I200>>
    {
        internal ConstrainedInt(int value) : base(value) { }
    }

    internal class NewTypeObject
    {
        public ConstrainedString ConstrainedString { get; set; }
        public ConstrainedInt ConstrainedInt { get; set; }
    }
    
    public class NewTypeSerializerTest : WithBsonSerializerSetup
    {
        [Fact]
        public void ConstrainedString_must_be_serialized_to_string()
        {
            var bson = new NewTypeObject
            {
                ConstrainedString = ConstrainedString.New("1234"),
                ConstrainedInt = ConstrainedInt.New(150)
            }.ToBsonDocument();

            bson.Contains("ConstrainedString").Should().BeTrue();
            bson["ConstrainedString"].AsString.Should().Be("1234");
            bson.Contains("ConstrainedInt").Should().BeTrue();
            bson["ConstrainedInt"].AsInt32.Should().Be(150);
        }
        
        [Fact]
        public void Valid_string_must_be_deserialized_to_ConstrainedString()
        {
            var bson = new BsonDocument
            {
                ["ConstrainedString"] = new BsonString("1234"),
                ["ConstrainedInt"] = new BsonInt32(150)
            };

            var doc = (NewTypeObject) BsonSerializer.Deserialize(bson, typeof(NewTypeObject));

            doc.Should().BeEquivalentTo(new NewTypeObject
            {
                ConstrainedString = ConstrainedString.New("1234"),
                ConstrainedInt = ConstrainedInt.New(150)
            });
        }
        
        [Fact]
        public void Invalid_string_must_not_be_deserialized_to_ConstrainedString()
        {
            var bson = new BsonDocument
            {
                ["ConstrainedString"] = new BsonString("123"),
                ["ConstrainedInt"] = new BsonInt32(150)
            };

            FluentActions.Invoking(() => BsonSerializer.Deserialize(bson, typeof(NewTypeObject)))
                .Should()
                .Throw<FormatException>("The string is not a valid value for the ConstrainedString type")
                .WithMessage("*Specified argument was out of the range of valid values. (Parameter 'value')*");
        }
    }
}