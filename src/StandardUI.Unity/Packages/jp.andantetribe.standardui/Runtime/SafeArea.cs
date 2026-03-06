#nullable enable

using System;
using UnityEngine;
#if UNITY_EDITOR
using Screen = UnityEngine.Device.Screen;
#else
using Screen = UnityEngine.Screen;
#endif

namespace StandardUI
{
    /// <summary>
    /// Adjusts a RectTransform to match the device safe area.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private void OnEnable() => Adjust();

#if UNITY_EDITOR
        private void Reset() => Adjust();
#endif

        /// <summary>
        /// Applies the current safe area to this RectTransform's anchors.
        /// </summary>
        public void Adjust()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);

            var rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.offsetMin = new Vector2(0f, 0f);
                rectTransform.offsetMax = new Vector2(0f, 0f);
                rectTransform.anchorMin = safeArea.min / screenSize;
                rectTransform.anchorMax = safeArea.max / screenSize;
            }
        }
    }
}