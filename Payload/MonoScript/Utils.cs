using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public static class InjectDebug
    {
        public const string prefix = "__Injector--: ";
        public static void InjectLog(this MonoBehaviour mono, object msg)
        {
            Debug.Log(prefix + mono.GetType().Name + ":\r\n" + msg.ToString());
        }
        public static void InjectLogWarning(this MonoBehaviour mono, object msg)
        {
            Debug.LogWarning(prefix + mono.GetType().Name + ":\r\n" + msg.ToString());
        }
        public static void InjectLogError(this MonoBehaviour mono, object msg, Exception e = null)
        {
            string trace = string.Empty;
            if (e != null)
            {
                trace = "\r\n" + e.Message + "\r\n" + e.StackTrace;
            }
            Debug.LogError(prefix + mono.GetType().Name + ":\r\n" + msg.ToString() + trace);
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
        //Ln1
        public static Rect FreeCamRect      = new Rect(Screen.width * 0.02f, Screen.height * 0.02f, Screen.width * 0.30f, Screen.height * 0.14f);
        public static Rect HierRect         = new Rect(Screen.width * 0.02f, Screen.height * 0.18f, Screen.width * 0.30f, Screen.height * 0.80f);
        //Ln2
        public static Rect CompoRect        = new Rect(Screen.width * 0.34f, Screen.height * 0.02f, Screen.width * 0.3f, Screen.height * 0.40f);
        public static Rect PropRect         = new Rect(Screen.width * 0.34f, Screen.height * 0.44f, Screen.width * 0.3f, Screen.height * 0.54f);
        //Ln3
        public static Rect StatisticRect    = new Rect(Screen.width * 0.66f, Screen.height * 0.02f, Screen.width * 0.32f, Screen.height * 0.46f);
        public static Rect ConsoleRect      = new Rect(Screen.width * 0.66f, Screen.height * 0.50f, Screen.width * 0.32f, Screen.height * 0.48f);
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

    

    public class Reflector
    {
        public enum VarCata
        {
            Field,
            Property
        }
        
        

        public Component component { get; private set; }
        private List<PropertyInfo> props;
        private List<FieldInfo> fields;
        //List<MethodInfo> methods = new List<MethodInfo>(mono.GetType().GetMethods());

        private List<object> propsModifyCache;
        private List<object> fieldsModifyCache;
        
        public Reflector(Component component)
        {
            this.component = component;
            props = new List<PropertyInfo>(component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            fields = new List<FieldInfo>(component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

            propsModifyCache = new List<object>();
            for (int i = 0; i < props.Count; i++)
            {
                propsModifyCache.Add(props[i].CanRead ? props[i].GetValue(component, null) : "UNREADABLE");
            }

            fieldsModifyCache = new List<object>();
            for (int i = 0; i < fields.Count; i++)
            {
                fieldsModifyCache.Add(fields[i].GetValue(component));
            }
        }

        private void UpdateCache(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
                propsModifyCache[Pointer] = props[Pointer].GetValue(component, null);

            if (cata == VarCata.Field)
                fieldsModifyCache[Pointer] = fields[Pointer].GetValue(component);
        }
        private void SetCache(VarCata cata, int Pointer, object o)
        {
            if (cata == VarCata.Property)
                propsModifyCache[Pointer] = o;

            if (cata == VarCata.Field)
                fieldsModifyCache[Pointer] = o;
        }
        private object GetCache(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
                return propsModifyCache[Pointer];

            if (cata == VarCata.Field)
                return fieldsModifyCache[Pointer];

            return null;
        }

        private void SetVal(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
            {
                if (props[Pointer].CanWrite)
                {
                    props[Pointer].SetValue(component, propsModifyCache[Pointer], null);
                }
                else
                    Debug.LogError(InjectDebug.prefix + "This property is Readonly.");
            }

            if (cata == VarCata.Field)
            {
                fields[Pointer].SetValue(component, fieldsModifyCache[Pointer]);
            }
        }
        private object GetVal(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
            {
                if (props[Pointer].CanRead)
                    return props[Pointer].GetValue(component, null);
                else
                    return "UNREADABLE!";
            }

            if (cata == VarCata.Field)
                return fields[Pointer].GetValue(component);

            return null;
        }
        private string GetFullName(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
                return props[Pointer].PropertyType.Name + " " + props[Pointer].Name;

            if (cata == VarCata.Field)
            {
                string visit = string.Empty;
                if (fields[Pointer].IsPublic) visit = "Public ";
                if (fields[Pointer].IsPrivate) visit = "Private ";
                return visit + (fields[Pointer].IsStatic ? "Static " : "") + fields[Pointer].FieldType.Name + " " + fields[Pointer].Name;
            }

            return null;
        }
        private Type GetType(VarCata cata, int Pointer)
        {
            if (cata == VarCata.Property)
                return props[Pointer].PropertyType;

            if (cata == VarCata.Field)
                return fields[Pointer].FieldType;

            return null;
        }
        private bool IsPropertySetable(int Pointer)
        {
            return props[Pointer].CanWrite;
        }
        
        private int PropertyCount { get { return props.Count; } }
        private int FieldCount { get { return fields.Count; } }

        public void DrawVarList(VarLocker vl)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("---------------------------------------------------------------------------");
            for (int i = 0; i < PropertyCount; i++)
            {
                try
                {
                    GUILayout.Label(GetFullName(VarCata.Property, i));
                    GUILayout.Label("Get: " + GetVal(VarCata.Property, i));
                    if (IsPropertySetable(i)) VarEditBox(vl,i, VarCata.Property);
                }
                catch (Exception e)
                {
                    Debug.LogError(InjectDebug.prefix + "Something Went Wrong When Drawing Variables!" + "\r\n" + e.Message + "\r\n" + e.StackTrace);
                }

                GUILayout.Label("---------------------------------------------------------------------------");
            }
            for (int i = 0; i < FieldCount; i++)
            {
                try
                {
                    GUILayout.Label(GetFullName(VarCata.Field, i) + " = " + GetVal(VarCata.Field, i));
                    VarEditBox(vl,i, VarCata.Field);
                }
                catch (Exception e)
                {
                    Debug.LogError(InjectDebug.prefix + "Something Went Wrong When Drawing Variables!" + "\r\n" + e.Message + "\r\n" + e.StackTrace);
                }

                GUILayout.Label("---------------------------------------------------------------------------");
            }



            GUILayout.EndVertical();

        }
        private void VarEditBox(VarLocker vl,int i, VarCata cata)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            bool Editable = true;

            Type t = GetType(cata, i);
            object o = GetCache(cata, i);

            if (t == typeof(bool))
                o = GUILayout.Toggle((bool)o, "");
            else if (t == typeof(int))
                o = Convert.ToInt32(GUILayout.TextField(((int)o) + ""));
            else if (t == typeof(float))
                o = Convert.ToSingle(GUILayout.TextField(((float)o) + ""));
            else if (t == typeof(double))
                o = Convert.ToDouble(GUILayout.TextField(((double)o) + ""));
            else if (t == typeof(string))
                o = GUILayout.TextField((string)o);
            else if (t == typeof(Vector2))
            {
                GUILayout.BeginHorizontal();
                o = new Vector2(
                    Convert.ToSingle(GUILayout.TextField(((Vector2)o).x.ToString())),
                    Convert.ToSingle(GUILayout.TextField(((Vector2)o).y.ToString()))
                    );
                GUILayout.EndHorizontal();
            }
            else if (t == typeof(Vector3))
            {
                GUILayout.BeginHorizontal();
                o = new Vector3(
                    Convert.ToSingle(GUILayout.TextField(((Vector3)o).x.ToString())),
                    Convert.ToSingle(GUILayout.TextField(((Vector3)o).y.ToString())),
                    Convert.ToSingle(GUILayout.TextField(((Vector3)o).z.ToString()))
                    );
                GUILayout.EndHorizontal();
            }
            else if (t == typeof(Color))
            {
                GUILayout.BeginHorizontal();
                o = new Color(
                    GUILayout.HorizontalSlider(((Color)o).r, 0f, 1f),
                    GUILayout.HorizontalSlider(((Color)o).g, 0f, 1f),
                    GUILayout.HorizontalSlider(((Color)o).b, 0f, 1f),
                    GUILayout.HorizontalSlider(((Color)o).a, 0f, 1f)
                    );
                GUILayout.EndHorizontal();
            }
            else Editable = false;

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            bool Locked = false;
            if (cata == VarCata.Field) Locked = vl.IsLocked(component, fields[i]);
            if (cata == VarCata.Property) Locked = vl.IsLocked(component, props[i]);

            if (GUILayout.Button(Locked ? "UNLOCK" : "LOCK", GUILayout.Width(80)))
            {
                if (cata == VarCata.Field)
                {
                    if (Locked) vl.UnLock(component, fields[i]);
                    else vl.Lock(component, fields[i], fields[i].GetValue(component));
                }
                if (cata == VarCata.Property)
                {
                    if (Locked) vl.UnLock(component, props[i]);
                    else vl.Lock(component, props[i], props[i].GetValue(component, null));
                }
            }

            if (Editable)
            {
                SetCache(cata, i, o);
                
                if (GUILayout.Button("Update", GUILayout.Width(55)))
                    UpdateCache(cata, i);

                if (GUILayout.Button("Set", GUILayout.Width(35)))
                    SetVal(cata, i);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        

        public class VarLocker
        {
            private class Locker
            {
                private Component component;
                private FieldInfo field = null;
                private PropertyInfo property = null;

                public object LockObj;

                public static bool operator ==(Locker x, Locker y)
                {
                    return x.component == y.component && x.field == y.field && x.property == y.property && x.LockObj == y.LockObj;
                }
                public static bool operator !=(Locker x, Locker y)
                {
                    return x.component != y.component || x.field != y.field || x.property != y.property || x.LockObj != y.LockObj;
                }
                public override bool Equals(object obj)
                {
                    if (obj == null) return false;
                    if (obj is Locker) return ((Locker)obj).component == component && ((Locker)obj).field == field && ((Locker)obj).property == property && ((Locker)obj).LockObj == LockObj;
                    return base.Equals(obj);
                }
                public override int GetHashCode()
                {
                    return component.GetHashCode() + field.GetHashCode() + property.GetHashCode() + LockObj.GetHashCode();
                }

                public Locker(Component component, FieldInfo field, object o)
                {
                    this.component = component; this.field = field; LockObj = o;
                }
                public Locker(Component component, PropertyInfo property, object o)
                {
                    this.component = component; this.property = property; LockObj = o;
                }

                public bool Is(Locker l)
                {
                    return component == l.component && field == l.field && property == l.property;
                }
                public void LockUpdate()
                {
                    if (field != null)
                        field.SetValue(component, LockObj);
                    if (property != null && property.CanWrite)
                        property.SetValue(component, LockObj, null);
                }
            }

            private List<Locker> LockList = new List<Locker>();
            private bool IsLocked(Locker l)
            {
                foreach (Locker lo in LockList)
                {
                    if(lo.Is(l))return true;
                }
                return false;
            }
            private void AddList(Locker l)
            {
                foreach(Locker lo in LockList)
                {
                    if (lo.Is(l))
                    {
                        lo.LockObj = l.LockObj;
                        return;
                    }
                }
                LockList.Add(l);
            }
            private void RmList(Locker l)
            {
                if (LockList.Count > 0)
                    for (int i = 0; i < LockList.Count; i++)
                    {
                        if (LockList[i].Is(l))
                        {
                            LockList.RemoveAt(i);
                            i--;
                        }
                    }
            }

            public void Lock(Component component,FieldInfo field,object o)
            {
                AddList(new Locker(component, field, o));
            }
            public void Lock(Component component, PropertyInfo property, object o)
            {
                AddList(new Locker(component, property, o));
            }
            public void UnLock(Component component, FieldInfo field)
            {
                RmList(new Locker(component, field, null));
            }
            public void UnLock(Component component, PropertyInfo property)
            {
                RmList(new Locker(component, property, null));
            }
            public bool IsLocked(Component component, FieldInfo field)
            {
                return IsLocked(new Locker(component, field, null));
            }
            public bool IsLocked(Component component, PropertyInfo property)
            {
                return IsLocked(new Locker(component, property, null));
            }
            public void LockUpdate()
            {
                foreach (Locker lo in LockList) lo.LockUpdate();
            }
        }
    }

    public class GameObjectTree
    {
        public class GameObjectNode
        {
            public GameObject Me { get; private set; }
            public int Level { get; private set; }
            public string Path { get; private set; }
            public bool Expanded = false;
            public List<GameObjectNode> Children { get; private set; }

            public GameObjectNode(GameObject go, int level)
            {
                Me = go;
                Level = level;
                Path = Utils.GetGameObjectPath(Me);
                Children = new List<GameObjectNode>();
                int childCount = Me.transform.childCount;
                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        Children.Add(new GameObjectNode(Me.transform.GetChild(i).gameObject, level + 1));
                    }
                }
            }

            public void OnGUIHierarchyNode()
            {
                GUILayout.BeginHorizontal();

                if (Level > 0)
                    GUILayout.Label(" ┣", GUILayout.Width(20));
                if (Level > 1)
                    for (int i = 0; i < Level - 1; i++)
                        GUILayout.Label(" ━", GUILayout.Width(20));


                if (Children.Count > 0)
                    if (GUILayout.Button(Expanded ? "↑" : "↓", GUILayout.Width(20)))
                    {
                        Expanded = !Expanded;
                    }
                if (GUILayout.Button("□", GUILayout.Width(20)))
                {
                    Inspector.Activate(Me.transform);
                }
                GUILayout.Label(Me.name, AllGUIStyle.DEFAULT_LABEL);

                GUILayout.EndHorizontal();

                if (Expanded)
                    foreach (GameObjectNode node in Children)
                    {
                        node.OnGUIHierarchyNode();
                    }
            }
        }
        public List<GameObjectNode> RootNodes { get; private set; }

        public GameObjectTree()
        {
            RootNodes = new List<GameObjectNode>();
            //UpdateTree();
        }
        public void UpdateTree()
        {
            RootNodes.Clear();
            //TODO:It only works on Unity v5.3 and after!
            GameObject [] RootGO = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in RootGO) RootNodes.Add(new GameObjectNode(go,0));
        }
        
        private Vector2 GUIScrollPosition = new Vector2();
        public void OnGUIHierarchy()
        {

            GUIScrollPosition = GUILayout.BeginScrollView(GUIScrollPosition);
            GUILayout.BeginVertical();
            foreach (GameObjectNode node in RootNodes)
            {
                node.OnGUIHierarchyNode();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }

    public class DrawCollider
    {
        private class ColliderPostRenderer
        {
            public Collider collider { get; private set; }
            public Collider2D collider2D { get; private set; }

            public ColliderPostRenderer(GameObject go)
            {
                collider = go.GetComponent<Collider>();
                collider2D = go.GetComponent<Collider2D>();
            }
            public void OnPostRenderer()
            {
                if (collider) PostRenderDrawCollider(collider);
                if (collider2D) PostRenderDrawCollider2D(collider2D);
            }
            private static void PostRenderDrawCollider2D(Collider2D c2d)
            {
                var vertices = new Vector3[4];
                var thisMatrix = c2d.transform.localToWorldMatrix;
                var storedRotation = c2d.transform.rotation;
                c2d.transform.rotation = Quaternion.identity;

                var extents = c2d.bounds.extents;
                vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
                vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, 0));
                vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, 0));
                vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, 0));

                c2d.transform.rotation = storedRotation;

                //UP
                GL.Vertex3(vertices[0].x, vertices[0].y, 0);
                GL.Vertex3(vertices[1].x, vertices[1].y, 0);

                //SIDE
                GL.Vertex3(vertices[0].x, vertices[0].y, 0);
                GL.Vertex3(vertices[2].x, vertices[2].y, 0);

                GL.Vertex3(vertices[1].x, vertices[1].y, 0);
                GL.Vertex3(vertices[3].x, vertices[3].y, 0);

                //BOTTOM
                GL.Vertex3(vertices[2].x, vertices[2].y, 0);
                GL.Vertex3(vertices[3].x, vertices[3].y, 0);
            }
            private static void PostRenderDrawCollider(Collider c)
            {
                //Get all verticles
                var vertices = new Vector3[8];
                var thisMatrix = c.transform.localToWorldMatrix;
                var storedRotation = c.transform.rotation;
                c.transform.rotation = Quaternion.identity;

                var extents = c.bounds.extents;
                vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
                vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
                vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
                vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
                vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
                vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
                vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
                vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);

                c.transform.rotation = storedRotation;

                //GL process
                //UP
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);
                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);

                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);
                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);

                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);
                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);

                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);

                //SIDE
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);

                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);
                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);

                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);
                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);

                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);
                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);

                //BOTTOM
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);
                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);

                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);
                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);

                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);
                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);

                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);
            }
        }

        private List<ColliderPostRenderer> ColliderCache;
        public DrawCollider()
        {
            ColliderCache = new List<ColliderPostRenderer>();
        }

        public void UpdateCache(GameObject[] AllObjectsInScene,Vector3 center,float dist,string filter)
        {
            ColliderCache.Clear();
            foreach (GameObject go in AllObjectsInScene)
            {
                if ((filter == "" || go.name.ToUpper().Contains(filter.ToUpper())) && Vector3.Distance(center, go.transform.position) <= dist)
                {
                    ColliderPostRenderer pr = new ColliderPostRenderer(go);
                    if (pr.collider || pr.collider2D)
                        ColliderCache.Add(new ColliderPostRenderer(go));
                }
            }
        }
        public void OnPostRenderer(Material lineMat)
        {
            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(new Color(0f, 1f, 0f, 1f));

            foreach (ColliderPostRenderer pr in ColliderCache)
            {
                pr.OnPostRenderer();
            }

            GL.End();
        }


    }

    public class GameObjectSearch
    {
        private class GameObjectWithPath
        {
            public GameObject gameObject { get; private set; }
            public string path { get; private set; }
            public GameObjectWithPath(GameObject go)
            {
                gameObject = go;
                path = Utils.GetGameObjectPath(go);
            }
        }
        private List<GameObjectWithPath> Result = new List<GameObjectWithPath>();

        public void Search(GameObject[] AllObjectsInScene,string str)
        {
            Result.Clear();
            foreach(GameObject go in AllObjectsInScene)
            {
                if (go.name.ToUpper().Contains(str.ToUpper())) Result.Add(new GameObjectWithPath(go));
            }
        }

        private Vector2 GUIScrollPosition = new Vector2();
        public void OnGUISearch()
        {
            GUIScrollPosition = GUILayout.BeginScrollView(GUIScrollPosition);
            GUILayout.BeginVertical();
            foreach (GameObjectWithPath gowp in Result)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("□", GUILayout.Width(20)))
                {
                    Inspector.Activate(gowp.gameObject.transform);
                }
                GUILayout.Label(gowp.path, AllGUIStyle.DEFAULT_LABEL);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}
