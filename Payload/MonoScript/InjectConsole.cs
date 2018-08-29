using System.Collections.Generic;
using UnityEngine;

namespace Payload.MonoScript
{
    public class InjectConsole : MonoBehaviour
    {
        public bool Active = true;
        public KeyCode Switch = KeyCode.Home;
        private Rect ConsoleRect = new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.4f, Screen.height * 0.6f);
        private List<string> ConsoleString = new List<string>();
        private Vector2 scrollPosition = new Vector2();


        private void Start()
        {
            Application.logMessageReceived +=
                (string condition, string stackTrace, LogType type) =>
                {
                    scrollPosition = new Vector2(0, System.Single.MaxValue - 1);
                    ConsoleString.Add("<size=18>" + TagString(condition, type) + " </size>" + "\n" + stackTrace + "\n");

                    if (ConsoleString.Count > 20)
                    {
                        ConsoleString.RemoveAt(0);
                    }
                };
        }
        private void OnGUI()
        {
            if (!Active) return;
            GUILayout.Window(0, ConsoleRect, (id) =>
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(ConsoleRect.width), GUILayout.Height(ConsoleRect.height - 20));
                GUILayout.Label(GetConsoleString(), new GUIStyle(GUI.skin.label) { fontSize = 13 });
                GUILayout.EndScrollView();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("清除", GUILayout.Height(20), GUILayout.Width(ConsoleRect.width * 0.5f))) Clear();
                if (GUILayout.Button("关闭(按HOME键开启)", GUILayout.Height(20), GUILayout.Width(ConsoleRect.width * 0.5f))) Active = false;
                GUILayout.EndHorizontal();

            }, "留白君的重定向控制台", new GUIStyle(GUI.skin.window) { fontSize = 18 });
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
                case LogType.Log: ColorTag = "<color=lightblue>"; break;
                case LogType.Warning: ColorTag = "<color=yellow>"; break;
                case LogType.Error: ColorTag = "<color=red>"; break;
                case LogType.Exception: ColorTag = "<color=brown>"; break;
                case LogType.Assert: ColorTag = "<color=magenta>"; break;
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
