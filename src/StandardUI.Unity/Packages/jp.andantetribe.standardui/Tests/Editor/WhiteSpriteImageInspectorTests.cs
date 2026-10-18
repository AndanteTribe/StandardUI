#nullable enable

using System.Reflection;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace StandardUI.Editor.Tests
{
    public sealed class WhiteSpriteImageInspectorTests
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
        public void DefaultImageInspectorLocksWhileHelperExists()
        {
            var editor = UnityEditor.Editor.CreateEditor(_image);
            try
            {
                Assert.That(editor, Is.TypeOf<WhiteSpriteImageInspector>());
                Assert.That(IsLocked(editor), Is.False);
                var helper = _object.AddComponent<WhiteSprite>();
                Assert.That(IsLocked(editor), Is.True);
                helper.enabled = false;
                Assert.That(IsLocked(editor), Is.True);
                helper.enabled = true;
                Assert.That(IsLocked(editor), Is.True);
                _object.SetActive(false);
                Assert.That(IsLocked(editor), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(editor);
            }
        }

        [Test]
        public void MixedSelectionLocksSourceIfAnyHelperExists()
        {
            var second = new GameObject("Second", typeof(UnityEngine.UI.Image));
            var helper = _object.AddComponent<WhiteSprite>();
            var editor = UnityEditor.Editor.CreateEditor(new Object[]
                { _image, second.GetComponent<UnityEngine.UI.Image>() });
            try
            {
                Assert.That(IsLocked(editor), Is.True);
                Object.DestroyImmediate(helper);
                Assert.That(IsLocked(editor), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(editor);
                Object.DestroyImmediate(second);
            }
        }

        [Test]
        public void UndoRedoAddingHelperSupportsNormalUnityUndo()
        {
            Undo.IncrementCurrentGroup();
            try
            {
                Undo.AddComponent<WhiteSprite>(_object);
                Assert.That(IsWhite(_image.sprite), Is.True);
                Undo.FlushUndoRecordObjects();
                Undo.IncrementCurrentGroup();
                Undo.PerformUndo();
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Null);
                Undo.PerformRedo();
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Not.Null);
                Assert.That(IsWhite(_image.sprite), Is.True);
            }
            finally
            {
                Undo.ClearUndo(_object);
            }
        }

        [UnityTest]
        public IEnumerator AddingToImageWithExistingSpriteReportsErrorAndRejectsHelper()
        {
            var original = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            _image.sprite = original;
            try
            {
                LogAssert.Expect(LogType.Error,
                    "WhiteSprite cannot be added to an Image with an existing Source Image. Clear Source Image first.");
                Undo.AddComponent<WhiteSprite>(_object);
                Assert.That(_image.sprite, Is.SameAs(original));
                yield return null;
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Null);
                Assert.That(_image.sprite, Is.SameAs(original));
            }
            finally
            {
                Undo.ClearUndo(_object);
                Object.DestroyImmediate(original);
            }
        }

        [UnityTest]
        public IEnumerator AddingToInactiveImageWithExistingSpriteIsRejected()
        {
            _object.SetActive(false);
            var original = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            _image.sprite = original;
            try
            {
                LogAssert.Expect(LogType.Error,
                    "WhiteSprite cannot be added to an Image with an existing Source Image. Clear Source Image first.");
                Undo.AddComponent<WhiteSprite>(_object);
                yield return null;
                Assert.That(_object.GetComponent<WhiteSprite>(), Is.Null);
                Assert.That(_image.sprite, Is.SameAs(original));
            }
            finally
            {
                Undo.ClearUndo(_object);
                Object.DestroyImmediate(original);
            }
        }

        private static bool IsWhite(Sprite? sprite) => sprite != null && sprite.texture == Texture2D.whiteTexture;

        private static bool IsLocked(UnityEditor.Editor editor) =>
            (bool)typeof(WhiteSpriteImageInspector).GetProperty("IsSourceImageLocked",
                BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(editor)!;
    }
}