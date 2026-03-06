# StandardUI
[![unity-meta-check](https://github.com/AndanteTribe/StandardUI/actions/workflows/unity-meta-check.yml/badge.svg)](https://github.com/AndanteTribe/StandardUI/actions/workflows/unity-meta-check.yml)
[![Releases](https://img.shields.io/github/release/AndanteTribe/StandardUI.svg)](https://github.com/AndanteTribe/StandardUI/releases)
[![GitHub license](https://img.shields.io/github/license/AndanteTribe/StandardUI.svg)](./LICENSE)
[![openupm](https://img.shields.io/npm/v/jp.andantetribe.standardui?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/jp.andantetribe.standardui/)

[English](README.md) | 日本語

## 概要
**StandardUI** は、Unity の uGUI に追加のコンポーネントとエフェクトを提供するパッケージです。

| コンポーネント | 説明 |
|--------------|------|
| `EmptyGraphic` | 描画を行わない Graphic コンポーネント。マスクのベース、全画面透明オーバーレイ、ボタンの透明なヒット領域として使用できます。 |
| `PropagationGraphic` | 親の `Selectable` のカラートランジションを子の `Graphic` コンポーネントに伝播します。 |
| `SafeArea` | デバイスのセーフエリアに合わせて `RectTransform` を調整します。 |
| `SimpleGauge` | `RectMask2D` を利用したシンプルなゲージ表示コンポーネントです。 |
| `SkewMeshEffect` | uGUI のメッシュを斜めに傾ける `BaseMeshEffect` です。 |
| `TouchSurface` | uGUI の入力を一時的にブロックするなど、UI 入力を制御します。 |

## 要件
- Unity 2021.3 以上
- [com.unity.ugui](https://docs.unity3d.com/Manual/com.unity.ugui.html) 1.0.0 以上

## インストール
`Window > Package Manager` から Package Manager ウィンドウを開き、`[+] > Add package from git URL` を選択して以下の URL を入力します。

```
https://github.com/AndanteTribe/StandardUI.git?path=src/StandardUI.Unity/Packages/jp.andantetribe.standardui
```

## クイックスタート

### EmptyGraphic
`EmptyGraphic` コンポーネントを `GameObject` に追加することで、描画しない透明な UI 要素を作成できます。ボタンのヒット領域やマスクのベースとして最適です。

### PropagationGraphic
親の `Selectable` に `PropagationGraphic` をアタッチすると、そのカラートランジションを子の `Graphic` コンポーネントに伝播できます。

```csharp
// Editor の Reset() で子 Graphic を自動検出するか、
// Inspector で手動で割り当てることもできます。
```

### SafeArea
`SafeArea` コンポーネントを `RectTransform` にアタッチすると、デバイスのセーフエリアに合わせてレイアウトが自動的に調整されます。

### SimpleGauge
`MaskableGraphic` を持つ `GameObject` に `SimpleGauge` コンポーネントを追加し、`Value` プロパティでゲージの量を制御します。

```csharp
using StandardUI;
using UnityEngine;

public class GaugeSample : MonoBehaviour
{
    [SerializeField] private SimpleGauge _gauge;

    private void Update()
    {
        // ゲージの量を設定する (0.0 〜 1.0)
        _gauge.Value = Mathf.PingPong(Time.time * 0.5f, 1f);
    }
}
```

### SkewMeshEffect
`Image` に `SkewMeshEffect` コンポーネントを追加すると、メッシュを斜めに傾けられます。斜めにカットされたゲージフィルやスタイリッシュな UI 要素の作成に活用できます。

### TouchSurface
`TouchSurface` を使用すると、長時間または非同期処理中に uGUI の入力を一時的に無効化できます。

```csharp
using StandardUI;
using UnityEngine;

public class TouchSurfaceSample : MonoBehaviour
{
    private readonly TouchSurface _touchSurface = new TouchSurface();

    private void SomeOperation()
    {
        // 処理中は UI 入力を無効化し、using スコープを抜けると自動的に復元されます。
        using (_touchSurface.BlockScope())
        {
            // 長時間または非同期の処理をここで行います。
        }
    }
}
```

## API

### EmptyGraphic

| メンバー | 説明 |
|--------|------|
| `SetMaterialDirty()` | 何もしないオーバーライドです。 |
| `SetVerticesDirty()` | 何もしないオーバーライドです。 |

### PropagationGraphic

| メンバー | 説明 |
|--------|------|
| `CrossFadeColor(Color, float, bool, bool)` | 割り当てられた子 `Graphic` コンポーネントすべてにカラートランジションを伝播します。 |

### SafeArea

| メンバー | 説明 |
|--------|------|
| `Adjust()` | 現在のセーフエリアをこの `RectTransform` のアンカーに適用します。 |

### SimpleGauge

| メンバー | 説明 |
|--------|------|
| `Value` | ゲージの量 (0.0 〜 1.0) を取得または設定します。設定時に `RectMask2D` のパディングが更新されます。 |

### SkewMeshEffect

| メンバー | 説明 |
|--------|------|
| `ModifyMesh(VertexHelper)` | メッシュの全頂点にスキュー変換を適用します。 |

### TouchSurface

| メンバー | 説明 |
|--------|------|
| `BlockScope()` | UI 入力を無効化し、`Dispose` 時に再有効化する `BlockingScope` を返します。 |

## ライセンス
このライブラリは MIT ライセンスで公開しています。
