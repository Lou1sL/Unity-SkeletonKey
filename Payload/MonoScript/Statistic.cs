using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Payload.MonoScript
{
    public class Statistic : MonoBehaviour
    {

        private bool Active = true;
        private KeyCode Switch = KeyCode.Delete;

        private Rect LabelRect = new Rect(Screen.width * 0.7f, Screen.height * 0.75f, Screen.width * 0.3f, Screen.height * 0.25f);

        private void OnGUI()
        {
            if (!Active) return;

            string str = "Game Statistics:\n(Unity Ver: " + Application.unityVersion + ")\n\n";

            str += "FPS: " + Rnd((1.0f / Time.deltaTime)) + "\n";
            str += "Current Scene: " + SceneManager.GetActiveScene().name + "\n";
            str += "Main Camera: " + Camera.main.name + "\n";
            str += "    Position:" + Vec2Str(Camera.main.transform.position) + "\n";
            str += "    EularAng:" + Vec2Str(Camera.main.transform.eulerAngles) + "\n";


            GUI.Label(LabelRect, str, new GUIStyle(GUI.skin.label) { fontSize = 13 });
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }


        private static string Vec2Str(Vector3 v3)
        {
            return " X:" + Rnd(v3.x) + " Y:" + Rnd(v3.y) + " Z:" + Rnd(v3.z);
        }
        private static string Rnd(float f)
        {
            return f.ToString("F2");
        }
    }

}
