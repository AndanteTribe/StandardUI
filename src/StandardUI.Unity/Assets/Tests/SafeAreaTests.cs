#nullable enable

using NUnit.Framework;
using UnityEngine;
#if UNITY_EDITOR
using Screen = UnityEngine.Device.Screen;
#else
using Screen = UnityEngine.Screen;
#endif

namespace StandardUI.Tests
{
    public sealed class SafeAreaTests
    {
        private GameObject _gameObject = null!;
        private RectTransform _rectTransform = null!;
        private SafeArea _safeArea = null!;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("SafeAreaTest", typeof(RectTransform));
            _rectTransform = _gameObject.GetComponent<RectTransform>();
            _safeArea = _gameObject.AddComponent<SafeArea>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Adjust_ResetsOffsetsToZero()
        {
            _rectTransform.offsetMin = new Vector2(10f, 20f);
            _rectTransform.offsetMax = new Vector2(-30f, -40f);

            _safeArea.Adjust();

            AssertVector2(Vector2.zero, _rectTransform.offsetMin);
            AssertVector2(Vector2.zero, _rectTransform.offsetMax);
        }

        [Test]
        public void Adjust_AppliesCurrentScreenSafeAreaToAnchors()
        {
            var screenSize = new Vector2(Screen.width, Screen.height);
            var safeArea = Screen.safeArea;

            var expectedAnchorMin = safeArea.min / screenSize;
            var expectedAnchorMax = safeArea.max / screenSize;

            _safeArea.Adjust();

            AssertVector2(expectedAnchorMin, _rectTransform.anchorMin);
            AssertVector2(expectedAnchorMax, _rectTransform.anchorMax);
        }

        private static void AssertVector2(Vector2 expected, Vector2 actual)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.0001f));
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.0001f));
        }
    }
}