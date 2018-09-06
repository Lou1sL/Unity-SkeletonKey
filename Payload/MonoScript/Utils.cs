using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public static class Utils
    {

        public static string Vec2Str(Vector3 v3)
        {
            return " X:" + Rnd(v3.x) + " Y:" + Rnd(v3.y) + " Z:" + Rnd(v3.z);
        }
        public static string Rnd(float f)
        {
            return f.ToString("F2");
        }
        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        public static Matrix4x4 ScreenToWorldMatrix(Camera cam)
        {
            // Make a matrix that converts from
            // screen coordinates to clip coordinates.
            var rect = cam.pixelRect;
            var viewportMatrix = Matrix4x4.Ortho(rect.xMin, rect.xMax, rect.yMin, rect.yMax, -1, 1);

            // The camera's view-projection matrix converts from world coordinates to clip coordinates.
            var vpMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

            // Setting column 2 (z-axis) to identity makes the matrix ignore the z-axis.
            // Instead you get the value on the xy plane!
            vpMatrix.SetColumn(2, new Vector4(0, 0, 1, 0));

            // Going from right to left:
            // convert screen coords to clip coords, then clip coords to world coords.
            return vpMatrix.inverse * viewportMatrix;
        }
        public static Vector2 ScreenToWorldPointPerspective(Vector2 point)
        {
            return ScreenToWorldMatrix(Camera.main).MultiplyPoint(point);
        }

    }
}
