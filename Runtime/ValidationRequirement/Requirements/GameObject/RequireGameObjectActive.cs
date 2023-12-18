using System;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements.GameObject
{

[Serializable]
[SerializeReferenceDropdownName("GameObject/Active", -10, 0)]
public class RequireGameObjectActive : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("The state the gameObject should have")]
    private bool _targetState;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _targetState = true;
    }

    protected override void Validate(UnityEngine.GameObject gameObject)
    {
        var isValid = gameObject.activeSelf == _targetState;

        if (isValid)
            return;

        var errorMessage = $"GameObject should be {(_targetState ? "active" : "inactive")}";

        AddError(
            "Incorrect active state",
            errorMessage,
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(UnityEngine.GameObject gameObject)
    {
        gameObject.SetActive(_targetState);
    }

    #endregion
}

}
