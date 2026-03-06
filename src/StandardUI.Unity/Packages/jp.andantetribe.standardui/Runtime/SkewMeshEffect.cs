#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace StandardUI
{
    /// <summary>
    /// Effect that skews a uGUI mesh.
    /// </summary>
    /// <remarks>
    /// Attach this to an <see cref="Image"/> object to create visuals such as
    /// diagonally clipped gauge fills.
    /// </remarks>
    [RequireComponent(typeof(Graphic))]
    public class SkewMeshEffect : BaseMeshEffect
    {
        [SerializeField]
        private Vector2 _skew = new Vector2(0.5f, 0f);

        /// <inheritdoc />
        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive())
            {
                return;
            }

            var vertex = new UIVertex();
            for (var i = 0; i < helper.currentVertCount; i++)
            {
                helper.PopulateUIVertex(ref vertex, i);
                var pos = vertex.position;
                pos.x += _skew.x * pos.y;
                pos.y += _skew.y * pos.x;
                vertex.position = pos;
                helper.SetUIVertex(vertex, i);
            }
        }
    }
}
