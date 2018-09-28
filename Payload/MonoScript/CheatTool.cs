using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class CheatTool
    {
        public enum ExecuteType
        {
            Once,EachUpdate,EachFixUpdate
        }

        public delegate void Modifier(object original);

        public KeyCode ActiveKey = KeyCode.None;
        public ExecuteType ExecuteWay;
        public string Path = string.Empty;
        public string MonoName = string.Empty;
        public string VarName = string.Empty;
        public Modifier modifier;

        public CheatTool
            (KeyCode activeKey,
            string path,
            string monoName,
            string varName,
            Modifier modifier, 
            ExecuteType executeType = ExecuteType.Once)
        {
            this.ActiveKey = activeKey;
            this.Path = path;
            this.MonoName = monoName;
            this.VarName = varName;
            this.modifier = modifier;
            this.ExecuteWay = executeType;
        }
    }

    public class CheatToolExecuter : MonoBehaviour
    {
        private List<CheatTool> cheatToolList = new List<CheatTool>();


    }
}
