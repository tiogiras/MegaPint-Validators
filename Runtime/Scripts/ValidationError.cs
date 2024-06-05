using System;
using UnityEngine;

namespace MegaPint
{

/// <summary> Contains info about an occured error </summary>
public struct ValidationError
{
    public Action <GameObject> fixAction;
    public ValidationState severity;
    public GameObject gameObject;
    public string errorName;
    public string errorText;
}

}
