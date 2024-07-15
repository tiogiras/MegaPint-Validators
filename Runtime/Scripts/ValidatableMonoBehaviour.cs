using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using MegaPint.ValidationRequirement.Requirements;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: InternalsVisibleTo("tiogiras.megapint.editor")]
[assembly: InternalsVisibleTo("tiogiras.megapint.validators.editor")]
[assembly: InternalsVisibleTo("tiogiras.megapint.validators.runtime.serializereferencedropdown.editor")]

namespace MegaPint
{

/// <summary> MonoBehaviour that extends to be validatable via <see cref="ScriptableValidationRequirement" /> </summary>
[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    [Serializable]
    public struct SettingPriority
    {
        public ValidatorSettings setting;
        public int priority;
    }

    public bool HasImportedSettings => _importedSettings?.Count > 0 || defaultSettings?.Count > 0;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <ScriptableValidationRequirement> _requirements = new();

    [HideInInspector] public List <ValidatorSettings> defaultSettings = new();
    [SerializeField] private List <ValidatorSettings> _importedSettings = new();

    // Used to store the toggle state of the imported settings in the gui
    [HideInInspector] public bool importedSettingsFoldout;
    [HideInInspector] public List <SettingPriority> settingPriorities = new();

    private readonly List <ScriptableValidationRequirement> _activeRequirements = new();
    private ValidatableMonoBehaviourStatus _status;

    #region Unity Event Functions

    internal void OnValidate()
    {
        TryImportDefaultImports();

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
            foreach (ScriptableValidationRequirement requirement in _requirements)
                requirement?.OnValidate(this);
        }

        if (GetActiveRequirements() is not {Count: > 0})
            return;

        _status.ValidateStatus();
    }

    #endregion

    #region Public Methods

    /// <summary> Get the imported settings </summary>
    /// <returns> The imported settings of the behaviour </returns>
    public List <ValidatorSettings> GetImportedSettings()
    {
        _importedSettings = _importedSettings.Where(setting => setting != null).ToList();

        return _importedSettings;
    }

    /// <summary> Set the imported settings </summary>
    /// <param name="setting"> New imported settings </param>
    public void ImportSetting(ValidatorSettings setting)
    {
        if (_importedSettings.Contains(setting) || defaultSettings.Contains(setting))
        {
            Debug.LogWarning("ValidatorSettings already imported.");

            return;
        }

        _importedSettings.Add(setting);

        settingPriorities.Add(new SettingPriority {priority = settingPriorities.Count + 1, setting = setting});

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    /// <summary> Remove an imported setting </summary>
    /// <param name="setting"> Target setting </param>
    public void RemoveImportedSetting(ValidatorSettings setting)
    {
        _importedSettings.Remove(setting);

        var index = -1;

        foreach (SettingPriority settingPriority in settingPriorities.Where(
                     settingPriority => settingPriority.setting == setting))
        {
            index = settingPriorities.IndexOf(settingPriority);

            break;
        }

        var oldPriority = settingPriorities[index].priority;

        settingPriorities.RemoveAt(index);

        for (var i = 0; i < settingPriorities.Count; i++)
        {
            SettingPriority settingPriority = settingPriorities[i];

            if (settingPriority.priority < oldPriority)
                continue;

            settingPriority.priority--;
            settingPriorities[i] = settingPriority;
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    /// <summary> Get all non imported requirements on this gameObject </summary>
    /// <returns> All defined requirements </returns>
    public List <ScriptableValidationRequirement> Requirements(bool excludeNulls = false)
    {
        _requirements ??= new List <ScriptableValidationRequirement>();

        return excludeNulls ? _requirements.Where(requirement => requirement != null).ToList() : _requirements;
    }

    /// <summary> Validate this gameObject based on the set <see cref="ScriptableValidationRequirement" /> </summary>
    /// <param name="errors"> Found errors </param>
    /// <returns> Validation state after validation </returns>
    public ValidationState Validate(out List <ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <ValidationError>();

        if (GetActiveRequirements() is not {Count: > 0})
            return state;

        foreach (ScriptableValidationRequirement requirement in
                 GetActiveRequirements().Where(requirement => requirement != null))
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

    #endregion

    #region Protected Methods

    /// <summary> Called before validating </summary>
    protected virtual void BeforeValidation()
    {
    }

    protected virtual string[] DefaultImports()
    {
        return null;
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

    #region Internal Methods

    /// <summary> All requirements that contribute to the validation process of the behaviour </summary>
    internal List <ScriptableValidationRequirement> GetActiveRequirements()
    {
        _activeRequirements.Clear();
        _activeRequirements.AddRange(Requirements(true));

        if (_importedSettings.Count == 0 && defaultSettings.Count == 0)
            return _activeRequirements;

        List <ValidatorSettings> importedSettings = _importedSettings.Where(setting => setting != null).ToList();
        importedSettings.AddRange(defaultSettings.Where(setting => setting != null));

        foreach (ValidatorSettings setting in importedSettings.OrderBy(GetSettingPriority))
        {
            if (setting.Requirements(true).Count == 0)
                continue;

            _activeRequirements.AddRange(
                ScriptableValidationRequirement.GetCompatibleRequirements(
                    _activeRequirements,
                    setting.Requirements()));
        }

        return _activeRequirements;
    }

    /// <summary> Get the possible priorities for the imported settings </summary>
    /// <param name="values"> Possible priorities as values </param>
    /// <returns> Possible priorities as options </returns>
    internal string[] GetPriorityOptions(out List <int> values)
    {
        values = settingPriorities.Select(s => s.priority).ToList();
        values.Sort();

        return values.Select(value => value.ToString()).ToArray();
    }

    /// <summary> Get priority of the targeted setting </summary>
    /// <param name="setting"> Targeted setting </param>
    /// <returns> Priority of the setting </returns>
    internal int GetSettingPriority(ValidatorSettings setting)
    {
        if (defaultSettings.Contains(setting))
            return defaultSettings.IndexOf(setting) - 999;

        return settingPriorities.FirstOrDefault(s => s.setting == setting).priority;
    }

    /// <summary> Set the priority of a setting </summary>
    /// <param name="setting"> Targeted setting </param>
    /// <param name="newPriority"> New priority value </param>
    internal void SetSettingPriority(ValidatorSettings setting, int newPriority)
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

    /// <summary> Check if the requirements include <see cref="RequireChildrenValidation" /> </summary>
    /// <returns> If the requirement is set on this gameObject </returns>
    internal bool ValidatesChildren()
    {
        return GetActiveRequirements().Any(requirement => requirement?.GetType() == typeof(RequireChildrenValidation));
    }

    #endregion

    #region Private Methods

    /// <summary> Import the specified default settings </summary>
    private void ImportDefaultImports()
    {
        defaultSettings ??= new List <ValidatorSettings>();

        defaultSettings.Clear();

        foreach (var import in DefaultImports())
        {
            var setting = AssetDatabase.LoadAssetAtPath <ValidatorSettings>(import);

            if (setting is null)
                continue;

            defaultSettings.Add(setting);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    /// <summary> Try to import the specified default settings </summary>
    private void TryImportDefaultImports()
    {
        var defaultImports = DefaultImports();

        if (defaultImports is null or {Length: <= 0})
        {
            defaultSettings.Clear();

            return;
        }

        if (defaultSettings is not {Count: > 0})
        {
            ImportDefaultImports();

            return;
        }

        var nonNullImports = defaultImports.Select(Path.GetFileNameWithoutExtension).
                                            Where(path => !string.IsNullOrEmpty(path)).
                                            ToArray();

        if (!nonNullImports.Any())
        {
            defaultSettings.Clear();

            return;
        }

        if (defaultSettings.Count != nonNullImports.Length)
        {
            ImportDefaultImports();

            return;
        }

        if (nonNullImports.Where((import, i) => !defaultSettings[i].name.Equals(import)).Any())
            ImportDefaultImports();
    }

    #endregion
}

}
