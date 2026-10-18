#nullable enable

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
#if UGUI_2_6_OR_GREATER
    [System.Obsolete("Use UnityEngine.UI.SafeArea instead (available in com.unity.ugui 2.6.0 or newer).")]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private void OnEnable() => Adjust();

#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
