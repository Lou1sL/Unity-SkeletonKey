using UnityEngine;

namespace Payload.MonoScript
{
    public class FreeCamera : MonoBehaviour
    {
        private bool Active = false;
        private KeyCode Switch = KeyCode.ScrollLock;

        private Vector3 DefaultPosition;
        private Quaternion DefaultRotation;

        private Vector3 NewPosition;
        private Quaternion NewRotation;

        public float RotationSpeed = 1f;
        public float MoveSpeed = 200f;


        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        

        void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                if (Active)
                {
                    transform.position = DefaultPosition;
                    transform.rotation = DefaultRotation;
                    Active = false;
                }
                else
                {
                    DefaultPosition = transform.position;
                    DefaultRotation = transform.rotation;

                    NewPosition = transform.position;
                    NewRotation = transform.rotation;

                    xDeg = Vector3.Angle(Vector3.right, transform.right);
                    yDeg = Vector3.Angle(Vector3.up, transform.up);
                    Active = true;
                }
            }

            if (!Active) return;

            transform.position = NewPosition;
            transform.rotation = NewRotation;


            xDeg += Input.GetAxis("Mouse X") * RotationSpeed;
            yDeg -= Input.GetAxis("Mouse Y") * RotationSpeed;
            yDeg = Utils.ClampAngle(yDeg, -80, 80);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yDeg, xDeg, 0), Time.deltaTime * 5f);


            float spd = 0f;

            if (Input.GetMouseButton(0))
                spd = MoveSpeed;
            if (Input.GetMouseButton(1))
                spd = -MoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                spd *= 2f;

            transform.position += transform.rotation * Vector3.forward * Time.deltaTime * spd;

            NewPosition = transform.position;
            NewRotation = transform.rotation;
        }
        
        private void OnGUI()
        {
            if (!Active) return;
            Color color = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 120f, 20), "Free Camera");
            GUI.color = color;

        }
    }
}


