using UnityEngine;

namespace Payload.MonoScript
{
    public class Tetris : MonoBehaviour
    {
        public static class Shape
        {
            public static int[,] GetnerateShape(int type)
            {
                switch (type)
                {
                    case -1:
                        return new int[4, 4]
                        {
                            {0, 1, 1, 0 },
                            {0, 1, 1, 0 },
                            {0, 1, 1, 0 },
                            {0, 1, 1, 0 },
                        };
                    case 0:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 0, 1, 0 },
                            { 0, 0, 1, 0 },
                            { 0, 1, 1, 0 },
                        };

                    case 1:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 1, 1, 1, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case 2:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 1, 1, 0 },
                            { 0, 0, 1, 1 },
                            { 0, 0, 0, 0 },
                        };
                    case 3:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 1, 1, 0 },
                            { 1, 1, 0, 0 },
                            { 0, 0, 0, 0 },
                        };
                    case 4:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 1, 1, 0 },
                        };
                    case 5:
                        return new int[4, 4]
                        {
                            { 0, 1, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 1, 0, 0 },
                        };
                    case 6:
                        return new int[4, 4]
                        {
                            { 0, 0, 0, 0 },
                            { 0, 1, 1, 0 },
                            { 0, 1, 1, 0 },
                            { 0, 0, 0, 0 },
                        };
                    default: return null;
                }
            }

        }
        public class Graph
        {

            public enum Command { Left, Right, Rotate }
            //标准俄罗斯方块棋盘大小：10x24
            private int[,] graph = new int[24, 10];
            public int Row { get; private set; }
            public int Column { get; private set; }

            private int mvobjrow = 0;
            private int mvobjcolumn = 0;
            private int[,] mvobj = null;

            public Graph(int row = 24, int column = 10)
            {
                Row = row; Column = column;
                graph = new int[row, column];
            }
            public int GetPoint(int x, int y)
            {
                if (x >= Row || y >= Column || x < 0 || y < 0)
                    return -1;

                if ((x >= mvobjrow && x < mvobjrow + 4) && (y >= mvobjcolumn && y < mvobjcolumn + 4))
                {
                    if (graph[x, y] > 0) return graph[x, y];

                    if (mvobj != null)
                    {
                        int or = x - mvobjrow;
                        int oc = y - mvobjcolumn;

                        if (mvobj[or, oc] > 0)
                        {
                            return mvobj[or, oc];
                        }
                    }
                }
                return graph[x, y];
            }

            /// <summary>
            /// 逻辑帧
            /// </summary>
            public void UpdateLogic()
            {
                RowClearChk();
                if (RowDeathChk()) Clear();
                if (mvobj == null) ObjGen();
                MoveObjARow();
            }
            /// <summary>
            /// 命令帧
            /// </summary>
            public void CommandUpdateLogic(Command cmd)
            {
                if (cmd == Command.Rotate) ObjRotate();
                else MoveObjACol(cmd);
            }


            /// <summary>
            /// 清理盘面
            /// </summary>
            public void Clear()
            {
                for (int i = 0; i < Row; i++)
                    for (int j = 0; j < Column; j++)
                        graph[i, j] = 0;

                mvobjrow = 0;
                mvobjcolumn = 0;
            }
            /// <summary>
            /// 产生一个Obj
            /// </summary>
            private void ObjGen()
            {
                if (mvobj != null) return;
                mvobj = Shape.GetnerateShape(Random.Range(0, 6));
                mvobjrow = 0;
                mvobjcolumn = Column / 2;
            }
            /// <summary>
            /// 旋转obj90度
            /// </summary>
            private void ObjRotate()
            {
                int[,] newmvobj = new int[4, 4];

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        newmvobj[i, j] = mvobj[j, 3 - i];
                    }
                }
                if (!IsHit(newmvobj, mvobjrow, mvobjcolumn)) mvobj = newmvobj;
            }
            /// <summary>
            /// 左右移动obj一列
            /// </summary>
            private void MoveObjACol(Command cmd)
            {
                if (cmd == Command.Left)
                {
                    if (!IsHit(mvobj, mvobjrow, mvobjcolumn - 1)) mvobjcolumn--;
                }
                if (cmd == Command.Right)
                {
                    if (!IsHit(mvobj, mvobjrow, mvobjcolumn + 1)) mvobjcolumn++;
                }
            }
            /// <summary>
            /// 向下移动obj一行
            /// </summary>
            private void MoveObjARow()
            {
                if (mvobj == null) return;
                if (IsHit(mvobj, mvobjrow + 1, mvobjcolumn)) PutMvobjInGraph();
                else mvobjrow++;
            }
            /// <summary>
            /// 判断graph的任意一行是否是可消的，并消除之
            /// </summary>
            private void RowClearChk()
            {
                for (int i = Row - 1; i >= 0; i--)
                {
                    bool clearthisrow = true;

                    for (int j = 0; j < Column; j++)
                    {
                        if (graph[i, j] <= 0)
                        {
                            clearthisrow = false;
                            break;
                        }
                    }


                    if (clearthisrow)
                    {
                        if (i == 0)
                        {
                            for (int j = 0; j < Column; j++)
                            {
                                graph[0, j] = 0;
                            }
                        }
                        else
                        {
                            for (int ii = i; ii > 0; ii--)
                            {
                                for (int j = 0; j < Column; j++)
                                {
                                    graph[ii, j] = graph[ii - 1, j];
                                }
                            }

                        }
                    }
                }
            }
            private bool RowDeathChk()
            {
                for (int j = 0; j < Column; j++)
                {
                    if (graph[0, j] > 0) return true;
                }
                return false;
            }
            /// <summary>
            /// 检查碰撞
            /// </summary>
            /// <param name="newobjrow"></param>
            /// <param name="newobjcolumn"></param>
            /// <returns></returns>
            private bool IsHit(int[,] newmvobj, int newobjrow, int newobjcolumn)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (newmvobj != null && newmvobj[i, j] > 0)
                        {
                            if (i + newobjrow >= Row || i + newobjrow < 0) return true;
                            if (j + newobjcolumn >= Column || j + newobjcolumn < 0) return true;
                            if (graph[i + newobjrow, j + newobjcolumn] > 0) return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 把当前的mvobj放入Graph数组并清除mvobj
            /// </summary>
            /// <returns></returns>
            private void PutMvobjInGraph()
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (mvobj[i, j] > 0)
                        {
                            graph[i + mvobjrow, j + mvobjcolumn] = mvobj[i, j];
                        }
                    }
                }
                mvobj = null;
            }
        }


        private Graph graph = new Graph();
        public float UpdateLogicDT = 0.8f;
        private float tcounter = 0f;

        private void Update()
        {
            tcounter += Time.deltaTime;
            if (tcounter > (Input.GetKey(KeyCode.S) ? UpdateLogicDT / 5f : UpdateLogicDT))
            {
                tcounter = 0f;
                graph.UpdateLogic();
            }

            if (Input.GetKeyDown(KeyCode.A)) graph.CommandUpdateLogic(Graph.Command.Left);
            if (Input.GetKeyDown(KeyCode.D)) graph.CommandUpdateLogic(Graph.Command.Right);
            if (Input.GetKeyDown(KeyCode.Space)) graph.CommandUpdateLogic(Graph.Command.Rotate);
        }


        private void OnGUI()
        {
            float squaresz = Screen.height / (graph.Row > graph.Column ? graph.Row : graph.Column);
            float X = squaresz * graph.Row / 2f;

            for (int i = 0; i < graph.Row; i++)
            {
                for (int j = 0; j < graph.Column; j++)
                {
                    if (graph.GetPoint(i, j) > 0) GUI.Button(new Rect(squaresz * j + X, squaresz * i, squaresz, squaresz), "");
                }
            }
            GUI.Button(new Rect(X - squaresz, 0, squaresz, Screen.height), "");
            GUI.Button(new Rect(X + graph.Column * squaresz, 0, squaresz, Screen.height), "");

        }
    }
}
