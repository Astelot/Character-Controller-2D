using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor {

    private PlayerController playerController;
    private AnimBool velocityFields;
    private Material material;

    private float y1;
    private float y2;
    private float a;
    private float b;

    private Vector2 c1, c2;

    private float c1x, c1y, c2x, c2y;

    private void OnEnable() {
        playerController = (PlayerController)target;
        velocityFields = new AnimBool(true);
        velocityFields.valueChanged.AddListener(Repaint);
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
    }

    public override void OnInspectorGUI() {

        playerController.acceleration = EditorGUILayout.Slider("Acceleration", playerController.acceleration, 0.1f, 10f);
        EditorGUILayout.Separator();

        velocityFields.target = EditorGUILayout.Foldout(velocityFields.target, "Velocity Constraints", true);

        if (EditorGUILayout.BeginFadeGroup(velocityFields.faded)) {
            EditorGUI.indentLevel++;
                y1 = EditorGUILayout.Slider("Forward", y1, 0f, 20f);
                y2 = EditorGUILayout.Slider("Backward", y2, -20f, 0f);
                a = EditorGUILayout.Slider("Left", a, 0f, 20f);
                b = EditorGUILayout.Slider("Right", b, 0f, 20f);
            c1x = EditorGUILayout.Slider("c1x", c1x, -20f, 20f);
            c1y = EditorGUILayout.Slider("c1y", c1y, -20f, 20f);
            c2x = EditorGUILayout.Slider("c2x", c2x, -20f, 20f);
            c2y = EditorGUILayout.Slider("c2y", c2y, -20f, 20f);
            c1 = new Vector2(c1x, c1y);
            c2 = new Vector2(c2x, c2y);
            playerController.velocityConstraint = new VelocityConstraint();
                playerController.velocityConstraint.line = playerController.velocityLine;
            EditorGUI.indentLevel--;

            // Draw velocity constraints graph.
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            Rect layoutRectangle = GUILayoutUtility.GetRect(10, 1000, 0, 220);

            if (Event.current.type == EventType.Repaint) {
                GUI.BeginClip(layoutRectangle);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);
                material.SetPass(0);

                DrawBackground(layoutRectangle);
                DrawCartesianSystem(layoutRectangle);
                BezierCurve curve = new BezierCurve(new Vector2(b,0), new Vector2(0,y1), c1, c1);
                DrawBezierCurve(curve, layoutRectangle);
                DrawLine(layoutRectangle);

                GL.PopMatrix();
                GUI.EndClip();
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFadeGroup();
    }

    private void DrawBackground(Rect layout) {
        GL.Begin(GL.QUADS);
        GL.Color(Color.black);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(layout.width, 0, 0);
        GL.Vertex3(layout.width, layout.height, 0);
        GL.Vertex3(0, layout.height, 0);
        GL.End();
    }

    private void DrawCartesianSystem(Rect layout) {
        GL.Begin(GL.LINES);

        // TODO: IMPLEMENT SINGLE SCALING FUNCTION FOR MULITPLE VELOCITY CONSTRAINTS!!!
        float height_scale = layout.height / (2 * Mathf.CeilToInt(a) + 2);
        float width_scale = layout.width / (Mathf.CeilToInt(y1) - Mathf.CeilToInt(y2) + 2);

        float scale_factor = (height_scale < width_scale) ? height_scale : width_scale;

        int min_cartX = (int)Mathf.Ceil(RectPointToCartesian(layout, new Vector2(0, 0), scale_factor).x);
        int max_cartX = (int)Mathf.Ceil(RectPointToCartesian(layout, new Vector2(layout.width, 0), scale_factor).x);

        int min_cartY = (int)Mathf.Ceil(RectPointToCartesian(layout, new Vector2(0, 0), scale_factor).y);
        int max_cartY = (int)Mathf.Ceil(RectPointToCartesian(layout, new Vector2(0, layout.height), scale_factor).y);

        for (int x = min_cartX; x < max_cartX; x++) {
            GL.Color(LineColor(x));

            Vector3 screenPoint = CartesianToRectPoint(layout, new Vector2(x, 0), scale_factor);

            GL.Vertex3(screenPoint.x, 0, 0);
            GL.Vertex3(screenPoint.x, layout.height, 0);
        }

        for (int y = min_cartY; y < max_cartY; y++) {
            GL.Color(LineColor(y));

            Vector3 screenPoint = CartesianToRectPoint(layout, new Vector2(0, y), scale_factor);

            GL.Vertex3(0, screenPoint.y, 0);
            GL.Vertex3(layout.width, screenPoint.y, 0);
        }
        GL.End();
    }

    private void DrawLine(Rect layout) {
        if (Application.isPlaying) {
            if (playerController.velocityConstraint.line.PointB != Vector2.zero) {

                float height_scale = layout.height / (2 * Mathf.CeilToInt(a) + 2);
                float width_scale = layout.width / (Mathf.CeilToInt(y1) - Mathf.CeilToInt(y2) + 2);

                float scale_factor = (height_scale < width_scale) ? height_scale : width_scale;

                GL.Begin(GL.LINES);
                GL.Color(Color.red);
                Vector2 line = CartesianToRectPoint(layout, playerController.velocityLine.PointB, scale_factor);
                Vector2 zeroLine = CartesianToRectPoint(layout, Vector2.zero, scale_factor);
                GL.Vertex3(zeroLine.x, zeroLine.y, 0);
                GL.Vertex3(line.x, line.y, 0);
                GL.End();
            }
        }
    }

    private void DrawBezierCurve(BezierCurve bezier, Rect layout) {
        float t = 0;
        int samples = 20;
        Vector2 v1, v2;
        v1 = bezier.CalculatePoint(t);

        float height_scale = layout.height / (2 * Mathf.CeilToInt(a) + 2);
        float width_scale = layout.width / (Mathf.CeilToInt(y1) - Mathf.CeilToInt(y2) + 2);

        float scale_factor = (height_scale < width_scale) ? height_scale : width_scale;

        GL.Begin(GL.LINE_STRIP);
        GL.Color(Color.yellow);
        for (int i = 1; i <= samples; i++) {
            t = i / (float)samples;
            v2 = bezier.CalculatePoint(t);

            Vector2 rect_v1 = CartesianToRectPoint(layout, v1, scale_factor);
            Vector2 rect_v2= CartesianToRectPoint(layout, v2, scale_factor);

            GL.Vertex3(rect_v1.x, rect_v1.y, 0);
            GL.Vertex3(rect_v2.x, rect_v2.y, 0);
            v1 = v2;
        }
        GL.End();
    }

    private Vector2 RectPointToCartesian(Rect layout, Vector2 rectPoint, float scaleFactor = 1) {
        float cartesian_x = (rectPoint.x - layout.width / 2) / scaleFactor;
        float cartesian_y = (rectPoint.y - layout.height / 2) / scaleFactor;

        return new Vector2(cartesian_x, cartesian_y);
    }

    private Vector2 CartesianToRectPoint(Rect layout, Vector2 cartesianPoint, float scaleFactor = 1) {
        float rect_x = scaleFactor * cartesianPoint.x + layout.width / 2;
        float rect_y = layout.height / 2 - scaleFactor * cartesianPoint.y;

        return new Vector2(rect_x, rect_y);
    }

    private Color LineColor(int line_n) {
        return (line_n % 10 == 0) ? Color.white : new Color(0.2f, 0.2f, 0.2f);
    }
}
