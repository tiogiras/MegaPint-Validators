using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;
using ValidationRequirement.Requirements;

[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    public bool HasImportedSettings => _importedSettings != null;

    private List <IValidationRequirement> _activeRequirements => _importedSettings == null ? _requirements : _importedSettings.Requirements();

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
            _status = GetComponent <ValidatableMonoBehaviourStatus>();
            _status.AddValidatableMonoBehaviour(this);
        }

        if (_activeRequirements is not {Count: > 0})
            return;

        foreach (IValidationRequirement requirement in _activeRequirements)
            requirement?.OnValidate();

        _status.ValidateStatus();
    }

    #endregion

    #region Public Methods

    public List <IValidationRequirement> Requirements()
    {
        return _requirements;
    }

    protected void SetRequirements(List <IValidationRequirement> requirements)
    {
        _requirements = requirements;
    }

    public void SetImportedSettings(ValidatorSettings settings)
    {
        _importedSettings = settings;
    }

    public ValidationState Validate(out List <ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <ValidationError>();

        if (_activeRequirements is not {Count: > 0})
            return state;

        foreach (IValidationRequirement requirement in _activeRequirements.Where(requirement => requirement != null))
        {
            ValidationState requirementState = requirement.Validate(gameObject, out List <ValidationError> additionalErrors);

            if (additionalErrors.Count > 0)
                errors.AddRange(additionalErrors);

            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }

    public bool ValidatesChildren()
    {
        return _activeRequirements.Any(requirement => requirement.GetType() == typeof(RequireChildrenValidation));
    }

    #endregion

    protected virtual void BeforeValidation()
    {
        
    }
}
