#nullable enable

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StandardUI.Editor
{
    public static class UICreationMenu
    {
        private const string MenuRoot = "GameObject/UI (Canvas)/";

        [MenuItem(MenuRoot + "Simple Gauge", false, 2050)]
        public static void CreateSimpleGauge(MenuCommand command)
        {
            Undo.IncrementCurrentGroup();
            var group = Undo.GetCurrentGroup();
            const string undoName = "Create Simple Gauge";
            try
            {
                var parent = command.context as GameObject;
                if (parent == null)
                {
                    parent = Selection.activeGameObject;
                }

                // Use Unity's own creation commands for Canvas/EventSystem/stage handling.
                if (parent != null && parent.GetComponentInParent<Canvas>(true) == null)
                {
                    ExecuteMenu(nameof(Canvas));
                    var canvas = Selection.activeGameObject;
                    if (canvas == null || canvas.GetComponent<Canvas>() == null)
                    {
                        throw new InvalidOperationException("Unity did not create a Canvas.");
                    }

                    Undo.SetTransformParent(canvas.transform, parent.transform, undoName);
                    GameObjectUtility.SetParentAndAlign(canvas, parent);
                    parent = canvas;
                }

                Selection.activeGameObject = parent;
                ExecuteMenu(nameof(Image));
                var element = Selection.activeGameObject;
                if (element == null || !element.TryGetComponent<Image>(out var image))
                {
                    throw new InvalidOperationException("Unity did not create an Image.");
                }

                if (parent != null)
                {
                    Undo.SetTransformParent(element.transform, parent.transform, undoName);
                    GameObjectUtility.SetParentAndAlign(element, parent);
                }

                Undo.RecordObject(element, undoName);
                element.name = GameObjectUtility.GetUniqueNameForSibling(element.transform.parent, "Simple Gauge");
                var rect = (RectTransform)element.transform;
                Undo.RecordObject(rect, undoName);
                rect.sizeDelta = new Vector2(200, 20);
                rect.anchoredPosition = Vector2.zero;

                Undo.RecordObject(image, undoName);
                image.sprite = null;
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = (int)Image.OriginHorizontal.Left;
                image.fillAmount = 0.5f;
                image.raycastTarget = false;
                Undo.AddComponent<WhiteSprite>(element);
                Selection.activeGameObject = element;
            }
            finally
            {
                Undo.SetCurrentGroupName(undoName);
                Undo.CollapseUndoOperations(group);
            }
        }

        private static void ExecuteMenu(string item)
        {
            if (!EditorApplication.ExecuteMenuItem(MenuRoot + item))
            {
                throw new InvalidOperationException("Unity UI menu is unavailable: " + MenuRoot + item);
            }
        }
    }
}