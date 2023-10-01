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
            componentName = "ValidatableMonoBehaviourStatus",
            tooltip = "Component to display the validation status of this gameObject"
        },
        new Type
        {
            canBeModified = false,
            componentName = "Fill",
            tooltip = "All non specified monoBehaviours will be placed here",
            function = SpecialFunction.Fill
        }
    };

    [Serializable]
    public class Type
    {
        public bool canBeModified = true;
        public string componentName;
        public string tooltip;
        public SpecialFunction function;
    }
    
    public enum SpecialFunction
    {
        None, Fill
    }
}

}
