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
            public static int[,] GetnerateShape(int type)
            {
                switch (type)
                {
                    case -1:
                        return new int[4, 4]
                        {
                            {1, 1, 1, 1 },
                            {1, 1, 1, 1 },
                            {1, 1, 1, 1 },
                            {1, 1, 1, 1 },
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
            public int Row { get; private set; } = 24;
            public int Column { get; private set; } = 10;

            private int mvobjrow = 0;
            private int mvobjcolumn = 0;
            private int[,] mvobj = new int[4, 4];

            public Graph(int row = 24, int column = 10)
            {
                Row = row; Column = column;
                graph = new int[row, column];
            }
            public int GetPoint(int x, int y)
            {
                if (x >= Row || y >= Column || x < 0 || y < 0)
                    return -1;

                if ((x > mvobjrow && x < mvobjrow + 4) && (y > mvobjcolumn && y < mvobjcolumn + 4))
                    return (graph[x, y] == 1 || mvobj[x - mvobjrow, y - mvobjcolumn] == 1) ? 1 : 0;

                return graph[x, y];
            }

            /// <summary>
            /// 逻辑帧
            /// </summary>
            public void UpdateLogic()
            {
                RowClearChk();
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
            /// 旋转obj90度，检查碰撞
            /// 左右碰撞移动，上下碰撞阻止
            /// </summary>
            private void ObjRotate()
            {
                
            }
            /// <summary>
            /// 左右移动obj一列，检查碰撞
            /// </summary>
            private void MoveObjACol(Command cmd)
            {
                if(cmd == Command.Left)
                {

                }
                if(cmd == Command.Right)
                {

                }

            }
            /// <summary>
            /// 向下移动obj一行，检查碰撞，是就移到graph
            /// </summary>
            private void MoveObjARow()
            {
                


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
                                graph[i, j] = 0;
                            }
                        }
                        else
                        {
                            for (int ii = 1; ii < i; ii++)
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

            
        }













    }
}
