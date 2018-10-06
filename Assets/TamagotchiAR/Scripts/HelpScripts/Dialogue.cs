using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue{

    // Use this for initialization
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
}
