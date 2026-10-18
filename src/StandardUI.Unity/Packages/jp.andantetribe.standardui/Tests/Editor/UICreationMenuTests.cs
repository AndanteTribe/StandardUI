#nullable enable

using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace StandardUI.Editor.Tests
{
    public sealed class UICreationMenuTests
    {
        private GameObject _root = null!;
        private Object? _previousSelection;

        [SetUp]
        public void SetUp()
        {
            _previousSelection = Selection.activeObject;
            _root = new GameObject("Canvas", typeof(Canvas));
            _root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }

        [TearDown]
        public void TearDown()
        {
            Undo.ClearUndo(_root);
            Object.DestroyImmediate(_root);
            Selection.activeObject = _previousSelection;
        }

        [Test]
        public void CreatesConfiguredImageUnderContext()
        {
            var panel = new GameObject("Panel", typeof(RectTransform));
            panel.transform.SetParent(_root.transform, false);
            UICreationMenu.CreateSimpleGauge(new MenuCommand(panel));
            var gauge = Selection.activeGameObject;
            Assert.That(gauge.transform.parent, Is.SameAs(panel.transform));
            Assert.That(gauge.GetComponent<WhiteSprite>(), Is.Not.Null);
            var image = gauge.GetComponent<UnityEngine.UI.Image>();
            Assert.That(image.type, Is.EqualTo(UnityEngine.UI.Image.Type.Filled));
            Assert.That(image.fillMethod, Is.EqualTo(UnityEngine.UI.Image.FillMethod.Horizontal));
            Assert.That(image.fillOrigin, Is.EqualTo((int)UnityEngine.UI.Image.OriginHorizontal.Left));
            Assert.That(image.fillAmount, Is.EqualTo(0.5f));
            Assert.That(image.raycastTarget, Is.False);
            Assert.That(image.sprite != null, Is.True);
            Assert.That(image.sprite!.texture, Is.SameAs(Texture2D.whiteTexture));
            Assert.That(((RectTransform)gauge.transform).sizeDelta, Is.EqualTo(new Vector2(200, 20)));
        }

        [Test]
        public void CreatesCanvasUnderNonUIContext()
        {
            var parent = new GameObject("NonUIParent");
            try
            {
                UICreationMenu.CreateSimpleGauge(new MenuCommand(parent));
                var gauge = Selection.activeGameObject;
                var canvas = gauge.GetComponentInParent<Canvas>();
                Assert.That(canvas, Is.Not.Null);
                Assert.That(canvas.transform.parent, Is.SameAs(parent.transform));
                Assert.That(canvas.GetComponent<UnityEngine.UI.CanvasScaler>(), Is.Not.Null);
                Assert.That(canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>(), Is.Not.Null);
            }
            finally
            {
                Undo.ClearUndo(parent);
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void CreatesUniqueSiblingNames()
        {
            UICreationMenu.CreateSimpleGauge(new MenuCommand(_root));
            var first = Selection.activeGameObject;
            UICreationMenu.CreateSimpleGauge(new MenuCommand(_root));
            Assert.That(Selection.activeGameObject.name, Is.Not.EqualTo(first.name));
        }

        [Test]
        public void CreationCanBeUndoneInOneStep()
        {
            UICreationMenu.CreateSimpleGauge(new MenuCommand(_root));
            var gauge = Selection.activeGameObject;
            Undo.FlushUndoRecordObjects();
            Undo.IncrementCurrentGroup();
            Undo.PerformUndo();
            Assert.That(gauge == null, Is.True);
            Assert.That(_root != null, Is.True);
        }
    }
}