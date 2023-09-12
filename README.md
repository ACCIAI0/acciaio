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
This section presents an overview of the points above. For some of them, the overview should be enough to understand and use the feature, whilst the others are presented in more details in the [Wiki](../../wiki/Home).

### Installation

> [!INFO]
> **COMPATIBLE WITH**: Unity **2021.3.x and higher**.

Acciaio for Unity can be added using the OpenUPM registry:
```
openupm add com.abambini.acciaio
```
Alternatively, it can be added to a Unity project as a Git URL using the Package Manager or by modifying the `manifest.json` file directly:
```
"com.abambini.acciaio": "https://github.com/ACCIAI0/acciaio.git#upm"
```
> [!IMPORTANT]
> *The Package Manager doesn't handle versioning when installing packages as a Git URL and won't notify if there are new versions available to download. For the same reason, it won't allow to do a rollback to an older version after an update. Please, refer to this page to check if the new version is compatible with your version of the Unity editor.*

### $\textcolor{green}{\textsf{Extension Methods}}$
Many extension methods have been added for vector-like types (from `Vector2` to `Color`) that should reduce the number of boilerplate required to change or swizzle single components. 

For instance: all Vector types now have a method `yx()` which returns a `Vector2` with x component equal to the y component of the starting vector and viceversa. All types have methods of the family `With_(float value)` where the underscore represent any of the components (*x, y, z, w* for vectors and *r, g, b, a* for colors.) Multiple calls can be chained: 
```C#
vector = vector.WithX(0).WithZ(1)...
```
but it's *preferable to use the constructor if most of the components is going to change*.

### $\textcolor{green}{\textsf{Coroutine Runner}}$
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

### $\textcolor{darkred}{\textsf{Systems}}$
The Systems architecture proposes a different paradigm for handling high-level functionalities that span across scenes and the scenes themselves. 

In order to enable the Systems, this package provides a handy-dandy shortcut to create and pre-populate a *Systems scene*: 
```Tools >> Acciaio >> Create Systems Scene```
This scene contains all Systems of the application and once loaded it won't be unloaded until the application is closed. It can be loaded by simply calling `Systems.Load()`, which loads up the scene and initializes all Systems. If the Systems scene has been renamed the method to call is `Systems.Load("Systems_Scene_Name")`. Both calls return a data structure that can be yielded upon to wait for the full initialization. 

The Systems scene comes with two default Systems: `ScenesSystem` and `PubSubSystem`. 

The ScenesSystem is used to handle scenes and is supposed to replace the native ScenesManager. It exposes functionalities to Load and Unload scenes from both the build and Addressables. It automatically handles fading screens (if assigned) and can use SceneReferences to load the corresponding scene (see [Scene Reference Wiki](../../wiki/SceneReference).) Together with this System comes another feature of Acciaio: the override of the starting scene when playing from editor. If enabled from the Project settings, pressing play will start the game from the scene specified in the Project settings instead of the one that was being edited. This can be used to always guarantee, for example, the execution of some bootstrap logic before any scene is actually loaded.

The PubSubSystem is used to transfer settings or configurations between scenes.

Users can add new Systems to the scene and they will be automatically initialized together with the default ones when calling `Systems.Load()`. Custom Systems can be created by extending the type `BaseSystem<T>` or implementing `ISystem<T>`, although the second method won't allow to access that system as a Singleton.

> [!NOTE]
> Systems not on a root GameObject will be ignored when initializing the Systems scene.
___
### Quick References

- [Systems](../../wiki/Systems)
- [Editor Scenes Settings](../../wiki/EditorScenesSettings)
- [Extensions](../../wiki/Extensions)
