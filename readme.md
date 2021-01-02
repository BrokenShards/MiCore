# MiCore
MiCore - Core library for my programs and other libraries.

MiCore is an accumulation of my old libraries, SharpID, SharpEntity, SharpLogger, SharpSerial and SharpTest
and is used as the core framework for my games and applications.

## Usage
See `MiCoreTest/Test.cs` for usage.

## Dependencies
- SFML.Net `https://github.com/graphnode/SFML.Net.git`

## Changelog

### Version 0.2.0
- Removed the need for `DataT` type in `ISerializable` and removed `ISerializable.FileData`. Because of this,
  default behaviour for `TextSerializable.SaveToStream(StreamWriter)` will write the result of `ToString()`
  to the stream instead of `FileData` and `BinarySerializable.SaveToStream(BinaryWriter)` is now completely
  abstract.
- Added `IXmlSerializable` interface and `XmlSerializable` class for classes that are serialized and deserialized
  in xml files node by node.
- `MiObject.Dispose()` is now virtual rather than abstract so classes that inherit from it no longer need to
  implement it.
- `Component.RequiredComponents` and `Component.IncompatibleComponents` now default to null instead of an
  empty string array.

### Version 0.1.0
- Initial release.
