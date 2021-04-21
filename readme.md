# MiCore
MiCore - Core library for my programs and other libraries.

MiCore is an accumulation of my old libraries, SharpID, SharpEntity, SharpLogger, SharpSerial and 
SharpTest and is used as the core framework for my games and applications.

## Dependencies
- SFML.Net `https://github.com/SFML/SFML.Net.git`

## Usage
See `MiCoreTest/Tests/` for example usage.

## TODO

## Changelog

### Version 0.10.0
- Added generic `TypeRegister` class that is used for registering string IDs to types and using the
  IDs to create objects of the registered types. `ComponentRegister` inherits from this.
- Added `SubscribeEvents(Window)` and `UnsubscribeEvents(Window)` virtual methods to `MiObject` to
  replace `MiComponent` and `MiEntity`'s `TextEntered(TextEventArgs)` method and expand for other
  events.
- `MiComponent` now contains virtual functions `OnAdd()` and `OnRemove()` that are triggered when
  the component is added to and removed from an entity; these are intended to initialize the new
  parent entities' other components.
- Added virtual `Refresh()` method to `MiComponent` that is called before `OnUpdate(float)` for
  refreshing a components' visual elements without updating runtime logic. `MiEntity.Refresh()` is
  also included as a passthrough. This is so components and entities can be visualized without being
  interacted with.

### Version 0.9.1
- Fixed bug where `MiEntity.LoadFromStream(BinaryReader)` would report failure even when it 
  succeeded.

### Version 0.9.0
- Added functionality to `MiEntity` for retrieving children by component.
- Fixed bug with `MiEntity.AddComponent` where if the component is added successfully, but adding
  one of the required components fails, the leftover components are not removed.
- Fixed bug where `MiNode<T>` was not saving and loading its ID in `LoadFromStream(BinaryReader)`,
  `SaveToStream(BinaryWriter)` and `LoadFromXml(XmlElement)`.
- Fixed bug where `MiNode<T>.ToString()` and `MiEntity.ToString()` mixed up the value for `Visible`
  with `Enabled`.
- `MiNode<T>` is no longer `INamable` and no longer has `Name` as `IIdentifiable<string>` with 'ID'
  does the job.
- `MiNode.HasChild(string,bool)`, `MiNode.GetChild(string,bool)`, `MiNode.RemoveChild(string,bool)`
  and `MiNode.ReleaseChild(string,bool)` now all support path-like syntax to access children.

### Version 0.8.0
- When adding a component to an entity, its required components are now added after the component
  rather than before to prevent component order issues. Due to this, all `Entity.Insert*` methods
  have been removed.
- Removed `Paths.Executable` and `Paths.ExecutableFolder` due to unforseen reflection issues.

### Version 0.7.0
- Now `MiComponent.RequiredComponents` and `MiComponent.IncompatibleComponents` are readonly and are
  assigned on construction with the `MiComponent.GetRequiredComponents()` and 
  `MiComponent.GetIncompatibleComponents()` methods respectively.
- Merged some path methods from `MiGfx`.

### Version 0.6.0
- `ComponentStack` has been merged back into `MiEntity` and the methods and properties have been
  renamed to show they interact with components.
- Added `MiEntity.ReleaseAllComponents()` that removes all components from an entity without
  disposing of them and returns an array containing them.
- Now `MiComponent` implements `IEquatable<MiComponent>` as a base for deriving classes.

### Version 0.5.0
- Changed SFML source to latest official SFML.Net repository.

### Version 0.4.0
- `MiEntity` objects now require a reference to the render window, `MiEntity.Window` to be assigned
  in order to contain components that need it. When setting `MiEntity.Window`, it will be applied
  to all child entities too.
- `MiComponent` now contains methods for handling text entered events. `TextEntered(TextEventArgs)`
  is to be called and passed the event args when the event happens and will then call the virtual
  `OnTextEntered(TextEventArgs)` which you would override to handle the event. Because of this,
  `MiEntity.TextEntered(TextEventArgs)` is provided to call the event on all components and
  all components of all children.
- `ComponentStack.Add<T>(bool)` has been renamed to `AddNew<T>(bool)` and has been replaced with
  `Add<T>(T, bool)` to add an object of the given type to the stack.
- Added methods `Insert(int, MiComponent, bool)`, `Insert<T>(int, T, bool)`,
  `InsertNew<T>(int, bool)` and 'AddRange(IEnumerable<MiComponent>, bool)` to `ComponentStack` for
  inserting components into perticular indicies in the stack. 
  `AddRange(IEnumerable<MiComponent>, bool)` has also been added to easily add a range of components
  from a collection.

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
