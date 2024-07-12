using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using MegaPint.ValidationRequirement.Requirements;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint
{

/// <summary> MonoBehaviour that extends to be validatable via <see cref="ScriptableValidationRequirement" /> </summary>
[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    public bool HasImportedSettings => _importedSettings.Count > 0;

    [SerializeField] private List<ValidatorSettings> _importedSettings;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <ScriptableValidationRequirement> _requirements;
    
    private ValidatableMonoBehaviourStatus _status;

    private List <ScriptableValidationRequirement> _activeRequirements = new();

    [HideInInspector] public bool importedSettingsFoldout;
    
    public List <ScriptableValidationRequirement> ActiveRequirements
    {
        get
        {
            _activeRequirements.Clear();
            _activeRequirements.AddRange(Requirements(true));

            if (_importedSettings.Count == 0)
                return _activeRequirements;

            IEnumerable <ValidatorSettings> importedSettings = _importedSettings.Where(setting => setting != null);

            importedSettings = importedSettings.OrderBy(GetSettingPriority);

            foreach (ValidatorSettings setting in importedSettings)
            {
                if (setting.Requirements().Count == 0)
                    continue;
            
                _activeRequirements.AddRange(ScriptableValidationRequirement.GetCompatibleRequirements(_activeRequirements, setting.Requirements()));
            }

            return _activeRequirements;
        }
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

        if (_requirements.Count > 0)
        {
            foreach (IValidationRequirement requirement in _requirements)
                requirement?.OnValidate(this);   
        }

        if (ActiveRequirements is not {Count: > 0})
            return;

        _status.ValidateStatus();
    }

    #endregion

    #region Public Methods

    /// <summary> Get all non imported requirements on this gameObject </summary>
    /// <returns> All defined requirements </returns>
    public List <ScriptableValidationRequirement> Requirements(bool excludeNulls = false)
    {
        return excludeNulls ? _requirements.Where(requirement => requirement != null).ToList() : _requirements;
    }

    /// <summary> Set the imported settings </summary>
    /// <param name="setting"> New imported settings </param>
    public void ImportSetting(ValidatorSettings setting)
    {
        if (_importedSettings.Contains(setting))
        {
            Debug.LogWarning("ValidatorSettings already imported.");
            return;   
        }

        _importedSettings.Add(setting);
        
        settingPriorities.Add(new SettingPriority
        {
            priority = settingPriorities.Count + 1,
            setting = setting
        });

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    
    /// <summary> Get the imported settings </summary>
    /// <returns> The imported settings of the behaviour </returns>
    public List<ValidatorSettings> GetImportedSettings()
    {
        _importedSettings = _importedSettings.Where(setting => setting != null).ToList();
        
        return _importedSettings;
    }

    /// <summary> Validate this gameObject based on the set <see cref="ScriptableValidationRequirement" /> </summary>
    /// <param name="errors"> Found errors </param>
    /// <returns> Validation state after validation </returns>
    public ValidationState Validate(out List <ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <ValidationError>();

        if (ActiveRequirements is not {Count: > 0})
            return state;

        foreach (IValidationRequirement requirement in ActiveRequirements.Where(requirement => requirement != null))
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
        return ActiveRequirements.Any(requirement => requirement?.GetType() == typeof(RequireChildrenValidation));
    }

    #endregion

    #region Protected Methods

    /// <summary> Called before validating </summary>
    protected virtual void BeforeValidation()
    {
    }

    /// <summary> Overwrite the requirements </summary>
    /// <param name="requirements"> new requirements </param>
    protected void SetRequirements(List <ScriptableValidationRequirement> requirements)
    {
        _requirements = requirements;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    #endregion

    public void RemoveImportedSetting(ValidatorSettings setting)
    {
        _importedSettings.Remove(setting);

        var index = -1;

        foreach (SettingPriority settingPriority in settingPriorities.Where(settingPriority => settingPriority.setting == setting))
        {
            index = settingPriorities.IndexOf(settingPriority);
            break;
        }
        
        settingPriorities.RemoveAt(index);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    [Serializable]
    public struct SettingPriority
    {
        public ValidatorSettings setting;
        public int priority;
    }

    [HideInInspector] public List <SettingPriority> settingPriorities;

    public int GetSettingPriority(ValidatorSettings setting)
    {
        return settingPriorities.FirstOrDefault(s => s.setting == setting).priority;
    }

    public string[] GetPriorityOptions(out List <int> values)
    {
        values = settingPriorities.Select(s => s.priority).ToList();
        values.Sort();

        return values.Select(value => value.ToString()).ToArray();
    }

    public void SetSettingPriority(ValidatorSettings setting, int newPriority)
    {
        var oldPriority = settingPriorities.FirstOrDefault(s => s.setting == setting).priority;

        for (var i = 0; i < settingPriorities.Count; i++)
        {
            SettingPriority settingPriority = settingPriorities[i];

            if (settingPriority.setting == setting)
            {
                settingPriority.priority = newPriority;
                settingPriorities[i] = settingPriority;
                continue;
            }

            if (settingPriority.priority != newPriority)
                continue;

            settingPriority.priority = oldPriority;
            settingPriorities[i] = settingPriority;
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

}
