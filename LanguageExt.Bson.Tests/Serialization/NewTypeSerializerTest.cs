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
    internal class ConstrainedString : NewType<ConstrainedString, string, StrLen<I24, I24>>
    {
        internal ConstrainedString(string value) : base(value) { }
    }

    internal class ConstrainedInt : NewType<ConstrainedInt, int, Range<TInt, int, I100, I200>>
    {
        internal ConstrainedInt(int value) : base(value) { }
    }

    internal class ConstrainedDecimal : NewType<ConstrainedDecimal, decimal>
    {
        internal ConstrainedDecimal(decimal value) : base(value) { }
    }

    internal class NewTypeObject
    {
        public ConstrainedString ConstrainedString { get; set; }
        public ConstrainedInt ConstrainedInt { get; set; }
    }
    
    internal class NewTypeObjectWithRepresentation
    {
        public ConstrainedString ConstrainedString { get; set; }
        public ConstrainedDecimal ConstrainedDecimal { get; set; }
    }
    
    public class NewTypeSerializerTest : WithBsonSerializerSetup
    {
        [Fact]
        public void NewType_must_be_serialized_to_value_type()
        {
            var bson = new NewTypeObject
            {
                ConstrainedString = ConstrainedString.New("123456123456123456123456"),
                ConstrainedInt = ConstrainedInt.New(150)
            }.ToBsonDocument();

            bson.Contains("ConstrainedString").Should().BeTrue();
            bson["ConstrainedString"].AsString.Should().Be("123456123456123456123456");
            bson.Contains("ConstrainedInt").Should().BeTrue();
            bson["ConstrainedInt"].AsInt32.Should().Be(150);
        }
        
        [Fact]
        public void NewType_must_be_serialized_to_value_type_using_provided_representation()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(NewTypeObjectWithRepresentation)))
            {
                BsonClassMap.RegisterClassMap<NewTypeObjectWithRepresentation>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(x => x.ConstrainedString)
                        .SetSerializer(new NewTypeSerializer<ConstrainedString>(BsonType.ObjectId));
                    cm.MapMember(x => x.ConstrainedDecimal)
                        .SetSerializer(new NewTypeSerializer<ConstrainedDecimal>(BsonType.Decimal128));
                });
            }
            var bson = new NewTypeObjectWithRepresentation
            {
                ConstrainedString = ConstrainedString.New("123456123456123456123456"),
                ConstrainedDecimal = ConstrainedDecimal.New(150)
            }.ToBsonDocument();

            bson.Contains("ConstrainedString").Should().BeTrue();
            bson["ConstrainedString"].AsObjectId.Should().Be(new ObjectId("123456123456123456123456"));
            bson.Contains("ConstrainedDecimal").Should().BeTrue();
            bson["ConstrainedDecimal"].AsDecimal128.Should().Be(150);
        }
        
        [Fact]
        public void Valid_value_must_be_deserialized_to_NewType()
        {
            var bson = new BsonDocument
            {
                ["ConstrainedString"] = new BsonString("123456123456123456123456"),
                ["ConstrainedInt"] = new BsonInt32(150)
            };

            var doc = (NewTypeObject) BsonSerializer.Deserialize(bson, typeof(NewTypeObject));

            doc.Should().BeEquivalentTo(new NewTypeObject
            {
                ConstrainedString = ConstrainedString.New("123456123456123456123456"),
                ConstrainedInt = ConstrainedInt.New(150)
            });
        }
        
        [Fact]
        public void Valid_value_must_be_deserialized_to_NewType_with_provided_representation()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(NewTypeObjectWithRepresentation)))
            {
                BsonClassMap.RegisterClassMap<NewTypeObjectWithRepresentation>(cm =>
                {
                    cm.AutoMap();
                    cm.MapMember(x => x.ConstrainedString)
                        .SetSerializer(new NewTypeSerializer<ConstrainedString>(BsonType.ObjectId));
                    cm.MapMember(x => x.ConstrainedDecimal)
                        .SetSerializer(new NewTypeSerializer<ConstrainedDecimal>(BsonType.Decimal128));
                });
            }

            var bson = new BsonDocument
            {
                ["ConstrainedString"] = new ObjectId("123456123456123456123456"),
                ["ConstrainedDecimal"] = new BsonDecimal128(150)
            };

            var doc = (NewTypeObjectWithRepresentation) BsonSerializer.Deserialize(bson, typeof(NewTypeObjectWithRepresentation));

            doc.Should().BeEquivalentTo(new NewTypeObjectWithRepresentation
            {
                ConstrainedString = ConstrainedString.New("123456123456123456123456"),
                ConstrainedDecimal = ConstrainedDecimal.New(150)
            });
        }
        
        [Fact]
        public void Invalid_value_must_not_be_deserialized_to_NewType()
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