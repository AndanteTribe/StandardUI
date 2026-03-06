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
| `EmptyGraphic` | A no-op Graphic component. Useful as a mask base, full-screen transparent overlay, or a transparent hit area for buttons. |
| `PropagationGraphic` | Forwards `Selectable` color transitions to child `Graphic` components. |
| `SafeArea` | Adjusts a `RectTransform` to match the device safe area. |
| `SimpleGauge` | A simple and clean gauge display built on top of `RectMask2D`. |
| `SkewMeshEffect` | A `BaseMeshEffect` that skews a uGUI mesh diagonally. |
| `TouchSurface` | Controls UI input, such as temporarily blocking all uGUI interaction. |

## Requirements
- Unity 2021.3 or later
- [com.unity.ugui](https://docs.unity3d.com/Manual/com.unity.ugui.html) 1.0.0 or later

## Installation
Open `Window > Package Manager`, select `[+] > Add package from git URL`, and enter the following URL:

```
https://github.com/AndanteTribe/StandardUI.git?path=src/StandardUI.Unity/Packages/jp.andantetribe.standardui
```

## Quick Start

### EmptyGraphic
Add the `EmptyGraphic` component to a `GameObject` to create a transparent, non-rendering UI element — ideal for button hit areas or mask bases.

### PropagationGraphic
Attach `PropagationGraphic` to a parent `Selectable` to propagate its color transitions to child `Graphic` components.

```csharp
// Child graphics are automatically discovered via Reset() in the Editor,
// or you can assign them manually in the Inspector.
```

### SafeArea
Attach the `SafeArea` component to a `RectTransform` to automatically fit the layout to the device's safe area.

### SimpleGauge
Add the `SimpleGauge` component to a `GameObject` that has a `MaskableGraphic` and control the fill via the `Value` property.

```csharp
using StandardUI;
using UnityEngine;

public class GaugeSample : MonoBehaviour
{
    [SerializeField] private SimpleGauge _gauge;

    private void Update()
    {
        // Set gauge fill (0.0 to 1.0)
        _gauge.Value = Mathf.PingPong(Time.time * 0.5f, 1f);
    }
}
```

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

### EmptyGraphic

| Member | Description |
|--------|-------------|
| `SetMaterialDirty()` | No-op override. |
| `SetVerticesDirty()` | No-op override. |

### PropagationGraphic

| Member | Description |
|--------|-------------|
| `CrossFadeColor(Color, float, bool, bool)` | Forwards the color transition to all assigned child `Graphic` components. |

### SafeArea

| Member | Description |
|--------|-------------|
| `Adjust()` | Applies the current safe area to this `RectTransform`'s anchors. |

### SimpleGauge

| Member | Description |
|--------|-------------|
| `Value` | Gets or sets the gauge fill (0.0 to 1.0). Setting this updates the `RectMask2D` padding. |

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
