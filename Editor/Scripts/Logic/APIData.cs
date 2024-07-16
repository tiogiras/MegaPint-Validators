#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Logic
{

/// <summary> Holds information about the apis listed in the api tab </summary>
internal static class APIData
{
    /// <summary> Data container for any api </summary>
    public class Data
    {
        public readonly string assembly;
        public readonly string description;
        public readonly string displayName;

        public readonly int indentLevel;
        public readonly DataKey key;
        public readonly List <Data> subAPIs;
        public readonly string title;

        public Data(
            string title,
            string displayName,
            string description,
            string assembly,
            int indentLevel,
            DataKey key = DataKey.Undefined,
            List <Data> subAPIs = null)
        {
            this.title = title;
            this.displayName = displayName;
            this.description = description;
            this.assembly = assembly;
            this.indentLevel = indentLevel;
            this.key = key;
            this.subAPIs = subAPIs;
        }
    }

    // ReSharper disable InconsistentNaming
    public enum DataKey
    {
        Undefined,
        VRAttr, VRAttr0, VRAttr1, VRAttr2, VRAttr3, VRAttr4,
        SVR, SVR0, SVR1, SVR2, SVR3, SVR4, SVR5, SVR6, SVR7, SVR8, SVR9, SVR10,
        VMB, VMB0, VMB1, VMB2, VMB3, VMB4, VMB5, VMB6, VMB7, VMB8, VMB9, VMB10, VMB11, VMB12,
        VS,
        VErr, VErr0, VErr1, VErr2, VErr3, VErr4,
    }

    // ReSharper restore InconsistentNaming

    private static string s_validationRequirementNameAttribute;
    private static string s_validationRequirementNameAttributeAssembly;

    private static string s_validationRequirementMetaData;
    private static string s_validationRequirementMetaDataAssembly;

    private static string s_validatableMonoBehaviour;
    private static string s_validatableMonoBehaviourAssembly;

    private static string s_scriptableValidationRequirement;
    private static string s_scriptableValidationRequirementAssembly;
    
    private static string s_validationError;
    private static string s_validationErrorAssembly;

    private static readonly List <Data> s_data = new()
    {
        new Data(
            _ValidationRequirementNameAttribute,
            _ValidationRequirementNameAttribute,
            $"Class in {typeof(ValidationRequirementAttribute).Namespace}\nInherits from {nameof(PropertyAttribute)}",
            _ValidationRequirementNameAttributeAssembly,
            0,
            DataKey.VRAttr,
            new List <Data>
            {
                new(
                    $"<link={DataKey.VRAttr}>{_ValidationRequirementNameAttribute}</link>.allowMultiple",
                    "allowMultiple",
                    "Type of boolean",
                    _ValidationRequirementNameAttributeAssembly,
                    1,
                    DataKey.VRAttr0),
                new(
                    $"<link={DataKey.VRAttr}>{_ValidationRequirementNameAttribute}</link>.incompatibleRequirements",
                    "incompatibleRequirements",
                    "Type of List<Type>",
                    _ValidationRequirementNameAttributeAssembly,
                    1,
                    DataKey.VRAttr1),
                new(
                    $"<link={DataKey.VRAttr}>{_ValidationRequirementNameAttribute}</link>.menuOrder",
                    "menuOrder",
                    "Type of int[] (params)",
                    _ValidationRequirementNameAttributeAssembly,
                    1,
                    DataKey.VRAttr2),
                new(
                    $"<link={DataKey.VRAttr}>{_ValidationRequirementNameAttribute}</link>.name",
                    "name",
                    "Type of string",
                    _ValidationRequirementNameAttributeAssembly,
                    1,
                    DataKey.VRAttr3),
                new(
                    $"<link={DataKey.VRAttr}>{_ValidationRequirementNameAttribute}</link>.requirementType",
                    "requirementType",
                    "Type of Type",
                    _ValidationRequirementNameAttributeAssembly,
                    1,
                    DataKey.VRAttr4)
            }),

        // ====================================================

        new Data(
            _ScriptableValidationRequirement,
            _ScriptableValidationRequirement,
            $"Class in {typeof(ScriptableValidationRequirement).Namespace}\nInherits from {_ValidationRequirementMetaData}",
            _ScriptableValidationRequirementAssembly,
            0,
            DataKey.SVR,
            new List <Data>
            {
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.<m>AddError()</m>",
                    "AddError()",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR0),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.<m>AddErrorIf()</m>",
                    "AddErrorIf()",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR1),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.<m>AddErrors()</m>",
                    "AddErrors()",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR2),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.<m>OnRequirementValidation()</m>",
                    "OnRequirementValidation()",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR3),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.<m>Validate()</m>",
                    "Validate()",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR4),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.initialized",
                    "initialized",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR5),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.objectReference",
                    "objectReference",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR6),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.preventListHeaderIssue",
                    "preventListHeaderIssue",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR7),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.severityOverwrite",
                    "severityOverwrite",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR8),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.targetGameObject",
                    "targetGameObject",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR9),
                new(
                    $"<link={DataKey.SVR}>{_ScriptableValidationRequirement}</link>.uniqueID",
                    "uniqueID",
                    "...",
                    _ScriptableValidationRequirementAssembly,
                    1,
                    DataKey.SVR10)
            }),

        // ====================================================

        new Data(
        _ValidatableMonoBehaviour,
        _ValidatableMonoBehaviour,
        $"Class in {typeof(ValidatableMonoBehaviour).Namespace}\nInherits from {nameof(MonoBehaviour)}",
        _ValidatableMonoBehaviourAssembly,
        0,
        DataKey.VMB,
        new List <Data>
        {
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.SettingPriority",
                "SettingPriority",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB0),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>BeforeValidation()</m>",
                "BeforeValidation()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB1),            
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>DefaultImports()</m>",
                "DefaultImports()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB2),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>GetImportedSettings()</m>",
                "GetImportedSettings()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB3),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>ImportSetting()</m>",
                "ImportSetting()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB4),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>RemoveImportedSetting()</m>",
                "RemoveImportedSetting()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB5),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>Requirements()</m>",
                "Requirements()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB6),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>SetRequirements()</m>",
                "SetRequirements()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB7),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.<m>Validate()</m>",
                "Validate()",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB8),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.HasImportedSettings",
                "HasImportedSettings",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB9),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.defaultSettings",
                "defaultSettings",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB10),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.importedSettingsFoldout",
                "importedSettingsFoldout",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB11),
            new(
                $"<link={DataKey.VMB}>{_ValidatableMonoBehaviour}</link>.settingPriorities",
                "settingPriorities",
                "...",
                _ValidatableMonoBehaviourAssembly,
                1,
                DataKey.VMB12),
        }),
        
        // ====================================================
        
        new Data(
            _ValidationError,
            _ValidationError,
            $"Struct in {typeof(ValidationError).Namespace}",
            _ValidationErrorAssembly,
            0,
            DataKey.VErr,
            new List <Data>
            {
                new(
                    $"<link={DataKey.VErr}>{_ValidationError}</link>.errorName",
                    "errorName",
                    "...",
                    _ValidationErrorAssembly,
                    1,
                    DataKey.VErr0),
                new(
                    $"<link={DataKey.VErr}>{_ValidationError}</link>.errorText",
                    "errorText",
                    "...",
                    _ValidationErrorAssembly,
                    1,
                    DataKey.VErr1),
                new(
                    $"<link={DataKey.VErr}>{_ValidationError}</link>.fixAction",
                    "fixAction",
                    "...",
                    _ValidationErrorAssembly,
                    1,
                    DataKey.VErr2),
                new(
                    $"<link={DataKey.VErr}>{_ValidationError}</link>.gameObject",
                    "gameObject",
                    "...",
                    _ValidationErrorAssembly,
                    1,
                    DataKey.VErr3),
                new(
                    $"<link={DataKey.VErr}>{_ValidationError}</link>.severity",
                    "severity",
                    "...",
                    _ValidationErrorAssembly,
                    1,
                    DataKey.VErr4),
            }),
        
        // ====================================================

        /*
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
        new Data {title = "ValidationState", displayName = "ValidationState", indentLevel = 0}*/
    };

    #region ValidationRequirement

    private static string _ValidationRequirementNameAttribute =>
        s_validationRequirementNameAttribute ??= nameof(ValidationRequirementAttribute);
    
    private static string _ValidationRequirementNameAttributeAssembly =>
        s_validationRequirementNameAttributeAssembly ??= typeof(ValidationRequirementAttribute).Assembly.GetName().Name;

    #endregion

    #region ScriptableValidationRequirement

    private static string _ScriptableValidationRequirement =>
        s_scriptableValidationRequirement ??= nameof(ScriptableValidationRequirement);

    private static string _ScriptableValidationRequirementAssembly =>
        s_scriptableValidationRequirementAssembly ??= typeof(ScriptableValidationRequirement).Assembly.GetName().Name;

    #endregion

    #region ValidationRequirementMetaData

    private static string _ValidationRequirementMetaData =>
        s_validationRequirementMetaData ??= nameof(ValidationRequirementMetaData);
    
    private static string _ValidationRequirementMetaDataAssembly =>
        s_validationRequirementMetaDataAssembly ??= typeof(ValidationRequirementMetaData).Assembly.GetName().Name;

    #endregion

    #region ValidatableMonoBehaviour

    private static string _ValidatableMonoBehaviour =>
        s_validatableMonoBehaviour ??= nameof(ValidatableMonoBehaviour);
    
    private static string _ValidatableMonoBehaviourAssembly =>
        s_validatableMonoBehaviourAssembly ??= typeof(ValidatableMonoBehaviour).Assembly.GetName().Name;

    #endregion

    #region ValidationError
    
    private static string _ValidationError =>
        s_validationError ??= nameof(ValidationError);
    
    private static string _ValidationErrorAssembly =>
        s_validationErrorAssembly ??= typeof(ValidationError).Assembly.GetName().Name;

    #endregion



    #region Public Methods

    /// <summary> Get all data </summary>
    /// <returns> All data </returns>
    public static List <Data> Get()
    {
        return s_data;
    }

    /// <summary> Get a specific data by their key </summary>
    /// <param name="key"> Target key </param>
    /// <returns> Found data </returns>
    public static Data Get(DataKey key)
    {
        return s_data.FirstOrDefault(data => data.key == key);
    }

    #endregion
}

}
#endif
