using System;
using System.Collections.Generic;
using UnityEngine;

public struct InvalidBehaviour
{
    public string behaviourName;
    public List <ValidationError> errors;
    
    public struct ValidationError
    {
        public Action <GameObject> fixAction;
        public ValidationState severity;
        public GameObject gameObject;
        public string errorName;
        public string errorText;
    }
}