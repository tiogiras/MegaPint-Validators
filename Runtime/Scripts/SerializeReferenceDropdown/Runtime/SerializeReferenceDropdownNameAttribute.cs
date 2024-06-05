using System;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Runtime
{

[AttributeUsage(AttributeTargets.Class)]
public class SerializeReferenceDropdownNameAttribute : PropertyAttribute
{
    public readonly int[] menuOrder;
    public readonly string name;

    public SerializeReferenceDropdownNameAttribute(string name, params int[] menuOrder)
    {
        this.name = name;
        this.menuOrder = menuOrder;
    }
}

}
