//This is a Custom Gizmos Library that features more complex gizmos shapes that unity's default gizmos library  
//made By Ali Essam

using UnityEngine;
using UnityEditor;



public static class AdvancedGizmos
{
    private const int DEFAULT_SEGMENTS = 32;
    private const float TEXT_OFFSET = 0.1f;

    #if UNITY_EDITOR
    private static GUIStyle _gizmoTextStyle;
    #endif

    public static void DrawCylinder(Vector3 position, Quaternion rotation, float height, float radius, Color color, int SegmentsSize = DEFAULT_SEGMENTS)
    {
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        
        try
        {
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            
            
            Draw2DCircle(Vector3.up * height * 0.5f, radius);
            Draw2DCircle(Vector3.down * height * 0.5f, radius);

           
            Vector3[] topPoints = GetCirclePoints(Vector3.up * height * 0.5f, radius, DEFAULT_SEGMENTS);
            Vector3[] bottomPoints = GetCirclePoints(Vector3.down * height * 0.5f, radius, DEFAULT_SEGMENTS);
            
            for(int i = 0; i < DEFAULT_SEGMENTS; i++)
            {
                Gizmos.DrawLine(topPoints[i], bottomPoints[i]);
                Gizmos.DrawLine(topPoints[i], topPoints[(i+1) % DEFAULT_SEGMENTS]);
                Gizmos.DrawLine(bottomPoints[i], bottomPoints[(i+1) % DEFAULT_SEGMENTS]);
            }
        }
        finally
        {
            Gizmos.matrix = oldMatrix;
        }
    }

    public static void DrawCapsule(Vector3 position, Quaternion rotation, float height, float radius, Color color, int arcSegments = 16, int horizontalArcs = 8)
    {
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;

        try
        {
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            float cylinderHeight = Mathf.Max(height - radius * 2, 0);
            Vector3 top = Vector3.up * (cylinderHeight * 0.5f + radius);
            Vector3 bottom = Vector3.down * (cylinderHeight * 0.5f + radius);

            
            DrawHalfSphere(top, radius, false, arcSegments, horizontalArcs);

            
            DrawHalfSphere(bottom, radius, true, arcSegments, horizontalArcs);

            
            Vector3[] topPoints = GetCirclePoints(top, radius, horizontalArcs);
            Vector3[] bottomPoints = GetCirclePoints(bottom, radius, horizontalArcs);

            for (int i = 0; i < horizontalArcs; i++)
            {
                Gizmos.DrawLine(topPoints[i], bottomPoints[i]);
            }
        }
        finally
        {
            Gizmos.matrix = oldMatrix;
        }
    }

    public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color)
    {
        if (mesh == null) return;

        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }

        Gizmos.matrix = oldMatrix;
    }
 
    public static void DrawHalfSphere(Vector3 center, float radius, bool inverted, int arcSegments, int horizontalArcs)
    {
        Vector3 axis = inverted ? Vector3.down : Vector3.up;

        
        Vector3[] basePoints = GetCirclePoints(center, radius, arcSegments);
        for (int i = 0; i < arcSegments; i++)
        {
            Gizmos.DrawLine(basePoints[i], basePoints[(i + 1) % arcSegments]);
        }

        // vertical arcs (controlled by `horizontalArcs`)
        for (int arcIndex = 0; arcIndex < horizontalArcs; arcIndex++)
        {
            float angleOffset = (Mathf.PI * 2 / horizontalArcs) * arcIndex;
            Vector3[] arcPoints = new Vector3[arcSegments / 2];

            for (int i = 0; i < arcSegments / 2; i++)
            {
                float angle = Mathf.PI * i / (arcSegments / 2 - 1);
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius * (inverted ? -1 : 1);

                Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angleOffset, axis);
                arcPoints[i] = center + rotation * new Vector3(x, y, 0);

                if (i > 0)
                {
                    Gizmos.DrawLine(arcPoints[i - 1], arcPoints[i]);
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    public static void DrawIsometricText(Vector3 position, string text, Color color,Font Font ,FontStyle FontStyle= FontStyle.Bold, int fontSize =12)
    {
        if(_gizmoTextStyle == null)
        {
            _gizmoTextStyle = new GUIStyle();
            _gizmoTextStyle.normal.textColor = color;
            _gizmoTextStyle.alignment = TextAnchor.MiddleCenter;
            _gizmoTextStyle.fontSize = fontSize;
            _gizmoTextStyle.fontStyle = FontStyle;
            
            if(Font==null)
                _gizmoTextStyle.font= new Font("Arial");
            
            else
                _gizmoTextStyle.font = Font;
            
        }

        SceneView sceneView = SceneView.currentDrawingSceneView;
        if(sceneView == null || sceneView.camera == null) return;

        Vector3 screenPos = sceneView.camera.WorldToScreenPoint(position);
        if(screenPos.z < 0) return; // Behind camera

        Vector3 viewDirection = sceneView.camera.transform.rotation * Vector3.forward;
        Quaternion rotation = Quaternion.LookRotation(viewDirection);

        Handles.Label(
            position,
            text,
            _gizmoTextStyle
        );
    }
    #endif

    #if UNITY_EDITOR
    public static void DrawArrow(Vector3 Position,Vector3 Direction,Color arrowColor,int arrowSize)
    {
        Handles.color = arrowColor;
        Handles.ArrowHandleCap(0, Position, Quaternion.LookRotation(Direction), arrowSize, EventType.Repaint);
    }
    #endif

    public static void DrawFullSphere(Vector3 center, float radius, Color color, bool inverted = false)
    {
        Gizmos.color = color;
        DrawHalfSphere(center,radius,inverted,50,50);
        DrawHalfSphere(center,radius,!inverted,50,50);
        
        
        // int segments = Mathf.Max(DEFAULT_SEGMENTS / 2, 4);
        // Vector3[] points = new Vector3[segments + 1];
        //
        // for(int i = 0; i <= segments; i++)
        // {
        //     float angle = (inverted ? Mathf.PI : 0) + (Mathf.PI / segments) * i;
        //     float x = Mathf.Cos(angle) * radius;
        //     float z = Mathf.Sin(angle) * radius;
        //     
        //     points[i] = center + new Vector3(x, 0, z);
        //     
        //     if(i > 0)
        //     {
        //         Gizmos.DrawLine(points[i-1], points[i]);
        //         Gizmos.DrawLine(points[i], center + Vector3.up * (inverted ? -radius : radius));
        //     }
        // }
    }
    

    public static void Draw2DCircle(Vector3 center, float radius)
    {
        Vector3[] points = GetCirclePoints(center, radius, DEFAULT_SEGMENTS);
        
        for(int i = 0; i < DEFAULT_SEGMENTS; i++)
        {
            Gizmos.DrawLine(points[i], points[(i+1) % DEFAULT_SEGMENTS]);
        }
    }

    public static void DrawPolygon(Vector3 center, float radius, int sides, Color color)
    {
        if (sides < 3) return; // A polygon needs at least 3 sides

        Gizmos.color = color;
        Vector3[] points = new Vector3[sides];

        for (int i = 0; i < sides; i++)
        {
         float angle = (Mathf.PI * 2 / sides) * i;
         float x = Mathf.Cos(angle) * radius;
         float z = Mathf.Sin(angle) * radius;
         points[i] = center + new Vector3(x, 0, z);
        }

        for (int i = 0; i < sides; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % sides]);
        }
    }   

    public static void Draw2DTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, a);
    }


    public static void DrawRegularTriangle3D(Vector3 center, Vector3 normal, float size, Color color)
    {
        Gizmos.color = color;

        // creates base vectors in the plane
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f) tangent = Vector3.Cross(normal, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent);

        Vector3[] points = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            float angle = (Mathf.PI * 2 / 3) * i;
            points[i] = center + (Mathf.Cos(angle) * tangent + Mathf.Sin(angle) * bitangent) * size;
        }

        Draw2DTriangle(points[0], points[1], points[2], color);
    }

    internal static Vector3[] GetCirclePoints(Vector3 center, float radius, int segments)
    {
        Vector3[] points = new Vector3[segments];
        for(int i = 0; i < segments; i++)
        {
            float angle = (Mathf.PI * 2 / segments) * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            points[i] = center + new Vector3(x, 0, z);
        }
        return points;
    }
}