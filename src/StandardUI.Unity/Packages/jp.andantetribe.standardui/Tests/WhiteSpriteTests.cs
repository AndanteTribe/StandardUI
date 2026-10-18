#nullable enable

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StandardUI.Tests
{
    public sealed class WhiteSpriteTests
    {
        private GameObject _root = null!;
        private UnityEngine.UI.Image _image = null!;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("Canvas", typeof(Canvas));
            _root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var imageObject = new GameObject("Image", typeof(UnityEngine.UI.Image));
            imageObject.transform.SetParent(_root.transform, false);
            _image = imageObject.GetComponent<UnityEngine.UI.Image>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void RuntimeAddComponentAssignsWhiteWithoutKeepingExistingSprite()
        {
            var original = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            try
            {
                _image.sprite = original;
                var helper = _image.gameObject.AddComponent<WhiteSprite>();
                Assert.That(_image.sprite, Is.Not.SameAs(original));
                Object.DestroyImmediate(helper);
                Assert.That(_image.sprite, Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(original);
            }
        }

        [Test]
        public void EnablingAssignsSharedWhiteSpriteAndDisablingKeepsIt()
        {
            var helper = _image.gameObject.AddComponent<WhiteSprite>();
            var shared = _image.sprite;
            Assert.That(shared, Is.Not.Null);
            Assert.That(shared.texture, Is.SameAs(Texture2D.whiteTexture));

            helper.enabled = false;
            Assert.That(_image.sprite, Is.SameAs(shared));
            helper.enabled = true;
            Assert.That(_image.sprite, Is.SameAs(shared));
        }

        [Test]
        public void RemovingHelperClearsWhiteSpriteAndKeepsImage()
        {
            var helper = _image.gameObject.AddComponent<WhiteSprite>();
            Object.DestroyImmediate(helper);
            Assert.That(_image, Is.Not.Null);
            Assert.That(_image.sprite, Is.Null);
        }

        [Test]
        public void RemovingHelperPreservesSpriteAssignedByCode()
        {
            var helper = _image.gameObject.AddComponent<WhiteSprite>();
            var custom = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            try
            {
                _image.sprite = custom;
                Object.DestroyImmediate(helper);
                Assert.That(_image.sprite, Is.SameAs(custom));
            }
            finally
            {
                Object.DestroyImmediate(custom);
            }
        }

        [Test]
        public void HelpersShareOneSpriteAndRemovalDoesNotBreakOtherImages()
        {
            var first = _image.gameObject.AddComponent<WhiteSprite>();
            var secondObject = new GameObject("Second", typeof(WhiteSprite));
            secondObject.transform.SetParent(_root.transform, false);
            var secondImage = secondObject.GetComponent<UnityEngine.UI.Image>();
            var shared = _image.sprite;
            Assert.That(secondImage.sprite, Is.SameAs(shared));
            Object.DestroyImmediate(first);
            Assert.That(secondImage.sprite, Is.SameAs(shared));
            Assert.That(secondImage.sprite != null, Is.True);
        }

        [Test]
        public void HelperDoesNotChangeImageSettingsOrRectTransform()
        {
            _image.type = UnityEngine.UI.Image.Type.Filled;
            _image.fillMethod = UnityEngine.UI.Image.FillMethod.Vertical;
            _image.fillOrigin = 1;
            _image.fillAmount = 0.25f;
            _image.color = Color.cyan;
            _image.raycastTarget = false;
            var rect = (RectTransform)_image.transform;
            rect.sizeDelta = new Vector2(200, 80);
            _image.gameObject.AddComponent<WhiteSprite>();
            Assert.That(_image.type, Is.EqualTo(UnityEngine.UI.Image.Type.Filled));
            Assert.That(_image.fillMethod, Is.EqualTo(UnityEngine.UI.Image.FillMethod.Vertical));
            Assert.That(_image.fillOrigin, Is.EqualTo(1));
            Assert.That(_image.fillAmount, Is.EqualTo(0.25f));
            Assert.That(_image.color, Is.EqualTo(Color.cyan));
            Assert.That(_image.raycastTarget, Is.False);
            Assert.That(rect.sizeDelta, Is.EqualTo(new Vector2(200, 80)));
        }

        [Test]
        public void ReenablingDoesNotReassignWhiteSprite()
        {
            var helper = _image.gameObject.AddComponent<WhiteSprite>();
            helper.enabled = false;
            _image.sprite = null;
            helper.enabled = true;
            Assert.That(_image.sprite, Is.Null);
            helper.enabled = false;
            Assert.That(_image.sprite, Is.Null);
        }

        [Test]
        public void DeactivatingAndReactivatingObjectKeepsWhiteSprite()
        {
            _image.gameObject.AddComponent<WhiteSprite>();
            var shared = _image.sprite;
            _image.gameObject.SetActive(false);
            Assert.That(_image.sprite, Is.SameAs(shared));
            _image.gameObject.SetActive(true);
            Assert.That(_image.sprite, Is.SameAs(shared));
        }

        [Test]
        public void NewInstanceRecreatesDestroyedSharedSprite()
        {
            _image.gameObject.AddComponent<WhiteSprite>();
            var previous = _image.sprite;
            Object.DestroyImmediate(previous);
            var second = new GameObject("Second", typeof(WhiteSprite));
            second.transform.SetParent(_root.transform, false);
            var sprite = second.GetComponent<UnityEngine.UI.Image>().sprite;
            Assert.That(sprite != null, Is.True);
            Assert.That(sprite, Is.Not.SameAs(previous));
            Assert.That(sprite!.texture, Is.SameAs(Texture2D.whiteTexture));
        }

        [UnityTest]
        public IEnumerator WhiteSpriteSupportsNormalImageFillAfterOneFrame()
        {
            _image.gameObject.AddComponent<WhiteSprite>();
            _image.type = UnityEngine.UI.Image.Type.Filled;
            _image.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            ((RectTransform)_image.transform).sizeDelta = new Vector2(200, 80);
            yield return null;
            _image.fillAmount = 0.25f;
            Canvas.ForceUpdateCanvases();
            var mesh = _image.canvasRenderer.GetMesh();
            Assert.That(mesh.vertexCount, Is.EqualTo(4));
            Assert.That(mesh.bounds.size.x, Is.EqualTo(50).Within(0.01f));
        }
    }
}