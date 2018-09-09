using UnityEngine;
//SceneManagement feture implemented after Unity 5.3(include)
//using UnityEngine.SceneManagement;

namespace Payload.MonoScript
{
    public class GameStatistic : MonoBehaviour
    {

        private bool Active = true;
        private KeyCode Switch = KeyCode.Delete;

        private Rect WindowRect = new Rect(Screen.width * 0.66f, Screen.height * 0.02f, Screen.width * 0.32f, Screen.height * 0.48f);
        private Vector2 ScrollPosition = new Vector2();

        private void OnGUI()
        {
            if (!Active) return;
            
            GUILayout.Window(WindowID.GAME_STATISTIC, WindowRect, (id) =>
            {
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();

                string str = "Unity Ver: " + Application.unityVersion + "\n";
                str += "FPS: " + Utils.Rnd((1.0f / Time.deltaTime)) + "\n\n";
                //After Unity 5.3(include)
                //str += "Current Scene: " + SceneManager.GetActiveScene().name + "\n";
                //Before Unity 5.3
                str += "Current Scene: (" + Application.loadedLevel + ") " + Application.loadedLevelName + "\n\n";
                str += "Cameras (Total:" + Camera.allCamerasCount + "):\n";
                GUILayout.Label(str, GUIStyles.DEFAULT_LABEL);


                foreach (Camera cam in Camera.allCameras)
                {
                    GUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TransformModifier.Activate(Utils.GetGameObjectPath(cam.gameObject));
                    }
                    GUILayout.Label((cam == Camera.main ? "Main" : "") + "Camera: " + Utils.GetGameObjectPath(cam.gameObject), GUIStyles.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();


                    string camstr = string.Empty;
                    camstr += "    Projection:" +
                        (cam.orthographic ?
                        "Orthographic (Size:" + cam.orthographicSize + ")" :
                        "Perspective (Fov:" + cam.fieldOfView + ")")
                        + "\n";

                    camstr += "    Position:" + Utils.Vec2Str(cam.transform.position) + "\n";
                    camstr += "    EularAng:" + Utils.Vec2Str(cam.transform.eulerAngles) + "\n";
                    GUILayout.Label(camstr, GUIStyles.DEFAULT_LABEL);
                }

                
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }, "Game Statistic", GUIStyles.DEFAULT_WINDOW);
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }


        
    }

}
