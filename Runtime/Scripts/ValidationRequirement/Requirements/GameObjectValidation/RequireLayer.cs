using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Requirement requires a specific layer on the gameObject </summary>
[Serializable]
[ValidationRequirementTooltip("This requirement enforces the layer of the gameObject.")]
[ValidationRequirement("GameObject/Layer", typeof(RequireLayer), -30, 1)]
internal class RequireLayer : ScriptableValidationRequirement
{
    [SerializeField] private string _layerName;

    public RequireLayer()
    {
    }

    public RequireLayer(string layerName)
    {
        _layerName = layerName;
    }

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        if (string.IsNullOrEmpty(_layerName))
            return;

        AddErrorIf(
            gameObject.layer != LayerMask.NameToLayer(_layerName),
            "Incorrect Layer",
            $"Expected layer {_layerName}, but found {LayerMask.LayerToName(gameObject.layer)}",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    /// <summary> Set the layer of the gameObject </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
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
