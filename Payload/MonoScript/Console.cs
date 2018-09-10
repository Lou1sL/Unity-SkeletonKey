using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Console : MonoBehaviour
    {
        private bool Active = true;
        private KeyCode Switch = KeyCode.PageDown;

        private Rect ConsoleRect = new Rect(Screen.width * 0.34f, Screen.height * 0.6f, Screen.width * 0.64f, Screen.height * 0.38f);
        private Vector2 ScrollPosition = new Vector2();


        private LogBuffer logBuffer = new LogBuffer();
        private sealed class SealedLog
        {
            public System.DateTime time;
            public LogType type;
            public string condition;
            public string stackTrace;

            public override string ToString()
            {
                return "<size=15>" + TagString(condition, type) + " </size>" + "\n" + stackTrace + "\n";
            }

            public string ToRawString()
            {
                return "[" + time.ToString("MM/dd/yyyy HH:mm:ss.fff") + "] " + type.ToString() + ": " + condition + "\r\n" + (stackTrace != string.Empty ? stackTrace + "\r\n" : string.Empty);
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
        }
        private sealed class LogBuffer
        {
            public int MaxCount = 30;

            public bool ShowLog = true;
            public bool ShowWaring = true;
            public bool ShowError = true;

            public bool WriteToFile = false;

            private List<SealedLog> buffer = new List<SealedLog>();
            

            public void Add(SealedLog log)
            {
                buffer.Add(log);
                if (buffer.Count > MaxCount)
                    buffer.RemoveAt(0);

                if (WriteToFile)
                    File.AppendAllText(@".\UnityConsoleInjected.log", log.ToRawString());
            }
            public void Clear()
            {
                buffer.Clear();
            }
            public override string ToString()
            {
                string str = string.Empty;
                foreach (SealedLog l in buffer)
                {
                    if (!ShowLog && l.type == LogType.Log) continue;
                    if (!ShowWaring && l.type == LogType.Warning) continue;
                    if (!ShowError && (l.type == LogType.Error || l.type == LogType.Exception || l.type == LogType.Assert)) continue;
                    str += l.ToString();
                }
                return str;
            }
        }

        private void Awake()
        {
            Application.logMessageReceived +=
                (string condition, string stackTrace, LogType type) =>
                {
                    ScrollPosition = new Vector2(0, System.Single.MaxValue - 1);

                    SealedLog sealedLog = new SealedLog()
                    {
                        time = System.DateTime.Now,
                        type = type,
                        condition = condition,
                        stackTrace = stackTrace
                    };

                    logBuffer.Add(sealedLog);
                };
        }
        private void OnGUI()
        {
            if (!Active) return;
            GUILayout.Window(WindowID.CONSOLE, ConsoleRect, (id) =>
            {
                GUILayout.BeginHorizontal();
                logBuffer.ShowLog = GUILayout.Toggle(logBuffer.ShowLog, "Log");
                logBuffer.ShowWaring = GUILayout.Toggle(logBuffer.ShowWaring, "Warning");
                logBuffer.ShowError = GUILayout.Toggle(logBuffer.ShowError, "Error");
                logBuffer.WriteToFile = GUILayout.Toggle(logBuffer.WriteToFile, "Write2File");
                GUILayout.EndHorizontal();

                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.Label(logBuffer.ToString(), GUIStyles.DEFAULT_LABEL);
                GUILayout.EndScrollView();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Clr", GUILayout.Height(20))) logBuffer.Clear();
                if (GUILayout.Button("Close", GUILayout.Height(20))) Active = false;
                GUILayout.EndHorizontal();

            }, "Unity Console", GUIStyles.DEFAULT_WINDOW);
        }
        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }
    }
}
