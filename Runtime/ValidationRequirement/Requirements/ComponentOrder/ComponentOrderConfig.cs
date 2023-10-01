using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement.Requirements.ComponentOrder
{

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Component Order Config", order = 1)]
public class ComponentOrderConfig : ScriptableObject
{
    public List <Type> types = new()
    {
        new Type
        {
            canBeModified = false,
            canBeMoved = false,
            name = "Transform",
            componentName = "Transform"
        },
        new Type
        {
            canBeModified = true,
            canBeMoved = true,
            name = "Validation Status",
            componentName = "ValidatableMonoBehaviourStatus"
        }
};

    private void OnValidate()
    {
        // TODO enforce that transform is on top
        // TODO enforce that validationStatus cannot be removed
    }
    
    [Serializable]
    public class Type
    {
        public bool canBeModified;
        public bool canBeMoved;
        public string name;
        public string componentName;
    }
}

}
