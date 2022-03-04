# Acciaio Systems

A package that offers the following main functionalities:
- Easy to use Editor Scenes settings and setup flow
- Extension methods for common `UnityEngine` types such as `Color` and `Vector3`.
- **Systems architecture**

## Overview

The main feature of this package is the Systems architecture, a set of tools and classes that aims at quickly setting up and managing the main aspects of a Game in an easy to use manner. To begin, just click <span style="color: cyan">**Tools>>Acciaio>>Create Systems Scene**</span>. This will automatically create a new scene `Assets/_Scenes/Systems.unity`, pre-populated with two ready-to-use Systems.

The `Systems` scene is the heart of the architecture and **cannot be renamed** or it will cause runtime errors. It contains two pre-made systems and more can be added by the user in form of GameObjects with Components that derive from `ISystem`. Each GameObject in the root of the scene has **one and only one** System attached to it, any Component not on a root GameObject will be ignored.

**NOTE**: Systems will be accessible with the Singleton pattern only if they are subclasses of `BaseSystem<TSystem>`. 