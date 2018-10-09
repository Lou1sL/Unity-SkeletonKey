using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Tetris : MonoBehaviour
    {
        public class Graph
        {
            private List<List<bool>> graph;
            public Graph(int row,int column)
            {
                Init(row, column);
            }
            public void Init(int row, int column)
            {
                graph = new List<List<bool>>(row);
                for (int i = 0; i < row; i++) graph[i] = new List<bool>(column) { false };
            }
            public void Clear()
            {
                for (int i = 0; i < graph.Count; i++)
                {
                    for(int j = 0; j < graph[i].Count; j++)
                    {
                        graph[i][j] = false;
                    }
                }
            }
            public bool GetPoint(int x, int y)
            {
                if (x >= graph.Count || y >= graph[0].Count || x < 0 || y < 0) return false;
                return graph[x][y];
            }
            public void SetPoint(int x,int y,bool b)
            {
                if (x >= graph.Count || y >= graph[0].Count || x < 0 || y < 0) return;
                graph[x][y] = b;
            }
        }

        private Graph graph = new Graph(15, 35);
        private KeyCode PlayTetris = KeyCode.Pause;
        public bool Activated { get; private set; }

        private void Update()
        {
            if (Input.GetKeyDown(PlayTetris))
            {
                Activated = !Activated;
                graph.Clear();
            }
        }
        private void OnGUI()
        {
            
        }

        private void Game()
        {

        }
    }
}
