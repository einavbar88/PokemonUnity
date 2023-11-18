using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Dialog 
{
    [SerializeField] List<string> script;

    public List<string> Script { get { return script; } }
}
