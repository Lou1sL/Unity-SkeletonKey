using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Tetris : MonoBehaviour
    {
        private KeyCode PlayTetris = KeyCode.Pause;
        private bool PlayingTetris = true;
        private void Update()
        {
            PlayingTetris = Input.GetKeyDown(PlayTetris) ? (!PlayingTetris) : PlayingTetris;
        }
        private void OnGUI()
        {
            game();
        }

        private void game()
        {

        }
    }
}
