using System;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement.Requirements.ComponentOrder;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Component Order")]
public class RequireComponentOrder : ScriptableValidationRequirement
{
    [SerializeField] private ComponentOrderConfig _config;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        Component[] components = gameObject.GetComponents <Component>();

        foreach (Component component in components)
            Debug.Log(component.GetType().Name);
    }

    #endregion
}

}
