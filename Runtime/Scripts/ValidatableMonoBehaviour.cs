using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using MegaPint.ValidationRequirement.Requirements;
using UnityEngine;

namespace MegaPint
{

/// <summary> MonoBehaviour that extends to be validatable via <see cref="ScriptableValidationRequirement"/> </summary>
[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    public bool HasImportedSettings => _importedSettings != null;

    private List <IValidationRequirement> _activeRequirements =>
        _importedSettings == null ? _requirements : _importedSettings.Requirements();

    [SerializeField] private ValidatorSettings _importedSettings;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;

    //[HideInInspector] // TODO reenable
    public List <string> initializedRequirements;

    private ValidatableMonoBehaviourStatus _status;

    // TODO commenting
    public bool IsInitialized(IValidationRequirement requirement)
    {
        initializedRequirements ??= new List <string>();

        return initializedRequirements.Contains(requirement.GetType().ToString());
    }

    // TODO commenting
    public void OnRequirementInitialization(IValidationRequirement requirement)
    {
        initializedRequirements ??= new List <string>();
        
        if (!initializedRequirements.Contains(requirement.GetType().ToString()))
            initializedRequirements.Add(requirement.GetType().ToString());
    }
    
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

        foreach (IValidationRequirement requirement in _activeRequirements)
        {
            requirement?.OnValidate(this);
        }

        List <string> cleanedInitializedRequirements = (from requirement in _requirements
                                                        where requirement != null
                                                        select requirement.GetType().ToString()
                                                        into typeName
                                                        where initializedRequirements.Contains(typeName)
                                                        select initializedRequirements[
                                                            initializedRequirements.IndexOf(typeName)]).ToList();

        initializedRequirements = cleanedInitializedRequirements;

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

    /// <summary> Validate this gameObject based on the set <see cref="ScriptableValidationRequirement"/> </summary>
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

    /// <summary> Check if the requirements include <see cref="RequireChildrenValidation"/> </summary>
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
