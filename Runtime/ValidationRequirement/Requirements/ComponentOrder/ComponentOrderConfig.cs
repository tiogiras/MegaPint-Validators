using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement.Requirements.ComponentOrder
{

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Component Order Config", order = 1)]
public class ComponentOrderConfig : ScriptableObject
{
    [Serializable]
    public class Type
    {
        public bool canBeModified = true;
        public bool canBeDeleted = true;
        public string componentName;
        public string tooltip;
        public SpecialFunction function;
        
        // Category Specifics
        public CategoryFunction categoryFunction;
        public bool isCategory;
        
        // Namespace Categories
        public string nameSpaceString;
    }

    public enum CategoryFunction
    {
        AddCategory, NonUnityComponents, NamespaceContains, NamespaceEquals
    }

    public enum SpecialFunction
    {
        None, Fill
    }

    public List <Type> types = new()
    {
        new Type
        {
            canBeModified = false,
            canBeDeleted = false,
            componentName = "ValidatableMonoBehaviourStatus",
            tooltip = "Component to display the validation status of this gameObject"
        },
        new Type
        {
            canBeModified = false,
            canBeDeleted = false,
            componentName = "Fill",
            tooltip = "All non specified monoBehaviours will be placed here",
            function = SpecialFunction.Fill
        }
    };
}

}
