#nullable enable

using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace StandardUI.Editor
{
    // Exact Image targets only: custom inspectors for Image subclasses are not replaced.
    [CustomEditor(typeof(Image))]
    [CanEditMultipleObjects]
    public sealed class WhiteSpriteImageInspector : ImageEditor
    {
        private SerializedProperty _type = null!;
        private SerializedProperty _useSpriteMesh = null!;
        private SerializedProperty _preserveAspect = null!;

        private bool IsSourceImageLocked
        {
            get
            {
                foreach (var item in targets)
                {
                    var helper = ((Image)item).GetComponent<WhiteSprite>();
                    if (helper != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _type = serializedObject.FindProperty("m_Type");
            _useSpriteMesh = serializedObject.FindProperty("m_UseSpriteMesh");
            _preserveAspect = serializedObject.FindProperty("m_PreserveAspect");
        }

        public override void OnInspectorGUI()
        {
            if (!IsSourceImageLocked)
            {
                base.OnInspectorGUI();
                return;
            }

            // ImageEditor.SpriteGUI is not virtual. Reuse its protected controls for
            // the locked case, and delegate the entire ordinary case to the stock editor.
            serializedObject.Update();
            using (new EditorGUI.DisabledScope(true))
            {
                SpriteGUI();
            }

            EditorGUILayout.HelpBox(
                "Source Image is controlled by WhiteSprite. Remove the helper to edit it. In a mixed selection, the field is locked for all selected Images.",
                MessageType.Info);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            TypeGUI();

            var type = (Image.Type)_type.enumValueIndex;
            var showNativeSize = !_type.hasMultipleDifferentValues && type is Image.Type.Simple or Image.Type.Filled;
            if (showNativeSize)
            {
                EditorGUI.indentLevel++;
                if (type == Image.Type.Simple)
                {
                    EditorGUILayout.PropertyField(_useSpriteMesh);
                }

                EditorGUILayout.PropertyField(_preserveAspect);
                EditorGUI.indentLevel--;
            }

            SetShowNativeSize(showNativeSize, true);
            NativeSizeButtonGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}