#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace StandardUI
{
    /// <summary>
    /// No-op graphic component.
    /// </summary>
    /// <remarks>
    /// Intended use: mask base, full-screen transparent uGUI overlay,
    /// or a transparent hit area for buttons.
    /// </remarks>
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyGraphic : Graphic
    {
        /// <inheritdoc/>
        public override void SetMaterialDirty()
        {
            // Intentionally no-op.
        }

        /// <inheritdoc/>
        public override void SetVerticesDirty()
        {
            // Intentionally no-op.
        }

        /// <inheritdoc/>
        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
    }
}