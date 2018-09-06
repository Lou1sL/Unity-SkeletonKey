using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class TransformMover:MonoBehaviour
    {
        private bool Active = false;

        private KeyCode Switch = KeyCode.ScrollLock;

        private Transform MovingTransform;
        private Vector3 Distance = new Vector3();


        private void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                if (Active)
                {
                    Active = false;
                    MovingTransform = null;
                    Distance = Vector3.zero;
                }
                else
                {
                    GameObject go = GameObject.Find(TransformPath);

                    if (go)
                    {
                        Active = true;
                        MovingTransform = go.transform;
                        Distance = MovingTransform.position - Camera.main.transform.position;
                    }
                    else
                    {
                        Debug.LogError("__Injector--: TransformMover: \r\n GameObject Not Found!Are you sure you entered the right path?");

                        Active = false;
                        MovingTransform = null;
                        Distance = Vector3.zero;
                    }
                }
            }

            if (!Active) return;
            

            if (MovingTransform)
            {
                MovingTransform.position = Distance + Camera.main.transform.position;

            }
            else
            {
                Debug.LogError("__Injector--: TransformMover: \r\n GameObject Has Been Destoried!");
            }

            

        }


        private string TransformPath = string.Empty;
        private void OnGUI()
        {
            if(!Active) TransformPath = GUI.TextField(new Rect(Screen.width * 0.35f, Screen.height * 0.05f, Screen.width * 0.3f, 20), TransformPath, 50);
        }
    }
}
