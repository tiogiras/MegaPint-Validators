using System;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Runtime
{

[AttributeUsage(AttributeTargets.Class)]
public class TypeTooltipAttribute : TooltipAttribute
{
    public TypeTooltipAttribute(string tooltip) : base(tooltip)
    {
    }
}

}
