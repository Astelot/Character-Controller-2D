using UnityEngine;
using UnityEditor;
using System;

public class ConstraintWindow : EditorWindow {
    #region Window Fields
    private const float MinimumSizeRatio = 0.2f;
    private const float MaximumSizeRatio = 0.75f;

    private Rect propetiesPanel;
    private Rect graphPanel;
    private Rect divisionPanel;

    private float panelSizeRatio = 0.25f;
    private float divisionPanelWidth = 2f;

    private Color graphBackgroundColor = new Color(0.39f, 0.39f, 0.39f);

    private bool isResizing;

    private GUIStyle divisonStyle;
    private GUIStyle graphStyle;
    private GUIStyle labelStyle;
    #endregion;

    #region Bezier fields
    [Range(0, 100)]private float positiveX;
    [Range(0, 100)]private float positiveY;
    [Range(-100, 0)]private float negativeX;
    [Range(-100, 0)]private float negativeY;
    #endregion;

    [MenuItem("Window/Resizable Panels")]
    public static void OpenWindow() {
        ConstraintWindow window = CreateInstance<ConstraintWindow>();
        window.titleContent = new GUIContent("Constraint Editor");
        window.ShowUtility();
    }

    private void OnGUI() {
        labelStyle = GUI.skin.GetStyle("label");
        labelStyle.alignment = TextAnchor.MiddleCenter;

        DrawPropertiesPanel();
        DrawGraphPanel();
        DrawDivisionPanel();

        ProcessEvents(Event.current);
        if (GUI.changed) {
            Repaint();
        }
    }

    private void OnEnable() {
        divisonStyle = new GUIStyle();
        graphStyle = new GUIStyle();

        divisonStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        graphStyle.normal.background = CreateColorTexture(graphBackgroundColor);
    }

    private void OnLostFocus() {
        Close();
    }

    private void DrawDivisionPanel() {
        divisionPanel = new Rect(position.width * panelSizeRatio - (divisionPanelWidth / 2), 0, divisionPanelWidth, position.height);

        GUILayout.BeginArea(divisionPanel, divisonStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(divisionPanel, MouseCursor.ResizeHorizontal);
    }

    private void DrawPropertiesPanel() {
        propetiesPanel = new Rect(0, 0, position.width * panelSizeRatio - (divisionPanelWidth / 2), position.height);
        EditorGUIUtility.labelWidth = 75f;

        GUILayout.BeginArea(propetiesPanel);
        EditorGUI.LabelField(new Rect((int)(propetiesPanel.width / 2 - 50), 0, 100, 20), "Properties", labelStyle);
        positiveX = EditorGUI.FloatField(new Rect(propetiesPanel.min.x + 10, propetiesPanel.min.y + 30, propetiesPanel.width - 15, 20), "Positive X", positiveX);
        positiveY = EditorGUI.FloatField(new Rect(propetiesPanel.min.x + 10, propetiesPanel.min.y + 52, propetiesPanel.width - 15, 20), "Positive Y", positiveY);
        negativeX = EditorGUI.FloatField(new Rect(propetiesPanel.min.x + 10, propetiesPanel.min.y + 74, propetiesPanel.width - 15, 20), "Negative X", negativeX);
        negativeY = EditorGUI.FloatField(new Rect(propetiesPanel.min.x + 10, propetiesPanel.min.y + 96, propetiesPanel.width - 15, 20), "Negative Y", negativeY);
        GUILayout.EndArea();
    }

    private void DrawGraphPanel() {
        graphPanel = new Rect(position.width * panelSizeRatio + (divisionPanelWidth / 2), 0, position.width * ( 1 - panelSizeRatio) - (divisionPanelWidth / 2), position.height);

        GUILayout.BeginArea(graphPanel, graphStyle);
        EditorGUI.LabelField(new Rect((int)(graphPanel.width / 2 - 50), 0, 100, 20), "Curve graph", labelStyle);
        GUILayout.EndArea();
    }

    private void ProcessEvents(Event e)
    {
        switch (e.rawType)
        {
            case EventType.MouseDown:
                if (e.button == 0 && divisionPanel.Contains(e.mousePosition))
                {
                    isResizing = true;
                }
                break;
            case EventType.MouseUp:
                isResizing = false;
                break;
        }
        Resize(e);
    }

    private void Resize(Event e)
    {
        if (isResizing) {
            panelSizeRatio = Mathf.Clamp(e.mousePosition.x / position.width, MinimumSizeRatio, MaximumSizeRatio);
            Repaint();
        }
    }

    // TODO: Find new home for CreateColorTexture(), rename and maybe expand upon.
    private Texture2D CreateColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(1, 1, color);
        /*
        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                texture.SetPixel(x, y, color);
            }
        }*/
        texture.Apply();
        return texture;
    }
}