using System;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Runtime
{

[AttributeUsage(AttributeTargets.Class)]
public class SerializeReferenceDropdownNameAttribute : PropertyAttribute
{
    public readonly Type requirementType;
    public readonly int[] menuOrder;
    public readonly string name;

    public SerializeReferenceDropdownNameAttribute(string name, Type requirementType, params int[] menuOrder)
    {
        this.name = name;
        this.menuOrder = menuOrder;
        this.requirementType = requirementType;
    }
}

}
