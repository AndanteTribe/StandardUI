#nullable enable

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StandardUI
{
    /// <summary>
    /// Supplies a shared white sprite when an Image has no sprite assigned.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class WhiteSprite : UIBehaviour
    {
        private Image? _image;
        private Image Image => _image == null ? _image = GetComponent<Image>() : _image;

        private static Sprite? s_whiteSprite;

        private static Sprite SharedSprite
        {
            get
            {
                if (s_whiteSprite == null)
                {
                    var texture = Texture2D.whiteTexture;
                    s_whiteSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    s_whiteSprite.hideFlags = HideFlags.HideAndDontSave;
                }

                return s_whiteSprite;
            }
        }

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();
            SetSpriteIfMissing();
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            var image = Image;
            if (image != null && s_whiteSprite != null && image.sprite == s_whiteSprite)
            {
                image.sprite = null;
            }

            base.OnDestroy();
        }

        private void SetSpriteIfMissing()
        {
            var image = Image;
            if (image.sprite == null)
            {
                image.sprite = SharedSprite;
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            SetSpriteIfMissing();
        }

        // Only the transient Sprite is owned here, never Texture2D.whiteTexture.
        [UnityEditor.InitializeOnLoadMethod]
        private static void RegisterSpriteCleanup()
        {
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += ClearWhiteSprite;
            UnityEditor.EditorApplication.quitting += ClearWhiteSprite;

            static void ClearWhiteSprite()
            {
                if (s_whiteSprite != null)
                {
                    DestroyImmediate(s_whiteSprite);
                    s_whiteSprite = null;
                }
            }
        }
#endif
    }
}