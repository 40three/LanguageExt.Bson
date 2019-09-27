# LanguageExt.Bson

This library provides BSON-serializers for MongoDB.Bson for most LanguageExt containers.

Packages:

*LanguageExt.Bson* provides the serializers
*LanguageExt.Bson.DependencyInjection* provides `IServiceCollection` extensions 

Setup
-----

Call `LanguageExtBsonSerializer.Setup` at least once. Once you have done so, all supported types will be (de)serializable
to and from BSON. Alternatively, include *LanguageExt.Bson.DependencyInjection* and call the `AddLanguageExtBsonSerializers`
extension method on your IServiceCollection. This is functionally the same thing, as all MongoDB.Bson setup is held 
in a global variable anyway.

Supported Types
---------------

 Type | Serialized as
 -----|--------------
 `Option<A>` | Some as `A`, None as `BsonNull`
 `Lst<A>` | `BsonArray`
 `Seq<A>` | `BsonArray`
 `Set<A>` | `BsonArray`
 `Map<string,A>` | `BsonDocument` with the keys as element names
 `Map<A,B>` | Analogous to `DictionaryRepresentation.ArrayOfDocuments`, that is, a `BsonArray` consisting of `{"k": a, "v": b}` documents
 
 Future Work
 -----------
 
- Allow Map<string,A> to be serialized as an `ArrayOfDocuments`, too, if desired.
- Implement a serializer for `Either`