#nullable enable

using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace StandardUI.Tests
{
    public sealed class PropagationGraphicTests
    {
        private static readonly FieldInfo s_graphicsField = typeof(PropagationGraphic).GetField(
            "_graphics",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

#if UNITY_EDITOR
        private static readonly MethodInfo s_resetMethod = typeof(PropagationGraphic).GetMethod(
            "Reset",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
#endif

        private GameObject _root = null!;
        private PropagationGraphic _propagationGraphic = null!;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(
                "PropagationGraphic",
                typeof(RectTransform),
                typeof(PropagationGraphic));
            _propagationGraphic = _root.GetComponent<PropagationGraphic>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

#if UNITY_EDITOR
        [Test]
        public void ResetCollectsDescendantGraphicsIncludingInactiveAndExcludesSelf()
        {
            var activeGraphic = CreateChildImage("ActiveGraphic");
            var inactiveContainer = new GameObject("InactiveContainer", typeof(RectTransform));
            inactiveContainer.transform.SetParent(_root.transform, false);
            var inactiveGraphic = CreateChildImage(
                "NestedInactiveGraphic",
                inactiveContainer.transform);
            inactiveContainer.SetActive(false);

            Assert.That(s_resetMethod, Is.Not.Null);
            s_resetMethod.Invoke(_propagationGraphic, null);

            var graphics = (Graphic[])s_graphicsField.GetValue(_propagationGraphic);
            Assert.That(graphics, Has.Length.EqualTo(2));
            CollectionAssert.AreEquivalent(
                new Graphic[] { activeGraphic, inactiveGraphic },
                graphics);
            CollectionAssert.DoesNotContain(graphics, _propagationGraphic);
        }
#endif

        [Test]
        public void CrossFadeColorUpdatesOnlyActiveAndEnabledGraphics()
        {
            var activeGraphic = CreateChildImage("ActiveGraphic");
            var disabledGraphic = CreateChildImage("DisabledGraphic");
            disabledGraphic.enabled = false;

            activeGraphic.canvasRenderer.SetColor(Color.black);
            disabledGraphic.canvasRenderer.SetColor(Color.black);
            s_graphicsField.SetValue(
                _propagationGraphic,
                new Graphic[] { activeGraphic, disabledGraphic });

            var targetColor = new Color(0.25f, 0.5f, 0.75f, 0.4f);
            _propagationGraphic.CrossFadeColor(
                targetColor,
                duration: 0f,
                ignoreTimeScale: true,
                useAlpha: true);

            Assert.That(activeGraphic.canvasRenderer.GetColor(), Is.EqualTo(targetColor));
            Assert.That(disabledGraphic.canvasRenderer.GetColor(), Is.EqualTo(Color.black));
        }

        [Test]
        public void CrossFadeColorAcceptsAnEmptyGraphicList()
        {
            s_graphicsField.SetValue(_propagationGraphic, new Graphic[0]);

            Assert.That(
                () => _propagationGraphic.CrossFadeColor(
                    Color.red,
                    duration: 0f,
                    ignoreTimeScale: true,
                    useAlpha: true),
                Throws.Nothing);
        }

        private Image CreateChildImage(string name, Transform? parent = null)
        {
            var child = new GameObject(name, typeof(RectTransform), typeof(Image));
            child.transform.SetParent(parent == null ? _root.transform : parent, false);
            return child.GetComponent<Image>();
        }
    }
}
