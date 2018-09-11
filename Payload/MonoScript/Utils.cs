using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public static class InjectDebug
    {
        private const string prefix = "__Injector--: ";
        public static void InjectLog(this MonoBehaviour mono, string msg)
        {
            Debug.Log(prefix + mono.GetType().Name+":\r\n"+msg);
        }
        public static void InjectLogWarning(this MonoBehaviour mono, string msg)
        {
            Debug.LogWarning(prefix + mono.GetType().Name + ":\r\n" + msg);
        }
        public static void InjectLogError(this MonoBehaviour mono, string msg, Exception e = null)
        {
            string trace = string.Empty;
            if (e != null)
            {
                trace = "\r\n" + e.Message + "\r\n" + e.StackTrace;
            }
            Debug.LogError(prefix + mono.GetType().Name + ":\r\n" + msg + trace);
        }
    }

    public static class AllGUIStyle
    {
        public static readonly GUIStyle DEFAULT_LABEL = new GUIStyle(GUI.skin.label) { fontSize = 13 };
        public static readonly GUIStyle DEFAULT_WINDOW = new GUIStyle(GUI.skin.window) { fontSize = 15 };
    }

    public static class WindowID
    {
        public const int CONSOLE                            = 0x00;
        public const int GAME_STATISTIC                     = 0x01;
        public const int TRANSFORM_WITH_TRIGGER_LIST        = 0x02;
        public const int TRANSFORM_MODIFIER_COMPONENT_LIST  = 0x03;
        public const int TRANSFORM_MODIFIER_PROPERTIES_LIST = 0x04;
        public const int FREE_CAM_VALUES                    = 0x05;
    }

    public static class AllRect
    {
        public static readonly Rect HierRect         = new Rect(Screen.width * 0.02f, Screen.height * 0.02f, Screen.width * 0.30f, Screen.height * 0.96f);

        public static readonly Rect PathInputerRect  = new Rect(Screen.width * 0.34f, Screen.height * 0.02f, Screen.width * 0.3f, 20);
        public static readonly Rect CompoRect        = new Rect(Screen.width * 0.34f, Screen.height * 0.02f, Screen.width * 0.3f, Screen.height * 0.40f);
        public static readonly Rect PropRect         = new Rect(Screen.width * 0.34f, Screen.height * 0.42f, Screen.width * 0.3f, Screen.height * 0.56f);
        
        public static readonly Rect StatisticRect    = new Rect(Screen.width * 0.66f, Screen.height * 0.02f, Screen.width * 0.32f, Screen.height * 0.44f);
        public static readonly Rect FreeCamRect      = new Rect(Screen.width * 0.66f, Screen.height * 0.48f, Screen.width * 0.32f, Screen.height * 0.10f);
        public static readonly Rect ConsoleRect      = new Rect(Screen.width * 0.34f, Screen.height * 0.60f, Screen.width * 0.64f, Screen.height * 0.38f);
    }


    public static class Utils
    {

        public static string Vec32Str(Vector3 v3)
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
            var rect = cam.pixelRect;
            var viewportMatrix = Matrix4x4.Ortho(rect.xMin, rect.xMax, rect.yMin, rect.yMax, -1, 1);
            var vpMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;
            vpMatrix.SetColumn(2, new Vector4(0, 0, 1, 0));
            return vpMatrix.inverse * viewportMatrix;
        }
        public static Vector2 ScreenToWorldPointPerspective(Vector2 point,Camera camera)
        {
            return ScreenToWorldMatrix(camera).MultiplyPoint(point);
        }

    }
}
