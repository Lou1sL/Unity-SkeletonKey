using UnityEngine;
//SceneManagement feture implemented after Unity 5.3(include)
//using UnityEngine.SceneManagement;

namespace Payload.MonoScript
{
    public class GameStatistic : MonoBehaviour
    {

        private bool Active = true;
        private KeyCode Switch = KeyCode.Delete;

        private Rect WindowRect = new Rect(Screen.width * 0.68f, Screen.height * 0.02f, Screen.width * 0.3f, Screen.height * 0.48f);
        private Vector2 ScrollPosition = new Vector2();

        private void OnGUI()
        {
            if (!Active) return;

            string str = "(Unity Ver: " + Application.unityVersion + ")\n\n";

            str += "FPS: " + Utils.Rnd((1.0f / Time.deltaTime)) + "\n";
            //After Unity 5.3(include)
            //str += "Current Scene: " + SceneManager.GetActiveScene().name + "\n";
            //Before Unity 5.3
            str += "Current Scene: " + Application.loadedLevelName + "\n";
            str += "Main Camera: " + Utils.GetGameObjectPath(Camera.main.gameObject) + "\n";
            str += "    Position:" + Utils.Vec2Str(Camera.main.transform.position) + "\n";
            str += "    EularAng:" + Utils.Vec2Str(Camera.main.transform.eulerAngles) + "\n";

            
            GUILayout.Window(WindowID.GAME_STATISTIC, WindowRect, (id) =>
            {
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(WindowRect.width), GUILayout.Height(WindowRect.height));
                GUILayout.Label(str, new GUIStyle(GUI.skin.label) { fontSize = 13 });
                GUILayout.EndScrollView();
            }, "Game Statistic", new GUIStyle(GUI.skin.window) { fontSize = 15 });
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }


        
    }

}
