using System;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Runtime
{

[AttributeUsage(AttributeTargets.Class)]
public class ValidationRequirementTooltipAttribute : TooltipAttribute
{
    public ValidationRequirementTooltipAttribute(string tooltip) : base(tooltip)
    {
    }
}

}
