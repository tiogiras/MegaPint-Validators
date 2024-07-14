using System;
using UnityEngine;

namespace MegaPint.SerializeReferenceDropdown.Runtime
{

[AttributeUsage(AttributeTargets.Class)]
public class ValidationRequirementNameAttribute : PropertyAttribute
{
    public readonly bool allowMultiple;
    public readonly Type[] incompatibleRequirements;
    public readonly int[] menuOrder;
    public readonly string name;
    public readonly Type requirementType;

    /// <summary> Mark the requirement to be displayed in the requirements dropdown </summary>
    /// <param name="name"> Name of the requirement </param>
    /// <param name="requirementType"> Type of the requirement class </param>
    /// <param name="menuOrder"> Order in which the requirement is displayed </param>
    public ValidationRequirementNameAttribute(string name, Type requirementType, params int[] menuOrder)
    {
        this.name = name;
        this.menuOrder = menuOrder;
        this.requirementType = requirementType;
        allowMultiple = false;
        incompatibleRequirements = null;
    }

    /// <summary> Mark the requirement to be displayed in the requirements dropdown </summary>
    /// <param name="name"> Name of the requirement </param>
    /// <param name="requirementType"> Type of the requirement class </param>
    /// <param name="allowMultiple"> If more than one of this requirement can be added to one behaviour </param>
    /// <param name="menuOrder"> Order in which the requirement is displayed </param>
    public ValidationRequirementNameAttribute(
        string name,
        Type requirementType,
        bool allowMultiple,
        params int[] menuOrder)
    {
        this.name = name;
        this.menuOrder = menuOrder;
        this.requirementType = requirementType;
        this.allowMultiple = allowMultiple;
        incompatibleRequirements = null;
    }

    /// <summary> Mark the requirement to be displayed in the requirements dropdown </summary>
    /// <param name="name"> Name of the requirement </param>
    /// <param name="requirementType"> Type of the requirement class </param>
    /// <param name="incompatibleRequirements"> List of requirement types incompatible with this requirement </param>
    /// <param name="menuOrder"> Order in which the requirement is displayed </param>
    /// <param name="allowMultiple"> If more than one of this requirement can be added to one behaviour </param>
    public ValidationRequirementNameAttribute(
        string name,
        Type requirementType,
        bool allowMultiple,
        Type[] incompatibleRequirements,
        params int[] menuOrder)
    {
        this.name = name;
        this.menuOrder = menuOrder;
        this.requirementType = requirementType;
        this.allowMultiple = allowMultiple;
        this.incompatibleRequirements = incompatibleRequirements;
    }
}

}
