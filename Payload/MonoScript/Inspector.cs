using UnityEngine;

namespace Payload.MonoScript
{
    public class Inspector:MonoBehaviour
    {
        private bool Active = false;

        private float MvSpd = 100f;

        private Transform TargetTransform;
        private Reflector TargetComponentModifier;
        private Reflector.VarLocker varLocker = new Reflector.VarLocker();
        
        private Vector2 ScrollPosition = new Vector2();
        private Vector2 ScrollPositionProp = new Vector2();

        private static Inspector Instance = null;
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Active)
            {
                if (MvSpd < 0f) MvSpd = 0f;

                if (TargetTransform)
                    MovingInput();
                else
                {
                    this.InjectLogError("GameObject Has Been Destoried!");
                    DeActivate();
                }
            }
        }
        private void LateUpdate()
        {
            varLocker.LockUpdate();
        }

        private void DeActivate()
        {
            Active = false;
            TargetTransform = null;
            TargetComponentModifier = null;
        }
        private void Activate()
        {
            GameObject go = GameObject.Find(TransformPath);

            if (go)
            {
                Active = true;
                TargetTransform = go.transform;
                TargetComponentModifier = null;
            }
            else
            {
                this.InjectLogError("GameObject Not Found!Are you sure you entered the right path?");

                Active = false;
                TargetTransform = null;
                TargetComponentModifier = null;
            }
        }
        private void MovingInput()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                TargetTransform.position += Vector3.up * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TargetTransform.position += Vector3.down * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                TargetTransform.position += Vector3.left * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                TargetTransform.position += Vector3.right * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                TargetTransform.position += Vector3.forward * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightControl))
            {
                TargetTransform.position += Vector3.back * MvSpd * Time.deltaTime;
            }
        }

        private string TransformPath = string.Empty;
        private void OnGUI()
        {
            if (Active)
            {
                if (TargetTransform)
                {
                    OnGUIComponentWindow();
                    if (TargetComponentModifier != null)
                        OnGUIPropertyWindow();
                }
            }
        }

        private void OnGUIComponentWindow()
        {
            AllRect.CompoRect = GUILayout.Window(WindowID.TRANSFORM_MODIFIER_COMPONENT_LIST, AllRect.CompoRect, (id) =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("MoveSpd(0-500)");
                MvSpd = GUILayout.HorizontalSlider(MvSpd, 0f, 200f);
                GUILayout.EndHorizontal();

                TargetTransform.gameObject.SetActive(GUILayout.Toggle(TargetTransform.gameObject.activeInHierarchy, "Active"));
                GUILayout.Label("Position: " + Utils.Vec32Str(TargetTransform.position));
                GUILayout.Label("EulerAng: " + Utils.Vec32Str(TargetTransform.eulerAngles));
                GUILayout.Label("Scale: " + Utils.Vec32Str(TargetTransform.lossyScale));
                
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();
                //
                foreach (Component cp in TargetTransform.GetComponents<Component>())
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TargetComponentModifier = new Reflector(cp);
                    }
                    if (GUILayout.Button("RM", GUILayout.Width(40)))
                    {
                        Destroy(cp);
                        TargetComponentModifier = null;
                    }
                    GUILayout.Label(cp.GetType().Name, AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (GUILayout.Button("Close")) DeActivate();

                GUI.DragWindow(new Rect(0, 0, AllRect.CompoRect.width, 20));
            }, "Inspector (" + TargetTransform.gameObject.name + ")", AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUIPropertyWindow()
        {
            AllRect.PropRect = GUILayout.Window(WindowID.TRANSFORM_MODIFIER_PROPERTIES_LIST, AllRect.PropRect, (id) =>
            {
                ScrollPositionProp = GUILayout.BeginScrollView(ScrollPositionProp);

                TargetComponentModifier.DrawVarList(varLocker);

                GUILayout.EndScrollView();

                if (GUILayout.Button("Close"))
                {
                    TargetComponentModifier = null;
                }

                GUI.DragWindow(new Rect(0, 0, AllRect.PropRect.width, 20));
            }, TargetComponentModifier.component.GetType().Name, AllGUIStyle.DEFAULT_WINDOW);
        }

        public static void Activate(Transform transform)
        {
            if (!Instance)
                return;
            Instance.TransformPath = Utils.GetGameObjectPath(transform.gameObject);
            Instance.Active = true;
            Instance.TargetTransform = transform;
            Instance.TargetComponentModifier = null;
        }
    }
}
