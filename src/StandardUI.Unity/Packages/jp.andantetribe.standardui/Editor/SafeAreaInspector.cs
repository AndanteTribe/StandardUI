using UnityEditor;
using UnityEngine.UIElements;

namespace StandardUI.Editor
{
    [CustomEditor(typeof(SafeArea))]
    public class SafeAreaInspector : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var button = new Button()
            {
                text = "Adjust SafeArea"
            };
            button.RegisterCallback<ClickEvent, SafeArea>(static (_, safeArea) => safeArea.Adjust(), (SafeArea)target);
            root.Add(button);
            return root;
        }
    }
}