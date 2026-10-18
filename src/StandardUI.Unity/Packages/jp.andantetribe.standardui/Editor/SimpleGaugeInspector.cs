#nullable enable

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StandardUI.Editor
{
#pragma warning disable CS0618
    [CustomEditor(typeof(SimpleGauge))]
#pragma warning restore CS0618
    public class SimpleGaugeInspector : UnityEditor.Editor
    {
        /// <inheritdoc/>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new PropertyField(serializedObject.FindProperty("_mode")));
            root.Add(new PropertyField(serializedObject.FindProperty("_value")));
            return root;
        }
    }
}