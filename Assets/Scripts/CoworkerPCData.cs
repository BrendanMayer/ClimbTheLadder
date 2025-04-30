    using System.Collections;
    using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoworkerPCData : MonoBehaviour
{
    public string pcName;
    public List<string> files = new List<string>();
    public string ip;
    public string gateway;
    public string dns;
    public bool hasError;


}

