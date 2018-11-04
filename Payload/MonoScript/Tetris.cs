using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Tetris : MonoBehaviour
    {

        public static class Shape
        {
            public enum ShapeType
            {
                S, Z, T, L, Line, MirroredL, Square
            }

            public static int[,] GetnerateShape(ShapeType type)
            {
                switch (type)
                {
                    case ShapeType.S:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };

                    case ShapeType.Z:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case ShapeType.T:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case ShapeType.L:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case ShapeType.Line:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case ShapeType.MirroredL:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case ShapeType.Square:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    default:return null;
                }
            }

        }


        public class Graph
        {


            //标准俄罗斯方块棋盘大小：10x24
            private int[,] graph = new int[24,10];
            public int Row { get; private set; } = 24;
            public int Column { get; private set; } = 10;

            private int[,] mvobj = new int[4, 4];

            public Graph(int row = 24,int column = 10)
            {
                Row = row;Column = column;
                graph = new int[row, column];
            }
            public int GetPoint(int x, int y)
            {
                if (x >= Row || y >= Column || x < 0 || y < 0) return -1;
                return graph[x, y];
            }
            //逻辑帧
            public void UpdateLogic()
            {
                RowClearChk();


            }



            public void Clear()
            {
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Column; j++)
                        graph[i, j] = 0;
            }
            
            private void SetPoint(int x,int y,int value)
            {
                if (x >= Row || y >= Column || x < 0 || y < 0) return;
                graph[x,y] = value;
            }
            private void RowClearChk()
            {
                bool clear = true;

                for (int j = 0; j < Column; j++) if (graph[Row - 1, j] > 0)
                        clear = false;

                if (clear)
                    for (int i = 1; i < Row; i++)
                        for (int j = 0; j < Column; j++)
                            graph[i, j] = graph[i - 1, j];
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

            GUI.DrawTexture(new Rect(10, 10, 10, 10), new Texture());


        }
    }
}
