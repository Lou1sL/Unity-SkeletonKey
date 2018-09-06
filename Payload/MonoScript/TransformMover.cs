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
        private KeyCode MvSpdInc = KeyCode.PageUp;
        private KeyCode MvSpdDec = KeyCode.PageDown;

        private float MvSpd = 100f;

        private Transform MovingTransform;
        
        private void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                if (Active)
                {
                    Active = false;
                    MovingTransform = null;
                }
                else
                {
                    GameObject go = GameObject.Find(TransformPath);

                    if (go)
                    {
                        Active = true;
                        MovingTransform = go.transform;
                    }
                    else
                    {
                        Debug.LogError("__Injector--: TransformMover: \r\n GameObject Not Found!Are you sure you entered the right path?");

                        Active = false;
                        MovingTransform = null;
                    }
                }
            }

            if (!Active) return;

            if (Input.GetKeyDown(MvSpdInc)) MvSpd += 10f;
            if (Input.GetKeyDown(MvSpdDec)) MvSpd -= 10f;
            if (MvSpd < 0f) MvSpd = 0f;

            if (MovingTransform)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MovingTransform.position += Vector3.forward * MvSpd * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    MovingTransform.position += Vector3.back * MvSpd * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    MovingTransform.position += Vector3.left * MvSpd * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    MovingTransform.position += Vector3.right * MvSpd * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.RightShift))
                {
                    MovingTransform.position += Vector3.up * MvSpd * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.RightControl))
                {
                    MovingTransform.position += Vector3.down * MvSpd * Time.deltaTime;
                }

            }
            else
            {
                Debug.LogError("__Injector--: TransformMover: \r\n GameObject Has Been Destoried!");
            }

            

        }


        public static string TransformPath = string.Empty;
        private void OnGUI()
        {
            if(!Active) TransformPath = GUI.TextField(new Rect(Screen.width * 0.35f, Screen.height * 0.05f, Screen.width * 0.3f, 20), TransformPath, 50);
        }


    }
}
