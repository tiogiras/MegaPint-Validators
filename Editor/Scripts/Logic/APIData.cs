#if UNITY_EDITOR
using System.Collections.Generic;

namespace MegaPint.Editor.Scripts.Logic
{

/// <summary> Holds information about the apis listed in the api tab </summary>
internal static class APIData
{
    /// <summary> Data container for any api </summary>
    public class Data
    {
        public string displayName;
        public int indentLevel;
        public List <Data> subAPIs;
        public string title;
    }

    private static readonly List <Data> s_data = new()
    {
        new Data
        {
            title = "ValidationRequirementNameAttribute",
            displayName = "ValidationRequirementNameAttribute",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new()
                {
                    title = "bool : ValidationRequirementNameAttribute.allowMultiple",
                    displayName = "allowMultiple",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "List<Type> : ValidationRequirementNameAttribute.incompatibleRequirements",
                    displayName = "incompatibleRequirements",
                    indentLevel = 1
                },
                new()
                {
                    title = "int[] : ValidationRequirementNameAttribute.menuOrder",
                    displayName = "menuOrder",
                    indentLevel = 1
                },
                new()
                {
                    title = "string : ValidationRequirementNameAttribute.name",
                    displayName = "name",
                    indentLevel = 1
                },
                new()
                {
                    title = "Type : ValidationRequirementNameAttribute.requirementType",
                    displayName = "requirementType",
                    indentLevel = 1
                }
            }
        },
        new Data
        {
            title = "ScriptableValidationRequirement",
            displayName = "ScriptableValidationRequirement",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new()
                {
                    title = "void : ScriptableValidationRequirement.AddError()",
                    displayName = "AddError()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ScriptableValidationRequirement.AddErrorIf()",
                    displayName = "AddErrorIf()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ScriptableValidationRequirement.AddErrors()",
                    displayName = "AddErrors()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ScriptableValidationRequirement.OnRequirementValidation()",
                    displayName = "OnRequirementValidation()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ScriptableValidationRequirement.Validate()",
                    displayName = "Validate()",
                    indentLevel = 1
                },
                new()
                {
                    title = "bool : ScriptableValidationRequirement.initialized",
                    displayName = "initialized",
                    indentLevel = 1
                },
                new()
                {
                    title = "Object : ScriptableValidationRequirement.objectReference",
                    displayName = "objectReference",
                    indentLevel = 1
                },
                new()
                {
                    title = "Object : ScriptableValidationRequirement.preventListHeaderIssue",
                    displayName = "preventListHeaderIssue",
                    indentLevel = 1
                },
                new()
                {
                    title = "Object : ScriptableValidationRequirement.severityOverwrite",
                    displayName = "severityOverwrite",
                    indentLevel = 1
                },
                new()
                {
                    title = "Object : ScriptableValidationRequirement.targetGameObject",
                    displayName = "targetGameObject",
                    indentLevel = 1
                },
                new()
                {
                    title = "Object : ScriptableValidationRequirement.uniqueID",
                    displayName = "uniqueID",
                    indentLevel = 1
                }
            }
        },
        new Data
        {
            title = "ValidatableMonoBehaviour",
            displayName = "ValidatableMonoBehaviour",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new()
                {
                    title = "struct : ValidatableMonoBehaviour.SettingPriority",
                    displayName = "SettingPriority",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ValidatableMonoBehaviour.BeforeValidation()",
                    displayName = "BeforeValidation()",
                    indentLevel = 1
                },
                new()
                {
                    title = "string[] : ValidatableMonoBehaviour.DefaultImports()",
                    displayName = "DefaultImports()",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "List<ValidatorSettings> : ValidatableMonoBehaviour.GetImportedSettings()",
                    displayName = "GetImportedSettings()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : ValidatableMonoBehaviour.ImportSetting(ValidatorSettings)",
                    displayName = "ImportSetting()",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "void : ValidatableMonoBehaviour.RemoveImportedSetting(ValidatorSettings)",
                    displayName = "RemoveImportedSetting()",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "List <ScriptableValidationRequirement> : ValidatableMonoBehaviour.Requirements(bool = false)",
                    displayName = "Requirements()",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "void : ValidatableMonoBehaviour.SetRequirements(List <ScriptableValidationRequirement>)",
                    displayName = "SetRequirements()",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "ValidationState : ValidatableMonoBehaviour.Validate(out List <ValidationError>)",
                    displayName = "Validate()",
                    indentLevel = 1
                },
                new()
                {
                    title = "bool : ValidatableMonoBehaviour.HasImportedSettings",
                    displayName = "HasImportedSettings",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "List <ValidatorSettings> : ValidatableMonoBehaviour.defaultSettings",
                    displayName = "defaultSettings",
                    indentLevel = 1
                },
                new()
                {
                    title = "bool : ValidatableMonoBehaviour.importedSettingsFoldout",
                    displayName = "importedSettingsFoldout",
                    indentLevel = 1
                },
                new()
                {
                    title =
                        "List <SettingPriority> : ValidatableMonoBehaviour.settingPriorities",
                    displayName = "settingPriorities",
                    indentLevel = 1
                }
            }
        },
        new Data
        {
            title = "ValidationError",
            displayName = "ValidationError",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new() {title = "ValidationError.errorName", displayName = "errorName", indentLevel = 1},
                new() {title = "ValidationError.errorText", displayName = "errorText", indentLevel = 1},
                new() {title = "ValidationError.fixAction", displayName = "fixAction", indentLevel = 1},
                new() {title = "ValidationError.gameObject", displayName = "gameObject", indentLevel = 1},
                new() {title = "ValidationError.severity", displayName = "severity", indentLevel = 1}
            }
        },
        new Data
        {
            title = "ValidationRequirementMetaData",
            displayName = "ValidationRequirementMetaData",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new()
                {
                    title = "void : ValidationRequirementMetaData.OnInitialization()",
                    displayName = "OnInitialization()",
                    indentLevel = 1
                }
            }
        },
        new Data
        {
            title = "ValidatorSettings",
            displayName = "ValidatorSettings",
            indentLevel = 0,
            subAPIs = new List <Data>
            {
                new()
                {
                    title =
                        "List <ScriptableValidationRequirement> : Requirements(bool = false)",
                    displayName = "Requirements()",
                    indentLevel = 1
                },
                new()
                {
                    title = "void : SetRequirements(List <ScriptableValidationRequirement>)",
                    displayName = "SetRequirements()",
                    indentLevel = 1
                }
            }
        },
        new Data
        {
            title = "SerializeReferenceDropdownAttribute",
            displayName = "SerializeReferenceDropdownAttribute",
            indentLevel = 0
        },
        new Data
        {
            title = "ValidationRequirementTooltipAttribute",
            displayName = "ValidationRequirementTooltipAttribute",
            indentLevel = 0
        },
        new Data {title = "ToggleableSetting", displayName = "ToggleableSetting", indentLevel = 0},
        new Data {title = "ValidationState", displayName = "ValidationState", indentLevel = 0}
    };

    #region Public Methods

    public static List <Data> Get()
    {
        return s_data;
    }

    #endregion
}

}
#endif
