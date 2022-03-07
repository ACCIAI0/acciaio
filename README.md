# Acciaio Systems

A package that offers the following main functionalities:
- Easy to use Editor Scenes settings and setup flow
- Extension methods for common `UnityEngine` types such as `Color` and `Vector3`.
- **Systems architecture**

## Overview

The main feature of this package is the Systems architecture, a set of tools and classes that aims at quickly setting up and managing the main aspects of a Game in an easy to use manner. To begin, just click **Tools>>Acciaio>>Create Systems Scene**. This will automatically create a new scene `Assets/_Scenes/Systems.unity`, pre-populated with two ready-to-use Systems.

Acciaio Systems also provides settings to tell Unity which scene to load when the Play button is pressed in the Editor. It is useful to always start the game from a bootstrap or initialization scene even when testing a specific scene without following the real game flow. 
These settings resides in an asset that can be created through **Tools>>Acciaio>>Create Editor Scenes Settings** or through the Create contextual menu **Create>>Acciaio>>Editor Scenes Settings**. 

**NOTE**: If created through the Create contextual menu, the new asset must be placed under *Assets/Editor/*, while the Tools menu will automatically place it in there, creating the folder if it doesn't exists.
___
### Quick References

- [Systems](../../wiki/Systems)
- [Editor Scenes Settings](../../wiki/EditorScenesSettings)
- [Extensions](../../wiki/Extensions)

## Systems

The `Systems` scene is the heart of the architecture and **cannot be renamed** or it will cause runtime errors. It contains two pre-made systems and more can be added by the user in form of GameObjects with Components that derive from `ISystem`. Each GameObject in the root of the scene has **one and only one** System attached to it, any `ISystem` not on a root GameObject will be ignored.

**NOTE**: Systems will be accessible with the Singleton pattern only if they are subclasses of `BaseSystem<TSystem>`. For all the other Systems they're still available through queries to the `Systems` class.
