using UnityEngine;

namespace DebugUtils
{
    public static class DebugDraw
    {
        private static readonly LineDrawer _lineDrawer;
        private static readonly Material _onTopMaterial;
        private static readonly Material _defaultMaterial;

        static DebugDraw()
        {
            _lineDrawer = new LineDrawer();

            _defaultMaterial = new Material(Shader.Find("Sprites/Default"));

            _onTopMaterial = new Material(Shader.Find("Battlehub/RTGizmos/Handles"));
            _onTopMaterial.color = Color.white;
            _onTopMaterial.SetFloat("_Offset", 1f);
            _onTopMaterial.SetFloat("_MinAlpha", 1f);
        }

        public static bool Enabled { get; set; }
        public static bool DrawOnTop { get; set; } = true;

        public static void Clear() => _lineDrawer?.Clear();
        public static void Reset() => _lineDrawer?.Reset();
        public static void Draw() => _lineDrawer?.UpdateAndDraw(DrawOnTop ? _onTopMaterial : _defaultMaterial);

        public static void DrawLine(Vector3 start, Vector3 stop, Color color)
        { if (Enabled) _lineDrawer.PushLine(start, stop, color); }
        public static void DrawLine(Vector3 start, Vector3 stop, Color colorStart, Color colorEnd)
        { if (Enabled) _lineDrawer.PushLine(start, stop, colorStart, colorEnd); }

        public static void DrawRay(Vector3 position, Vector3 normal, float distance, Color color)
        { if (Enabled) _lineDrawer.PushRay(position, normal, distance, color); }

        public static void DrawPoint(Vector3 position, Color color, float size = 0.003f)
        { if (Enabled) _lineDrawer.PushPoint(position, color, size); }

        public static void DrawTransform(Vector3 position, Vector3 up, Vector3 right, Vector3 forward, float size = 1)
        { if (Enabled) _lineDrawer.PushTransform(position, up, right, forward, size); }
        public static void DrawTransform(Transform transform, Vector3 position, float size = 1)
        { if (Enabled) _lineDrawer.PushTransform(transform, position, size); }
        public static void DrawTransform(Transform transform, float size = 1)
        { if (Enabled) _lineDrawer.PushTransform(transform, size); }

        public static void DrawBox(Vector3 position, Quaternion rotation, Vector3 extents, Color color)
        { if (Enabled) _lineDrawer.PushBox(position, rotation, extents, color); }
        public static void DrawBox(Vector3 position, Vector3 extents, Color color)
        { if (Enabled) _lineDrawer.PushBox(position, extents, color); }
        public static void DrawBox(Bounds bounds, Color color)
        { if (Enabled) _lineDrawer.PushBox(bounds, color); }
        public static void DrawBox(Bounds bounds, Quaternion rotation, Color color)
        { if (Enabled) _lineDrawer.PushBox(bounds, rotation, color); }
        public static void DrawLocalBox(Bounds bounds, Vector3 position, Quaternion rotation, Color color)
        { if (Enabled) _lineDrawer.PushLocalBox(bounds, position, rotation, color); }

        public static void DrawArc(Vector3 position, Vector3 normal, Vector3 tangent, float radius, float fromAngle, float toAngle, Color color, int segments = 10)
        { if (Enabled) _lineDrawer.PushArc(position, normal, tangent, radius, fromAngle, toAngle, color, segments); }

        public static void DrawEllipse(Vector3 position, Quaternion rotation, Color color, float radiusX, float radiusZ, int segments = 20)
        { if (Enabled) _lineDrawer.PushEllipse(position, rotation, color, radiusX, radiusZ, segments); }
        public static void DrawEllipse(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float radiusX, float radiusZ, int segments = 20)
        { if (Enabled) _lineDrawer.PushEllipse(position, normal, tangent, color, radiusX, radiusZ, segments); }

        public static void DrawCircle(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float radius, int segments = 20)
        { if (Enabled) _lineDrawer.PushCircle(position, normal, tangent, color, radius, segments); }
        public static void DrawCircle(Vector3 position, Quaternion rotation, Color color, float radius, int segments = 20)
        { if (Enabled) _lineDrawer.PushCircle(position, rotation, color, radius, segments); }

        public static void DrawRectangle(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float width, float height)
        { if (Enabled) _lineDrawer.PushRectangle(position, normal, tangent, color, width, height); }
        public static void DrawRectangle(Vector3 position, Quaternion rotation, Color color, float width, float height)
        { if (Enabled) _lineDrawer.PushRectangle(position, rotation, color, width, height); }

        public static void DrawSquare(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float size)
        { if (Enabled) _lineDrawer.PushSquare(position, normal, tangent, color, size); }
        public static void DrawSquare(Vector3 position, Quaternion rotation, Color color, float size)
        { if (Enabled) _lineDrawer.PushSquare(position, rotation, color, size); }
    }
}