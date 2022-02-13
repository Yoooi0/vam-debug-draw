using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity.Infix;

namespace DebugUtils
{
    public class LineDrawer
    {
        private readonly Mesh _mesh;
        private readonly List<Vector3> _vertices;
        private readonly List<Color> _colors;

        public LineDrawer()
        {
            _mesh = new Mesh();
            _vertices = new List<Vector3>();
            _colors = new List<Color>();
        }

        public void PushLine(Vector3 start, Vector3 stop, Color color)
        {
            _vertices.Add(start);
            _vertices.Add(stop);
            _colors.Add(color);
            _colors.Add(color);
        }

        public void PushLine(Vector3 start, Vector3 stop, Color colorStart, Color colorEnd)
        {
            _vertices.Add(start);
            _vertices.Add(stop);
            _colors.Add(colorStart);
            _colors.Add(colorEnd);
        }

        public void PushRay(Vector3 position, Vector3 normal, float distance, Color color) => PushLine(position, position + normal * distance, color);

        public void PushPoint(Vector3 position, Color color, float size = 0.003f)
        {
            var scale = Mathf.Clamp(Vector3.Distance(position, Camera.main.transform.position), 0, 5);
            PushCircle(position, Camera.main.transform.forward, Camera.main.transform.right, color, size * (1 + scale), 10);
        }

        public void PushTransform(Vector3 position, Vector3 up, Vector3 right, Vector3 forward, float size = 1)
        {
            PushLine(position, position + forward * size, Color.blue);
            PushLine(position, position + right * size, Color.red);
            PushLine(position, position + up * size, Color.green);
        }

        public void PushTransform(Transform transform, Vector3 position, float size = 1) => PushTransform(position, transform.up, transform.right, transform.forward, size);
        public void PushTransform(Transform transform, float size = 1) => PushTransform(transform, transform.position, size);

        public void PushBox(Vector3 position, Quaternion rotation, Vector3 extents, Color color)
        {
            var p0 = position + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            var p1 = position + rotation * new Vector3(extents.x, extents.y, extents.z);
            var p3 = position + rotation * new Vector3(-extents.x, -extents.y, extents.z);
            var p4 = position + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            var p5 = position + rotation * new Vector3(extents.x, -extents.y, -extents.z);
            var p6 = position + rotation * new Vector3(-extents.x, extents.y, extents.z);
            var p7 = position + rotation * new Vector3(extents.x, -extents.y, extents.z);
            var p8 = position + rotation * new Vector3(extents.x, extents.y, -extents.z);

            PushLine(p6, p1, color);
            PushLine(p1, p8, color);
            PushLine(p8, p4, color);
            PushLine(p4, p6, color);
            PushLine(p3, p7, color);
            PushLine(p7, p5, color);
            PushLine(p5, p0, color);
            PushLine(p0, p3, color);
            PushLine(p6, p3, color);
            PushLine(p1, p7, color);
            PushLine(p8, p5, color);
            PushLine(p4, p0, color);
        }

        public void PushBox(Vector3 position, Vector3 extents, Color color) => PushBox(position, Quaternion.identity, extents, color);
        public void PushBox(Bounds bounds, Color color) => PushBox((bounds.min + bounds.max) / 2, (bounds.max - bounds.min) / 2, color);
        public void PushBox(Bounds bounds, Quaternion rotation, Color color) => PushBox((bounds.min + bounds.max) / 2, rotation, (bounds.max - bounds.min) / 2, color);
        public void PushLocalBox(Bounds bounds, Vector3 position, Quaternion rotation, Color color) => PushBox(position + rotation * (bounds.max + bounds.min) / 2, rotation, (bounds.max - bounds.min) / 2, color);

        public void PushArc(Vector3 position, Vector3 normal, Vector3 tangent, float radius, float fromAngle, float toAngle, Color color, int segments = 10)
        {
            if (segments <= 2)
                return;

            for (int i = 0, j = 1; j <= segments; i = j++)
            {
                var anglei = Mathf.Lerp(fromAngle, toAngle, (float)i / segments);
                var offseti = Quaternion.AngleAxis(anglei, normal) * tangent * radius;

                var anglej = Mathf.Lerp(fromAngle, toAngle, (float)j / segments);
                var offsetj = Quaternion.AngleAxis(anglej, normal) * tangent * radius;

                PushLine(position + offseti, position + offsetj, color);
            }
        }

        public void PushEllipse(Vector3 position, Quaternion rotation, Color color, float radiusX, float radiusZ, int segments = 20)
            => PushEllipse(position, rotation.GetUp(), rotation.GetRight(), color, radiusX, radiusZ, segments);

        public void PushEllipse(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float radiusX, float radiusZ, int segments = 20)
        {
            if (segments <= 2)
                return;

            var binormal = Vector3.Cross(normal, tangent);
            for (int i = 0, j = 1; j < segments + 1; i = j++)
            {
                var xi = Mathf.Sin(Mathf.Deg2Rad * i * (360f / segments)) * radiusX;
                var zi = Mathf.Cos(Mathf.Deg2Rad * i * (360f / segments)) * radiusZ;
                var xj = Mathf.Sin(Mathf.Deg2Rad * j * (360f / segments)) * radiusX;
                var zj = Mathf.Cos(Mathf.Deg2Rad * j * (360f / segments)) * radiusZ;

                PushLine(position + tangent * xi + binormal * zi, position + tangent * xj + binormal * zj, color);
            }
        }

        public void PushCircle(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float radius, int segments = 20) => PushEllipse(position, normal, tangent, color, radius, radius, segments);
        public void PushCircle(Vector3 position, Quaternion rotation, Color color, float radius, int segments = 20) => PushEllipse(position, rotation, color, radius, radius, segments);

        public void PushRectangle(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float width, float height)
        {
            var binormal = Vector3.Cross(normal, tangent);

            var p0 = position - width / 2 * tangent + height / 2 * binormal;
            var p1 = position + width / 2 * tangent + height / 2 * binormal;
            var p2 = position + width / 2 * tangent - height / 2 * binormal;
            var p3 = position - width / 2 * tangent - height / 2 * binormal;

            PushLine(p0, p1, color);
            PushLine(p1, p2, color);
            PushLine(p2, p3, color);
            PushLine(p3, p0, color);
        }

        public void PushRectangle(Vector3 position, Quaternion rotation, Color color, float width, float height) => PushRectangle(position, rotation.GetUp(), rotation.GetRight(), color, width, height);

        public void PushSquare(Vector3 position, Vector3 normal, Vector3 tangent, Color color, float size) => PushRectangle(position, normal, tangent, color, size, size);
        public void PushSquare(Vector3 position, Quaternion rotation, Color color, float size) => PushRectangle(position, rotation, color, size, size);

        public void Draw(Material material, int layer = 0)
        {
            if (_vertices.Count == 0)
                return;

            var idx = Enumerable.Range(0, _vertices.Count);

            _mesh.vertices = _vertices.ToArray();
            _mesh.colors = _colors.ToArray();
            _mesh.uv = idx.Select(_ => Vector2.zero).ToArray();
            _mesh.normals = idx.Select(_ => Vector3.zero).ToArray();
            _mesh.SetIndices(idx.ToArray(), MeshTopology.Lines, 0);
            _mesh.RecalculateBounds();

            Graphics.DrawMesh(_mesh, Matrix4x4.identity, material, layer);
        }

        public void Clear()
        {
            _vertices.Clear();
            _colors.Clear();
        }
    }
}