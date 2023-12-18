using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement.Requirements.ComponentOrder
{

[CreateAssetMenu(fileName = "Config", menuName = "MegaPint/Validators/Component Order Config", order = 1)]
public class ComponentOrderConfig : ScriptableObject
{
    [Serializable]
    public class Type
    {
        [Serializable]
        public struct CategorySettings
        {
            public CategoryFunction function;
            public string nameSpaceString;
        }

        public bool canBeModified = true;
        public bool canBeDeleted = true;
        public string componentName;
        public string tooltip;

        public bool isCategory;
        public CategorySettings category;
    }

    public enum CategoryFunction
    {
        AddCategory, NonUnityComponents, NamespaceContains, NamespaceEquals, Fill
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
            isCategory = true,
            category = {function = CategoryFunction.Fill}
        }
    };
}

}
