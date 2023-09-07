# Acciaio for Unity

This library is part of the .NET Acciaio Project, a collection of libraries that can be used in different contexts where C# or .NET are involved. In particular, this library focuses on Unity runtime and editor.
It offers the following functionalities:
- Extension methods for common `UnityEngine` types such as `Vector2`, `Vector2Int`, `Vector3`, `Vector3Int`, `Vector4` and `Color`.
- Extension methods for `IEnumerable<T>` and other System.Collections.Generic implementing logic that could be of use for games such as shuffling the elements inside a collection
- A `CoroutineRunner` utility for starting/stopping globally managed Coroutines 
- New serializable types for specific use cases such as `Map<TKey, TValue` (a serializable dictionary), `TypeReference` (for referencing specific types) or `SceneReference` (used together with the `ScenesSystem`).
- An `Id` and `IdReference` architecture for both persistence and runtime decoupling
- the **Systems** infrastructure.

## Overview
This section presents an overview of the points above. For some of them, the overview should be enough to understand and use the feature, whilst the others are presentede in more details in the [Wiki](../../wiki/Home).

### Installation
Acciaio for Unity can be added to a Unity project through the Package Manager as a Git URL. There are plans to publish it on a public repository, but for the time being this is the only available method.

> [!IMPORTANT]
> *The Package Manager doesn't handle versioning when installing packages as a Git URL and won't notify if there are new versions available to download. For the same reason, it won't allow to do a rollback to an older version after an update. Please, refer to this page to check if the new version is compatible with your version of the Unity editor.*
>
> **COMPATIBLE WITH**: Unity **2021.3.x and higher**.

### <span style="color:green">Extension Methods</span>
Many extension methods have been added for vector-like types (from `Vector2` to `Color`) that should reduce the number of boilerplate required to change or swizzle single components. 

For instance: all Vector types now have a method `yx()` which returns a `Vector2` with x component equal to the y component of the starting vector and viceversa. All types have methods of the family `With_(float value)` where the underscore represent any of the components (*x, y, z, w* for vectors and *r, g, b, a* for colors.) Multiple calls can be chained: `vector = vector.WithX(0).WithZ(1)...`, but it's *preferable to use the constructor if most components are going to change*.

### CoroutineRunner
The class `CoroutineRunner` allows to statically start and stop coroutine, using the same syntax Unity has always offered:

```C#
IEnumerator RoutineBody() { ... }

...

var coroutine = CoroutineRunner.StartCoroutine(RoutineBody());

...

CoroutineRunner.StopCoroutine(coroutine);
```

It also allows to delay the execution of a method and to cancel it moving forward:

```C#
void Action() { ... }

...

var cancelableDelay = CoroutineRunner.ExecuteAfterSeconds(Action, 1.0f);

...

// Cancels the delayed execution
cancelableDelay.Cancel();
```

___
### Quick References

- [Systems](../../wiki/Systems)
- [Editor Scenes Settings](../../wiki/EditorScenesSettings)
- [Extensions](../../wiki/Extensions)

## Systems

The `Systems` scene is the heart of the architecture and **cannot be renamed** or it will cause runtime errors. It contains two pre-made systems and more can be added by the user in form of GameObjects with Components that derive from `ISystem`. Each GameObject in the root of the scene has **one and only one** System attached to it, any `ISystem` not on a root GameObject will be ignored.

**NOTE**: Systems will be accessible with the Singleton pattern only if they are subclasses of `BaseSystem<TSystem>`. For all the other Systems they're still available through queries to the `Systems` class.
