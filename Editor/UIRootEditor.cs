using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yans.UI;

public class UIRootEditor
{
    [MenuItem("GameObject/UI/UI Panel Root", false, 10)]
    static void CreateUIRoot(MenuCommand menuCommand)
    {
        // Create UIRoot GameObject
        GameObject uiRootGo = new GameObject("UIRoot");
        UIRoot uiRoot = uiRootGo.AddComponent<UIRoot>();
        Canvas canvas = uiRootGo.AddComponent<Canvas>();
        CanvasScaler canvasScaler = uiRootGo.AddComponent<CanvasScaler>();
        GraphicRaycaster graphicRaycaster = uiRootGo.AddComponent<GraphicRaycaster>();

        // Configure Canvas
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Configure CanvasScaler
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        canvasScaler.matchWidthOrHeight = 0.5f;

        // Create EventSystem GameObject
        GameObject eventSystemGo = new GameObject("EventSystem");
        eventSystemGo.AddComponent<EventSystem>();
        eventSystemGo.AddComponent<StandaloneInputModule>();
        eventSystemGo.transform.SetParent(uiRootGo.transform);

        // Create ScreenRoot GameObject
        GameObject screenRootGo = new GameObject("ScreenRoot");
        RectTransform screenRootRectTransform = screenRootGo.AddComponent<RectTransform>();
        screenRootGo.transform.SetParent(uiRootGo.transform);

        // Assign components to UIRoot fields using SerializedObject
        SerializedObject so = new SerializedObject(uiRoot);
        so.FindProperty("_canvas").objectReferenceValue = canvas;
        so.FindProperty("_canvasScaler").objectReferenceValue = canvasScaler;
        so.FindProperty("_graphicRaycaster").objectReferenceValue = graphicRaycaster;
        so.FindProperty("_screenRoot").objectReferenceValue = screenRootRectTransform;
        so.ApplyModifiedProperties();

        // Ensure it's placed correctly in the scene hierarchy (optional)
        GameObjectUtility.SetParentAndAlign(uiRootGo, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(uiRootGo, "Create " + uiRootGo.name);
        Selection.activeObject = uiRootGo;
    }
}
