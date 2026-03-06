#nullable enable

using System;
using UnityEngine;
using UnityEngine.UI;

namespace StandardUI
{
    /// <summary>
    /// Forwards <see cref="Selectable"/> color transitions to child <see cref="Graphic"/> components.
    /// </summary>
    /// <remarks>
    /// Use this when a parent selectable controls the visual state of multiple child graphics.
    /// </remarks>
    public sealed class PropagationGraphic : EmptyGraphic
    {
        [SerializeField, Tooltip("Child graphics to propagate color transitions to.")]
        private Graphic[] _graphics = Array.Empty<Graphic>();

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            var graphics = GetComponentsInChildren<Graphic>(includeInactive: true);
            var selfIndex = Array.IndexOf(graphics, this);
            graphics.AsSpan(selfIndex + 1).CopyTo(graphics.AsSpan(selfIndex));
            Array.Resize(ref graphics, graphics.Length - 1);
            _graphics = graphics;
        }
#endif

        /// <inheritdoc/>
        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            foreach (var childGraphic in _graphics.AsSpan())
            {
                if (childGraphic.isActiveAndEnabled)
                {
                    childGraphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
                }
            }
        }
    }
}