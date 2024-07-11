using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using MegaPint.ValidationRequirement.Requirements;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint
{

/// <summary> MonoBehaviour that extends to be validatable via <see cref="ScriptableValidationRequirement" /> </summary>
[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    public bool HasImportedSettings => _importedSettings != null;

    private List <IValidationRequirement> _activeRequirements =>
        _importedSettings == null ? _requirements : _importedSettings.Requirements();

    [SerializeField] private ValidatorSettings _importedSettings;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;
    
    private ValidatableMonoBehaviourStatus _status;

    #region Unity Event Functions
    
    public void OnValidate()
    {
        BeforeValidation();

        if (_status == null)
        {
            _status = TryGetComponent(out ValidatableMonoBehaviourStatus status)
                ? status
                : gameObject.AddComponent <ValidatableMonoBehaviourStatus>();

            _status.AddValidatableMonoBehaviour(this);
        }

        if (_activeRequirements is not {Count: > 0})
            return;

        if (_importedSettings == null)
        {
            foreach (IValidationRequirement requirement in _activeRequirements)
                requirement?.OnValidate(this);
        }

        _status.ValidateStatus();
    }

    #endregion

    #region Public Methods

    /// <summary> Get all requirements on this gameObject </summary>
    /// <returns> All defined requirements </returns>
    public List <IValidationRequirement> Requirements()
    {
        return _requirements;
    }

    /// <summary> Set the imported settings </summary>
    /// <param name="settings"> New imported settings </param>
    public void SetImportedSettings(ValidatorSettings settings)
    {
        _importedSettings = settings;
    }
    
    /// <summary> Get the imported settings </summary>
    /// <returns> The imported settings of the behaviour </returns>
    public ValidatorSettings GetImportedSettings()
    {
        return _importedSettings;
    }

    /// <summary> Validate this gameObject based on the set <see cref="ScriptableValidationRequirement" /> </summary>
    /// <param name="errors"> Found errors </param>
    /// <returns> Validation state after validation </returns>
    public ValidationState Validate(out List <ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <ValidationError>();

        if (_activeRequirements is not {Count: > 0})
            return state;

        foreach (IValidationRequirement requirement in _activeRequirements.Where(requirement => requirement != null))
        {
            ValidationState requirementState = requirement.Validate(
                gameObject,
                out List <ValidationError> additionalErrors);

            if (additionalErrors.Count > 0)
                errors.AddRange(additionalErrors);

            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }

    /// <summary> Check if the requirements include <see cref="RequireChildrenValidation" /> </summary>
    /// <returns> If the requirement is set on this gameObject </returns>
    public bool ValidatesChildren()
    {
        return _activeRequirements.Any(requirement => requirement?.GetType() == typeof(RequireChildrenValidation));
    }

    #endregion

    #region Protected Methods

    /// <summary> Called before validating </summary>
    protected virtual void BeforeValidation()
    {
    }

    /// <summary> Overwrite the requirements </summary>
    /// <param name="requirements"> new requirements </param>
    protected void SetRequirements(List <IValidationRequirement> requirements)
    {
        _requirements = requirements;
    }

    #endregion
}

}
