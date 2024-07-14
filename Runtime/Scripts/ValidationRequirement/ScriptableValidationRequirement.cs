using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint.ValidationRequirement
{

/// <summary> Abstract class to create validation requirements for the <see cref="ValidatableMonoBehaviour" /> </summary>
[Serializable]
public abstract class ScriptableValidationRequirement : ValidationRequirementMetaData, IValidationRequirement
{
    [HideInInspector] public string preventListHeaderIssues;

    [HideInInspector] public bool initialized;
    [HideInInspector] public string uniqueID;
    [HideInInspector] public ValidationState severityOverwrite;
    [HideInInspector] public GameObject targetGameObject;
    [HideInInspector] public Object objectReference;

    private List <ValidationError> _errors;

    #region Unity Event Functions

    public virtual void OnValidate(Object o)
    {
        objectReference = o;
        TryInitialize(this);

        OnRequirementValidation();
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Validates the gameObject based on the specified <see cref="Validate" /> method.
    ///     Called when validating the <see cref="ValidatableMonoBehaviour" />.
    /// </summary>
    /// <param name="target"> GameObject that is validated </param>
    /// <param name="errors"> All found <see cref="ValidationError" /> </param>
    /// <returns> The highest <see cref="ValidationState" /> found in the <see cref="ValidationError" /> </returns>
    public ValidationState Validate(GameObject target, out List <ValidationError> errors)
    {
        _errors = new List <ValidationError>();
        targetGameObject = target;

        Validate(targetGameObject);

        errors = _errors;

        var severity = ValidationState.Ok;

        if (_errors.Count == 0)
            return severity;

        foreach (ValidationError error in errors.Where(error => error.severity > severity))
            severity = error.severity;

        return severity;
    }

    /// <summary> Get all compatible requirements of the lowPriority list </summary>
    /// <param name="highPriority"> Requirements with higher priority </param>
    /// <param name="lowPriority"> Requirements with lower priority </param>
    /// <returns> All requirements of the lower priority that are compatible with the higher priority </returns>
    internal static List <ScriptableValidationRequirement> GetCompatibleRequirements(
        List <ScriptableValidationRequirement> highPriority,
        List <ScriptableValidationRequirement> lowPriority)
    {
        List <ScriptableValidationRequirement> compatibles = new();

        foreach (ScriptableValidationRequirement lpRequirement in lowPriority)
        {
            if (lpRequirement == null)
                continue;

            Type lpType = lpRequirement.GetType();

            if (IsCompatible(highPriority, lpType))
                compatibles.Add(lpRequirement);
        }

        return compatibles;
    }

    /// <summary> Change the severity overwrite </summary>
    /// <exception cref="System.ArgumentOutOfRangeException"> Severity not found </exception>
    internal void ChangeSeverityOverwrite()
    {
        severityOverwrite = severityOverwrite switch
                            {
                                ValidationState.Unknown or ValidationState.Ok => ValidationState.Warning,
                                ValidationState.Warning => ValidationState.Error,
                                ValidationState.Error => ValidationState.Unknown,
                                var _ => throw new ArgumentOutOfRangeException()
                            };

        SetDirty();
    }

    /// <summary> Generate an unique id </summary>
    internal void GenerateUniqueID()
    {
        uniqueID = Guid.NewGuid().ToString();
    }

    /// <summary> Set the object of the requirement dirty </summary>
    internal void SetDirty()
    {
#if UNITY_EDITOR
        if (objectReference != null)
            EditorUtility.SetDirty(objectReference);
#endif
    }

    #endregion

    #region Protected Methods

    /// <summary> Add an error to the validation result </summary>
    /// <param name="errorName"> Name of the error </param>
    /// <param name="errorText"> Additional information about the error </param>
    /// <param name="severity"> <see cref="ValidationState" /> of the error </param>
    /// <param name="fixAction"> Action that is called when attempting to fix the error </param>
    protected void AddError(string errorName, string errorText, ValidationState severity, Action <GameObject> fixAction)
    {
        Action <GameObject> finalFixAction = fixAction == null
            ? null
            : o =>
            {
                if (o.scene.name == null && o.transform.parent != null)
                    ExecuteFixActionForChildInPrefab(o, fixAction);
                else
                {
                    fixAction.Invoke(o);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(o);
#endif
                }
            };

        _errors.Add(
            new ValidationError
            {
                errorName = errorName,
                errorText = errorText,
                gameObject = targetGameObject,
                fixAction = finalFixAction,
                severity = GetSeverity(severity)
            });
    }

    /// <summary> Add an error to the validation result when the statement is true </summary>
    /// <param name="statement"> Add the error when this is true </param>
    /// <param name="errorName"> Name of the error </param>
    /// <param name="errorText"> Additional information about the error </param>
    /// <param name="severity"> <see cref="ValidationState" /> of the error </param>
    /// <param name="fixAction"> Action that is called when attempting to fix the error </param>
    protected void AddErrorIf(
        bool statement,
        string errorName,
        string errorText,
        ValidationState severity,
        Action <GameObject> fixAction)
    {
        if (statement)
            AddError(errorName, errorText, severity, fixAction);
    }

    /// <summary> Add a range of errors </summary>
    /// <param name="errors"> Errors to add </param>
    protected void AddErrors(List <ValidationError> errors)
    {
        foreach (ValidationError error in errors)
            AddError(error.errorName, error.errorText, error.severity, error.fixAction);
    }

    /// <summary> Called when requirement is validated even when it is not initializing </summary>
    protected virtual void OnRequirementValidation()
    {
    }

    /// <summary> Validates the gameObject </summary>
    /// <param name="gameObject"> GameObject that is validated </param>
    protected abstract void Validate(GameObject gameObject);

    #endregion

    #region Private Methods

    /// <summary> Get the object by the specified path in it's parent </summary>
    /// <param name="path"> Path to the targeted object </param>
    /// <param name="root"> Root object the search starts in </param>
    /// <returns> Found child object at the path </returns>
    private static GameObject GetObjectByParentPath(string path, Transform root)
    {
        if (path.Equals(root.name))
            return root.gameObject;

        if (!path.StartsWith($"{root.name}/"))
        {
            Debug.LogError("Could not find root transform of the prefab hierarchy!");

            return null;
        }

        path = path[(root.name.Length + 1)..];

        Transform gameObject = root.Find(path);

        if (gameObject != null)
            return gameObject.gameObject;

        Debug.LogError("Could not find the targeted object inside the prefab hierarchy!");

        return null;
    }

    /// <summary> Get the path to the upmost parent of the targeted object </summary>
    /// <param name="transform"> Targeted object </param>
    /// <returns> Path to the upmost parent of the targeted object </returns>
    private static string GetPathToRoot(Transform transform)
    {
        List <Transform> path = new();

        while (transform != null)
        {
            path.Add(transform);
            transform = transform.parent;
        }

        path.Reverse();

        return string.Join("/", path.Select(p => p.name));
    }

    /// <summary> Check if the lower priority requirement is compatible with the higher priority requirements </summary>
    /// <param name="highPriority"> Higher priority requirements </param>
    /// <param name="lpType"> Type of the lower priority requirement </param>
    /// <returns> If the requirement is compatible </returns>

    // Resharper disable once CognitiveComplexity
    private static bool IsCompatible(List <ScriptableValidationRequirement> highPriority, Type lpType)
    {
        var compatible = true;

        foreach (ScriptableValidationRequirement hpRequirement in highPriority)
        {
            if (!compatible)
                break;

            if (hpRequirement == null)
                continue;

            Type hpType = hpRequirement.GetType();
            var hpAttr = hpType.GetCustomAttribute <ValidationRequirementNameAttribute>();

            if (lpType == hpType && !hpAttr.allowMultiple)
                compatible = false;

            if (hpAttr.incompatibleRequirements is not {Length: > 0})
                continue;

            if (hpAttr.incompatibleRequirements.Contains(lpType))
                compatible = false;
        }

        return compatible;
    }

    /// <summary> Execute the fixAction for the targeted child in the prefab </summary>
    /// <param name="o"> Targeted object </param>
    /// <param name="fixAction"> FixAction to execute </param>
    private void ExecuteFixActionForChildInPrefab(GameObject o, Action <GameObject> fixAction)
    {
#if UNITY_EDITOR
        var prefabPath = AssetDatabase.GetAssetPath(o);
        var prefab = AssetDatabase.LoadAssetAtPath <GameObject>(prefabPath);

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        GameObject go = GetObjectByParentPath(GetPathToRoot(o.transform), instance.transform);

        if (go == null)
            return;

        fixAction.Invoke(go);

        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
        Object.DestroyImmediate(instance);

        EditorUtility.SetDirty(prefab);
#endif
    }

    /// <summary> Get the severity to send with the error based on if the severity overwrite is active </summary>
    /// <param name="severity"> Severity given by the source error </param>
    /// <returns> Severity to send with the error </returns>
    private ValidationState GetSeverity(ValidationState severity)
    {
        return severityOverwrite is ValidationState.Unknown or ValidationState.Ok ? severity : severityOverwrite;
    }

    #endregion
}

}
