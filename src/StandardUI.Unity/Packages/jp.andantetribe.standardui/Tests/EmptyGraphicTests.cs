#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace StandardUI.Tests
{
    public sealed class EmptyGraphicTests
    {
        private GameObject _root = null!;
        private Canvas _canvas = null!;
        private GraphicRaycaster _raycaster = null!;
        private EventSystem _eventSystem = null!;
        private EmptyGraphic _emptyGraphic = null!;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));

            _canvas = _root.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _raycaster = _root.GetComponent<GraphicRaycaster>();

            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            eventSystemObject.transform.SetParent(_root.transform, false);
            _eventSystem = eventSystemObject.GetComponent<EventSystem>();

            var emptyGraphicObject = new GameObject("EmptyGraphic", typeof(RectTransform), typeof(EmptyGraphic));
            emptyGraphicObject.transform.SetParent(_canvas.transform, false);

            _emptyGraphic = emptyGraphicObject.GetComponent<EmptyGraphic>();
            _emptyGraphic.raycastTarget = true;
            ConfigureCenteredRect(_emptyGraphic.rectTransform, new Vector2(100f, 100f));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void SetMaterialDirtyDoesNotInvokeRegisteredCallback()
        {
            var wasInvoked = false;
            _emptyGraphic.RegisterDirtyMaterialCallback(() => wasInvoked = true);

            _emptyGraphic.SetMaterialDirty();

            Assert.That(wasInvoked, Is.False);
        }

        [Test]
        public void SetVerticesDirtyDoesNotInvokeRegisteredCallback()
        {
            var wasInvoked = false;
            _emptyGraphic.RegisterDirtyVerticesCallback(() => wasInvoked = true);

            _emptyGraphic.SetVerticesDirty();

            Assert.That(wasInvoked, Is.False);
        }

        [Test]
        public void OnPopulateMeshClearsGeometry()
        {
            using (var vertexHelper = new VertexHelper())
            {
                vertexHelper.AddVert(Vector3.zero, Color.white, Vector2.zero);
                vertexHelper.AddVert(new Vector3(10f, 0f, 0f), Color.white, Vector2.right);
                vertexHelper.AddVert(new Vector3(10f, 10f, 0f), Color.white, Vector2.one);
                vertexHelper.AddTriangle(0, 1, 2);

                Assert.That(vertexHelper.currentVertCount, Is.EqualTo(3));
                Assert.That(vertexHelper.currentIndexCount, Is.EqualTo(3));

                var method = typeof(EmptyGraphic).GetMethod(
                    "OnPopulateMesh",
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(VertexHelper) },
                    null);

                Assert.That(method, Is.Not.Null);
                method!.Invoke(_emptyGraphic, new object[] { vertexHelper });

                Assert.That(vertexHelper.currentVertCount, Is.Zero);
                Assert.That(vertexHelper.currentIndexCount, Is.Zero);
            }
        }

        [UnityTest]
        public IEnumerator RaycastIncludesEmptyGraphicWhenRaycastTargetIsTrue()
        {
            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.True);
        }

        [UnityTest]
        public IEnumerator RaycastExcludesEmptyGraphicWhenRaycastTargetIsFalse()
        {
            _emptyGraphic.raycastTarget = false;

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.False);
        }

        [UnityTest]
        public IEnumerator RaycastRespectsMask()
        {
            var maskObject = new GameObject("Mask", typeof(RectTransform), typeof(Image), typeof(Mask));
            maskObject.transform.SetParent(_canvas.transform, false);
            ConfigureCenteredRect(maskObject.GetComponent<RectTransform>(), new Vector2(50f, 50f));

            _emptyGraphic.transform.SetParent(maskObject.transform, false);
            ConfigureCenteredRect(_emptyGraphic.rectTransform, new Vector2(20f, 20f));
            _emptyGraphic.rectTransform.anchoredPosition = new Vector2(100f, 0f);

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.False);

            _emptyGraphic.rectTransform.anchoredPosition = Vector2.zero;

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.True);
        }

        [UnityTest]
        public IEnumerator RaycastRespectsRectMask2D()
        {
            var maskObject = new GameObject("RectMask", typeof(RectTransform), typeof(RectMask2D));
            maskObject.transform.SetParent(_canvas.transform, false);
            ConfigureCenteredRect(maskObject.GetComponent<RectTransform>(), new Vector2(50f, 50f));

            _emptyGraphic.transform.SetParent(maskObject.transform, false);
            ConfigureCenteredRect(_emptyGraphic.rectTransform, new Vector2(20f, 20f));
            _emptyGraphic.rectTransform.anchoredPosition = new Vector2(100f, 0f);

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.False);

            _emptyGraphic.rectTransform.anchoredPosition = Vector2.zero;

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.True);
        }

        [UnityTest]
        public IEnumerator RaycastRespectsCanvasGroupBlocksRaycasts()
        {
            var canvasGroup = _emptyGraphic.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.True);

            canvasGroup.blocksRaycasts = false;

            yield return null;

            Assert.That(ContainsEmptyGraphic(RaycastAtEmptyGraphic()), Is.False);
        }

        private static void ConfigureCenteredRect(RectTransform rectTransform, Vector2 size)
        {
            var center = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = center;
            rectTransform.anchorMax = center;
            rectTransform.pivot = center;
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private List<RaycastResult> RaycastAtEmptyGraphic()
        {
            Canvas.ForceUpdateCanvases();

            var pointerData = new PointerEventData(_eventSystem)
            {
                position = RectTransformUtility.WorldToScreenPoint(
                    null,
                    _emptyGraphic.rectTransform.position)
            };
            var results = new List<RaycastResult>();
            _raycaster.Raycast(pointerData, results);
            return results;
        }

        private bool ContainsEmptyGraphic(List<RaycastResult> results)
        {
            foreach (var result in results)
            {
                if (result.gameObject == _emptyGraphic.gameObject)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
