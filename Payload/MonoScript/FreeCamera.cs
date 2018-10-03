using UnityEngine;

namespace Payload.MonoScript
{
    public class FreeCamera : MonoBehaviour
    {
        private Vector3 DefaultPosition;
        private Quaternion DefaultRotation;
        private Vector3 NewPosition;
        private Quaternion NewRotation;
        public float RotationSpeed = 1f;
        public float MoveSpeed = 10f;


        private void Awake()
        {
            Activate();
        }
        void Update()
        {
            if (!Input.GetKey(KeyCode.LeftAlt)) return;

            xDeg += Input.GetAxis("Mouse X") * RotationSpeed;
            yDeg -= Input.GetAxis("Mouse Y") * RotationSpeed;
            yDeg = Utils.ClampAngle(yDeg, -80, 80);

            spd = 0f;
            if (Input.GetMouseButton(0))
                spd = MoveSpeed;
            if (Input.GetMouseButton(1))
                spd = -MoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                spd *= 2f;
        }

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float spd = 0f;
        private void LateUpdate()
        {
            transform.position = NewPosition;
            transform.rotation = NewRotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yDeg, xDeg, 0), Time.deltaTime * 5f);
            transform.position += transform.rotation * Vector3.forward * Time.deltaTime * spd;

            NewPosition = transform.position;
            NewRotation = transform.rotation;
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Screen.width, Screen.height), "<color=#00FF00>" + AimingObjName + "</color>");
            AllRect.FreeCamRect = GUILayout.Window(WindowID.FREE_CAM_VALUES, AllRect.FreeCamRect, (id) =>
            {
                if (GUILayout.Button("Update Collider Cache"))
                {
                    drawCollider.UpdateCache(FindObjectsOfType<GameObject>(), GetComponent<Camera>().transform.position, DrawDistance, "");
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Mov Speed:", GUILayout.Width(110));
                MoveSpeed = GUILayout.HorizontalSlider(MoveSpeed, 0f, 200f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rot Speed:", GUILayout.Width(110));
                RotationSpeed = GUILayout.HorizontalSlider(RotationSpeed, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("GizmosDist(0-500)", GUILayout.Width(110));
                DrawDistance = GUILayout.HorizontalSlider(DrawDistance, 0f, 500f);
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Deactivate"))
                    DeActivate();

                GUI.DragWindow(new Rect(0, 0, AllRect.FreeCamRect.width, 20));
            }, "Free Camera", AllGUIStyle.DEFAULT_WINDOW);
        }

        void Activate()
        {
            DefaultPosition = transform.position;
            DefaultRotation = transform.rotation;

            NewPosition = transform.position;
            NewRotation = transform.rotation;

            xDeg = Vector3.Angle(Vector3.right, transform.right);
            yDeg = Vector3.Angle(Vector3.up, transform.up);
        }
        void DeActivate()
        {
            transform.position = DefaultPosition;
            transform.rotation = DefaultRotation;
            Destroy(this);
        }

        
        //Collider Outline
        private new Camera camera;
        private Material LineMat;
        private float DrawDistance = 100f;
        private DrawCollider drawCollider;

        private void Start()
        {
            camera = GetComponent<Camera>();
            drawCollider = new DrawCollider();
            //TODO:May not included in build
            LineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            LineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Disabled);
        }
        private void OnPostRender()
        {
            drawCollider.OnPostRenderer(LineMat);
        }


        //Using Raycast to get aiming obj names.
        private string AimingObjName = string.Empty;
        private void FixedUpdate()
        {
            AimingObjName = MouseRayCastCollider();
        }
        private string MouseRayCastCollider()
        {
            string str = string.Empty;
            RaycastHit[] hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), DrawDistance);
            foreach (RaycastHit hit in hits)
            {
                str += Utils.GetGameObjectPath(hit.transform.gameObject) + "\n";
            }

            RaycastHit2D[] hit2ds = new RaycastHit2D[0];

            if (camera.orthographic) hit2ds = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            else hit2ds = Physics2D.RaycastAll(Utils.ScreenToWorldPointPerspective(Input.mousePosition, camera), Vector2.zero);
            foreach (RaycastHit2D hit2d in hit2ds)
            {
                if (hit2d)
                {
                    str += Utils.GetGameObjectPath(hit2d.transform.gameObject) + "\n";
                }
            }
            return str;
        }
    }
}


