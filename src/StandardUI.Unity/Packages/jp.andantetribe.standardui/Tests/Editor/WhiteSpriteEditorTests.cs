#nullable enable

using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace StandardUI.Editor.Tests
{
    public sealed class WhiteSpriteEditorTests
    {
        private GameObject _object = null!;
        private UnityEngine.UI.Image _image = null!;

        [SetUp]
        public void SetUp()
        {
            _object = new GameObject("Image", typeof(UnityEngine.UI.Image));
            _image = _object.GetComponent<UnityEngine.UI.Image>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_object);

        [Test]
        public void ImageUsesStandardInspectorWithHelper()
        {
            _object.AddComponent<WhiteSprite>();
            var editor = UnityEditor.Editor.CreateEditor(_image);
            try
            {
                Assert.That(editor, Is.TypeOf<UnityEditor.UI.ImageEditor>());
            }
            finally
            {
                Object.DestroyImmediate(editor);
            }
        }

        [Test]
        public void OnEnableRecreatesMissingEditModePreview()
        {
            var helper = _object.AddComponent<WhiteSprite>();
            var shared = _image.sprite;
            Assert.That(shared != null, Is.True);
            helper.enabled = false;
            _image.sprite = null;
            helper.enabled = true;
            Assert.That(_image.sprite, Is.SameAs(shared));
        }

        [Test]
        public void OnEnableRecreatesDestroyedSprite()
        {
            var helper = _object.AddComponent<WhiteSprite>();
            helper.enabled = false;
            Object.DestroyImmediate(_image.sprite);
            helper.enabled = true;
            Assert.That(_image.sprite != null, Is.True);
            Assert.That(_image.sprite!.texture, Is.SameAs(Texture2D.whiteTexture));
        }

        [Test]
        public void AddingAndResettingHelperPreservesExistingSprite()
        {
            var original = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            _image.sprite = original;
            try
            {
                var helper = _object.AddComponent<WhiteSprite>();
                typeof(WhiteSprite).GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(helper, null);
                Assert.That(_image.sprite, Is.SameAs(original));
                Object.DestroyImmediate(helper);
                Assert.That(_image.sprite, Is.SameAs(original));
            }
            finally
            {
                Object.DestroyImmediate(original);
            }
        }

        [Test]
        public void UndoRedoAddingHelperReappliesWhiteSprite()
        {
            Undo.IncrementCurrentGroup();
            try
            {
                Undo.AddComponent<WhiteSprite>(_object);
                Assert.That(_image.sprite != null, Is.True);
                Undo.FlushUndoRecordObjects();
                Undo.IncrementCurrentGroup();
                Undo.PerformUndo();
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Null);
                Undo.PerformRedo();
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Not.Null);
                Assert.That(_image.sprite != null, Is.True);
            }
            finally
            {
                Undo.ClearUndo(_object);
            }
        }
    }
}
