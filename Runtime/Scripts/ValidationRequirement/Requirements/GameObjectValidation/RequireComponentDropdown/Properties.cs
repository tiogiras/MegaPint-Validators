using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation.RequireComponentDropdown
{

/// <summary> Used to store and display information about the selected component </summary>
[Serializable]
internal class Properties
{
    public string typeName;
    public string typeFullName;
    public float propertyHeight;

    public List <string> types;
}

}
