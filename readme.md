# MiCore
MiCore - Core library for my programs and other libraries.

MiCore is an accumulation of my old libraries, SharpID, SharpEntity, SharpLogger, SharpSerial and 
SharpTest and is used as the core framework for my games and applications.

## Dependencies
- SFML.Net `https://github.com/graphnode/SFML.Net.git`

## Usage
See `MiCoreTest/Tests/` for example usage.

## TODO
- Look into why job tests occasionally fail.

## Changelog

### Version 0.3.0
- `Entity`, `Component` classes have been renamed to `MiEntity` and `MiComponent`.
- Added `MiNode<T>` abstract class to provide a parent-child relationship tree of type `T` for
  dervied classes.
- Added both sync and async ECS job functionality. `MiJob` objects contain job delegates and info,
  `JobList` objects are used to manage jobs into different priorities, and a `JobManager` object
  contains the job lists in priority order and is designed to run the jobs in order.
- Component related functionality has been moved from `MiEntity` to a new class `ComponentStack`.
  An entities components are now accessed through `MiEntity.Components`.
- Now `MiObject` and `MiEntity` implement `ICloneable`, this means all classes that inherit from 
  `Component` must return a deep copy of the object via `Clone()`. This is so that entities can be 
  cloned without knowing the actual type of their components.
- `MiObject.Dispose()` is no longer meant to be overridden, instead override `MiObject.OnDispose()`,
  this ensures the object is only disposed of once.
- `MiEntity` now inherits from and implements `MiNode<MiEntity>`.
- Added `MiEntity.Release<T>()`, `MiEntity.Release(int)` and `MiEntity.Release(string)` methods for 
  removing and extracting components without disposing of them.

### Version 0.2.0
- Removed the need for `DataT` type in `ISerializable` and removed `ISerializable.FileData`. Because
  of this, default behaviour for `TextSerializable.SaveToStream(StreamWriter)` will write the result
  of `ToString()` to the stream instead of `FileData` and 
  `BinarySerializable.SaveToStream(BinaryWriter)` is now completely abstract.
- Added `IXmlSerializable` interface and `XmlSerializable` class for classes that are serialized and
  deserialized in xml files node by node.
- `MiObject.Dispose()` is now virtual rather than abstract so classes that inherit from it no longer
  need to implement it.
- `Component.RequiredComponents` and `Component.IncompatibleComponents` now default to null instead
  of an empty string array.

### Version 0.1.0
- Initial release.
