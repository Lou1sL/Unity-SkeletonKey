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

            string str = "Game Statistics (Unity Ver: " + Application.unityVersion + " )\n\n";

            str += "Current Scene:" + SceneManager.GetActiveScene().name + "\n";


            GUI.Label(LabelRect, str, new GUIStyle(GUI.skin.label) { fontSize = 13 });
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }

    }
}
