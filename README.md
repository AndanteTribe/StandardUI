# StandardUI
[![unity-meta-check](https://github.com/AndanteTribe/StandardUI/actions/workflows/unity-meta-check.yml/badge.svg)](https://github.com/AndanteTribe/StandardUI/actions/workflows/unity-meta-check.yml)
[![Releases](https://img.shields.io/github/release/AndanteTribe/StandardUI.svg)](https://github.com/AndanteTribe/StandardUI/releases)
[![GitHub license](https://img.shields.io/github/license/AndanteTribe/StandardUI.svg)](./LICENSE)
[![openupm](https://img.shields.io/npm/v/jp.andantetribe.standardui?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/jp.andantetribe.standardui/)

English | [日本語](README_JA.md)

## Overview
**StandardUI** is a Unity package that provides extra uGUI components and effects.

| Component | Description |
|-----------|-------------|
| `PropagationGraphic` | Forwards `Selectable` color transitions to child `Graphic` components. |
| `WhiteSprite` | Supplies a shared white Sprite to an Image when no Sprite is assigned. |
| `SkewMeshEffect` | A `BaseMeshEffect` that skews a uGUI mesh diagonally. |
| `TouchSurface` | Controls UI input, such as temporarily blocking all uGUI interaction. |

## Requirements
- Unity 6000.6 or later
- [com.unity.ugui](https://docs.unity3d.com/Manual/com.unity.ugui.html) 2.6.0 or later

## Upgrading to 2.0.0
The deprecated components have been removed. Replace them before upgrading; existing scene and prefab components are not migrated automatically.

| Removed component | Replacement |
|-------------------|-------------|
| `StandardUI.EmptyGraphic` | `UnityEngine.UI.RaycastReceiver` |
| `StandardUI.SafeArea` | `UnityEngine.UI.SafeArea` |
| `StandardUI.SimpleGauge` | An Image with Type `Filled`; use `Image.fillAmount` instead of `Value`. Add `WhiteSprite` for a shared white Sprite. |

`PropagationGraphic` now derives from `UnityEngine.UI.RaycastReceiver`.

## Installation
Open `Window > Package Manager`, select `[+] > Add package from git URL`, and enter the following URL:

```
https://github.com/AndanteTribe/StandardUI.git?path=src/StandardUI.Unity/Packages/jp.andantetribe.standardui
```

## Quick Start

### PropagationGraphic
Attach `PropagationGraphic` to a parent `Selectable` to propagate its color transitions to child `Graphic` components.

```csharp
// Child graphics are automatically discovered via Reset() in the Editor,
// or you can assign them manually in the Inspector.
```

### WhiteSprite
Attach `WhiteSprite` to an Image to supply a shared white Sprite when Source Image is empty. This is checked on enable in both Edit Mode and Play Mode; existing Sprites are preserved and the standard Image Inspector remains editable. Removing the helper clears only its shared white Sprite. Hierarchy > UI > Simple Gauge (UI (Canvas) in Unity 6.6+) creates an Image with WhiteSprite and horizontal Fill Amount configured.

### SkewMeshEffect
Add the `SkewMeshEffect` component to an `Image` to apply a diagonal skew to the mesh — useful for creating slanted gauge fills or stylised UI elements.

### TouchSurface
Use `TouchSurface` to temporarily disable all uGUI input during long or asynchronous operations.

```csharp
using StandardUI;
using UnityEngine;

public class TouchSurfaceSample : MonoBehaviour
{
    private readonly TouchSurface _touchSurface = new TouchSurface();

    private void SomeOperation()
    {
        // Disable all UI input during processing and restore it automatically
        // when leaving the using scope.
        using (_touchSurface.BlockScope())
        {
            // Perform long or asynchronous operations here.
        }
    }
}
```

## API

### PropagationGraphic

| Member | Description |
|--------|-------------|
| `CrossFadeColor(Color, float, bool, bool)` | Forwards the color transition to all assigned child `Graphic` components. |

### SkewMeshEffect

| Member | Description |
|--------|-------------|
| `ModifyMesh(VertexHelper)` | Applies the skew transform to every vertex in the mesh. |

### TouchSurface

| Member | Description |
|--------|-------------|
| `BlockScope()` | Disables UI input and returns a `BlockingScope` that re-enables it on `Dispose`. |

## License
This library is released under the MIT license.
