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
| `PropagationGraphic` | 親の `Selectable` のカラートランジションを子の `Graphic` コンポーネントに伝播します。 |
| `WhiteSprite` | Sprite 未指定の Image に共有の白 Sprite を補充するヘルパーです。 |
| `SkewMeshEffect` | uGUI のメッシュを斜めに傾ける `BaseMeshEffect` です。 |
| `TouchSurface` | uGUI の入力を一時的にブロックするなど、UI 入力を制御します。 |

## 要件
- Unity 6000.6 以上
- [com.unity.ugui](https://docs.unity3d.com/Manual/com.unity.ugui.html) 2.6.0 以上

## 2.0.0 への移行
非推奨コンポーネントを削除しました。アップグレード前に置き換えてください。既存のシーン・Prefab のコンポーネントは自動移行されません。

| 削除したコンポーネント | 代替 |
|----------------------|------|
| `StandardUI.EmptyGraphic` | `UnityEngine.UI.RaycastReceiver` |
| `StandardUI.SafeArea` | `UnityEngine.UI.SafeArea` |
| `StandardUI.SimpleGauge` | Type を `Filled` にした Image。`Value` の代わりに `Image.fillAmount` を使用します。共有の白 Sprite が必要なら `WhiteSprite` を追加してください。 |

`PropagationGraphic` の継承元を `UnityEngine.UI.RaycastReceiver` に変更しました。

## インストール
`Window > Package Manager` から Package Manager ウィンドウを開き、`[+] > Add package from git URL` を選択して以下の URL を入力します。

```
https://github.com/AndanteTribe/StandardUI.git?path=src/StandardUI.Unity/Packages/jp.andantetribe.standardui
```

## クイックスタート

### PropagationGraphic
親の `Selectable` に `PropagationGraphic` をアタッチすると、そのカラートランジションを子の `Graphic` コンポーネントに伝播できます。

```csharp
// Editor の Reset() で子 Graphic を自動検出するか、
// Inspector で手動で割り当てることもできます。
```

### WhiteSprite
Image に `WhiteSprite` を追加すると、Source Image が空の場合だけ共有の白 Sprite を補充します。編集・プレイ両モードの有効化時に確認し、既存 Sprite は保持します。Image は標準 Inspector で編集でき、ヘルパー削除時には共有の白 Sprite だけを外します。Hierarchy の UI > Simple Gauge（Unity 6.6 以降は UI (Canvas)）から、WhiteSprite と水平 Fill Amount を設定済みの Image を作成できます。

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

### PropagationGraphic

| メンバー | 説明 |
|--------|------|
| `CrossFadeColor(Color, float, bool, bool)` | 割り当てられた子 `Graphic` コンポーネントすべてにカラートランジションを伝播します。 |

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
