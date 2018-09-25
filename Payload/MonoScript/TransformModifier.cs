using UnityEngine;

namespace Payload.MonoScript
{
    public class TransformModifier:MonoBehaviour
    {
        private bool Active = false;

        private float MvSpd = 100f;

        private Transform TargetTransform;
        private Reflector.VariableModifier TargetComponentModifier;
        
        
        private Vector2 ScrollPosition = new Vector2();
        private Vector2 ScrollPositionProp = new Vector2();

        private static TransformModifier Instance = null;
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
                TargetTransform.position += Vector3.forward * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TargetTransform.position += Vector3.back * MvSpd * Time.deltaTime;
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
                TargetTransform.position += Vector3.up * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightControl))
            {
                TargetTransform.position += Vector3.down * MvSpd * Time.deltaTime;
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
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_COMPONENT_LIST, AllRect.CompoRect, (id) =>
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
                        TargetComponentModifier = new Reflector.VariableModifier(cp);
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

            }, "Components On " + Utils.GetGameObjectPath(TargetTransform.gameObject), AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUIPropertyWindow()
        {
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_PROPERTIES_LIST, AllRect.PropRect, (id) =>
            {
                ScrollPositionProp = GUILayout.BeginScrollView(ScrollPositionProp);

                Reflector.DrawVarList(TargetComponentModifier);

                GUILayout.EndScrollView();

                if (GUILayout.Button("Close"))
                {
                    TargetComponentModifier = null;
                }
            }, "Var On " + TargetComponentModifier.component.GetType().Name, AllGUIStyle.DEFAULT_WINDOW);
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
