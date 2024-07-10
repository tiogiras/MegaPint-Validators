using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

[Serializable]
[SerializeReferenceDropdownName("GameObject/Layer", typeof(RequireLayer), -30, 1)]
public class RequireLayer : ScriptableValidationRequirement
{
    [SerializeField] private string _layerName;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        if (string.IsNullOrEmpty(_layerName))
            return;
        
        if (gameObject.layer != LayerMask.NameToLayer(_layerName))
        {
            AddError(
                "Incorrect Layer",
                $"Expected layer {_layerName}, but found {LayerMask.LayerToName(gameObject.layer)}",
                ValidationState.Error,
                FixAction);
        }
    }

    #endregion

    #region Private Methods

    private void FixAction(GameObject gameObject)
    {
        var layer = LayerMask.NameToLayer(_layerName);

        if (layer == -1)
        {
            Debug.LogWarning("Could not set the gameObject layer, as the targeted layer does not exist.");

            return;
        }

        gameObject.layer = layer;
    }

    #endregion
}

}
