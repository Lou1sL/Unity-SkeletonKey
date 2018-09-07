using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Console : MonoBehaviour
    {
        private bool Active = true;
        private KeyCode Switch = KeyCode.Home;

        private Rect ConsoleRect = new Rect(Screen.width * 0.35f, Screen.height * 0.65f, Screen.width * 0.6f, Screen.height * 0.3f);
        
        private List<string> ConsoleString = new List<string>();
        private Vector2 ScrollPosition = new Vector2();


        private void Awake()
        {
            Application.logMessageReceived +=
                (string condition, string stackTrace, LogType type) =>
                {
                    ScrollPosition = new Vector2(0, System.Single.MaxValue - 1);
                    ConsoleString.Add("<size=15>" + TagString(condition, type) + " </size>" + "\n" + stackTrace + "\n");

                    if (ConsoleString.Count > 20)
                    {
                        ConsoleString.RemoveAt(0);
                    }

                    File.AppendAllText(@".\UnityConsoleInjected.log", "[" + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + "] " + type.ToString() + ": " + condition + "\r\n" + (stackTrace != string.Empty ? stackTrace + "\r\n" : string.Empty));
                };
        }
        private void OnGUI()
        {
            if (!Active) return;
            GUILayout.Window(WindowID.CONSOLE, ConsoleRect, (id) =>
            {
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(ConsoleRect.width), GUILayout.Height(ConsoleRect.height - 20));
                GUILayout.Label(GetConsoleString(), new GUIStyle(GUI.skin.label) { fontSize = 13 });
                GUILayout.EndScrollView();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Clr", GUILayout.Height(20), GUILayout.Width(ConsoleRect.width * 0.5f))) Clear();
                if (GUILayout.Button("Close", GUILayout.Height(20), GUILayout.Width(ConsoleRect.width * 0.5f))) Active = false;
                GUILayout.EndHorizontal();

            }, "Unity Console", new GUIStyle(GUI.skin.window) { fontSize = 15 });
        }
        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }

        private string GetConsoleString()
        {
            string str = string.Empty;
            foreach (string s in ConsoleString) { str += s; }
            return str;
        }
        private string TagString(string str, LogType type)
        {
            string ColorTag = string.Empty;
            switch (type)
            {
                case LogType.Log: ColorTag = "<color=white>Log: "; break;
                case LogType.Warning: ColorTag = "<color=yellow>Warning: "; break;
                case LogType.Error: ColorTag = "<color=red>Error: "; break;
                case LogType.Exception: ColorTag = "<color=red>Exception: "; break;
                case LogType.Assert: ColorTag = "<color=magenta>Assert: "; break;
            }
            string ColorTagEnd = " </color>";

            return ColorTag + str + ColorTagEnd;
        }
        private void Clear()
        {
            ConsoleString.Clear();
            ConsoleString = new List<string>();
        }
    }
}
