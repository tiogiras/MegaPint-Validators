using System;
using UnityEngine;

public struct ValidationError
{
    public Action <GameObject> fixAction;
    public ValidationState severity;
    public GameObject gameObject;
    public string errorName;
    public string errorText;
}