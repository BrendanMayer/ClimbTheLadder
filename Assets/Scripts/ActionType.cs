using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionType : MonoBehaviour
{
    public enum ActionTypes
    {
        None,
        Printer,
    }

    public ActionTypes type;
}
