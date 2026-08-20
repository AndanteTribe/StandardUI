#nullable enable

using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace StandardUI.Tests
{
    [TestFixture]
    public sealed class SkewMeshEffectTests
    {
        private GameObject _gameObject = null!;
        private SkewMeshEffect _effect = null!;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(
                "SkewMeshEffectTest",
                typeof(RectTransform),
                typeof(Image),
                typeof(SkewMeshEffect));
            _effect = _gameObject.GetComponent<SkewMeshEffect>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void ModifyMeshSkewsEveryVertexAndPreservesOtherData()
        {
            SetSkew(new Vector2(0.5f, -0.25f));

            var input = CreateInputVertices();
            using (var helper = CreateTriangle(input))
            {
                _effect.ModifyMesh(helper);

                Assert.That(helper.currentVertCount, Is.EqualTo(3));
                Assert.That(helper.currentIndexCount, Is.EqualTo(3));

                AssertVertex(
                    helper,
                    0,
                    new Vector3(4f, 3f, 7f),
                    input[0]);
                AssertVertex(
                    helper,
                    1,
                    new Vector3(-2f, 2.5f, -1f),
                    input[1]);
                AssertVertex(
                    helper,
                    2,
                    new Vector3(-2f, -5.5f, 0.5f),
                    input[2]);
            }
        }

        [Test]
        public void ModifyMeshLeavesVerticesUnchangedWhenInactive()
        {
            SetSkew(new Vector2(3f, -2f));
            _effect.enabled = false;

            var input = CreateInputVertices();
            using (var helper = CreateTriangle(input))
            {
                _effect.ModifyMesh(helper);

                Assert.That(helper.currentVertCount, Is.EqualTo(3));
                Assert.That(helper.currentIndexCount, Is.EqualTo(3));

                for (var i = 0; i < input.Length; i++)
                {
                    AssertVertex(helper, i, input[i].position, input[i]);
                }
            }
        }

        private void SetSkew(Vector2 skew)
        {
            var field = typeof(SkewMeshEffect).GetField(
                "_skew",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field!.SetValue(_effect, skew);
        }

        private static UIVertex[] CreateInputVertices()
        {
            return new[]
            {
                CreateVertex(
                    new Vector3(2f, 4f, 7f),
                    new Color32(10, 20, 30, 40),
                    new Vector4(0.125f, 0.25f, 0.5f, 1f)),
                CreateVertex(
                    new Vector3(-3f, 2f, -1f),
                    new Color32(50, 60, 70, 80),
                    new Vector4(1f, 0.5f, 0.25f, 0.125f)),
                CreateVertex(
                    new Vector3(1f, -6f, 0.5f),
                    new Color32(90, 100, 110, 120),
                    new Vector4(-1f, -0.5f, 2f, 4f))
            };
        }

        private static UIVertex CreateVertex(
            Vector3 position,
            Color32 color,
            Vector4 uv0)
        {
            return new UIVertex
            {
                position = position,
                color = color,
                uv0 = uv0
            };
        }

        private static VertexHelper CreateTriangle(UIVertex[] vertices)
        {
            var helper = new VertexHelper();
            foreach (var vertex in vertices)
            {
                helper.AddVert(vertex);
            }

            helper.AddTriangle(0, 1, 2);
            return helper;
        }

        private static void AssertVertex(
            VertexHelper helper,
            int index,
            Vector3 expectedPosition,
            UIVertex expectedSource)
        {
            var actual = new UIVertex();
            helper.PopulateUIVertex(ref actual, index);

            Assert.That(actual.position, Is.EqualTo(expectedPosition), $"vertex {index} position");
            Assert.That(actual.color, Is.EqualTo(expectedSource.color), $"vertex {index} color");
            Assert.That(actual.uv0, Is.EqualTo(expectedSource.uv0), $"vertex {index} uv0");
        }
    }
}
