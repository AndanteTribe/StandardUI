#nullable enable

using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace StandardUI.Tests
{
    public sealed class SimpleGaugeTests
    {
        private const float GaugeWidth = 200f;
        private const float GaugeHeight = 80f;

        private static readonly BindingFlags s_instanceNonPublic =
            BindingFlags.Instance | BindingFlags.NonPublic;

        private GameObject _root = null!;
        private RectTransform _rectTransform = null!;
        private Image _image = null!;
        private SimpleGauge _gauge = null!;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
            _root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var gaugeObject = new GameObject("SimpleGauge", typeof(RectTransform), typeof(Image));
            gaugeObject.transform.SetParent(_root.transform, false);

            _rectTransform = gaugeObject.GetComponent<RectTransform>();
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.sizeDelta = new Vector2(GaugeWidth, GaugeHeight);

            _image = gaugeObject.GetComponent<Image>();
            _gauge = gaugeObject.AddComponent<SimpleGauge>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_root);
        }

        [TestCase(RectTransform.Edge.Left, 150f, 0f, 0f, 0f)]
        [TestCase(RectTransform.Edge.Right, 0f, 0f, 150f, 0f)]
        [TestCase(RectTransform.Edge.Top, 0f, 0f, 0f, 60f)]
        [TestCase(RectTransform.Edge.Bottom, 0f, 60f, 0f, 0f)]
        public void ValueSetsPaddingForEachMode(
            RectTransform.Edge mode,
            float left,
            float bottom,
            float right,
            float top)
        {
            SetSerializedField("_mode", mode);

            _gauge.Value = 0.25f;

            Assert.That(_gauge.Value, Is.EqualTo(0.25f));
            AssertVector4(new Vector4(left, bottom, right, top), _gauge.padding);
        }

        [TestCase(-0.5f, 0f, 200f)]
        [TestCase(1.5f, 1f, 0f)]
        public void ValueClampsToUnitInterval(float input, float expectedValue, float expectedRightPadding)
        {
            SetSerializedField("_mode", RectTransform.Edge.Right);

            _gauge.Value = input;

            Assert.That(_gauge.Value, Is.EqualTo(expectedValue));
            AssertVector4(new Vector4(0f, 0f, expectedRightPadding, 0f), _gauge.padding);
        }

        [Test]
        public void ValueThrowsWhenModeIsInvalid()
        {
            var invalidMode = (RectTransform.Edge)int.MaxValue;
            SetSerializedField("_mode", invalidMode);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _gauge.Value = 0.5f);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception!.ParamName, Is.EqualTo("_mode"));
            Assert.That(exception.ActualValue, Is.EqualTo(invalidMode));
            Assert.That(_gauge.Value, Is.EqualTo(0.5f));
        }

        [Test]
        public void StartAddsMaskableGraphicAsClippable()
        {
            Canvas.ForceUpdateCanvases();
            _gauge.PerformClipping();
            Assert.That(_image.canvasRenderer.hasRectClipping, Is.False);

            InvokeLifecycleMethod("Start");
            _gauge.PerformClipping();

            Assert.That(_image.canvasRenderer.hasRectClipping, Is.True);
        }

#if UNITY_EDITOR
        [Test]
        public void OnValidateReappliesSerializedValue()
        {
            SetSerializedField("_mode", RectTransform.Edge.Bottom);
            SetSerializedField("_value", -0.25f);
            _gauge.padding = new Vector4(1f, 2f, 3f, 4f);

            InvokeLifecycleMethod("OnValidate");

            Assert.That(_gauge.Value, Is.Zero);
            AssertVector4(new Vector4(0f, GaugeHeight, 0f, 0f), _gauge.padding);
        }
#endif

        private void SetSerializedField<T>(string fieldName, T value)
        {
            var field = typeof(SimpleGauge).GetField(fieldName, s_instanceNonPublic);
            Assert.That(field, Is.Not.Null);
            field!.SetValue(_gauge, value);
        }

        private void InvokeLifecycleMethod(string methodName)
        {
            var method = typeof(SimpleGauge).GetMethod(methodName, s_instanceNonPublic);
            Assert.That(method, Is.Not.Null);
            method!.Invoke(_gauge, null);
        }

        private static void AssertVector4(Vector4 expected, Vector4 actual)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.0001f));
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.0001f));
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(0.0001f));
            Assert.That(actual.w, Is.EqualTo(expected.w).Within(0.0001f));
        }
    }
}
