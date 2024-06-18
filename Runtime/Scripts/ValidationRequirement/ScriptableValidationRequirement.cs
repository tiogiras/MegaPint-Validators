using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegaPint.ValidationRequirement
{

/// <summary> Abstract class to create validation requirements for the <see cref="ValidatableMonoBehaviour" /> </summary>
public abstract class ScriptableValidationRequirement : ValidationRequirementMetaData, IValidationRequirement
{
    private List <ValidationError> _errors;
    private GameObject _gameObject;

    #region Public Methods

    #region Unity Event Functions

    /// <summary> Called when the <see cref="ValidatableMonoBehaviour" /> is validated by unity </summary>
    public virtual void OnValidate(ValidatableMonoBehaviour behaviour)
    {
        TryInitialize(behaviour, this);
    }

    #endregion

    /// <summary>
    ///     Validates the gameObject based on the specified <see cref="Validate" /> method.
    ///     Called when validating the <see cref="ValidatableMonoBehaviour" />.
    /// </summary>
    /// <param name="gameObject"> GameObject that is validated </param>
    /// <param name="errors"> All found <see cref="ValidationError" /> </param>
    /// <returns> The highest <see cref="ValidationState" /> found in the <see cref="ValidationError" /> </returns>
    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        _errors = new List <ValidationError>();
        _gameObject = gameObject;

        Validate(gameObject);

        errors = _errors;

        var severity = ValidationState.Ok;

        if (_errors.Count == 0)
            return severity;

        foreach (ValidationError error in errors)
        {
            if (error.severity > severity)
                severity = error.severity;
        }

        return severity;
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
                    Undo.RegisterCompleteObjectUndo(o, errorName);
                    fixAction.Invoke(o);
                    EditorUtility.SetDirty(o);
                }
            };

        _errors.Add(
            new ValidationError
            {
                errorName = errorName,
                errorText = errorText,
                gameObject = _gameObject,
                fixAction = finalFixAction,
                severity = severity
            });
    }

    // TODO commenting
    private void ExecuteFixActionForChildInPrefab(GameObject o, Action <GameObject> fixAction)
    {
        var prefabPath = AssetDatabase.GetAssetPath(o);
        var prefab = AssetDatabase.LoadAssetAtPath <GameObject>(prefabPath);

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    
        GameObject gameObject = GetObjectPyParentPath(GetPathToRoot(o.transform), instance.transform);
                    
        if (gameObject == null)
            return;
                    
        fixAction.Invoke(gameObject);
                    
        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
        Object.DestroyImmediate(instance);
        
        EditorUtility.SetDirty(prefab);
    }

    // TODO commenting
    private string GetPathToRoot(Transform transform)
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
    
    // TODO commenting
    private GameObject GetObjectPyParentPath(string path, Transform root)
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

    /// <summary> Validates the gameObject </summary>
    /// <param name="gameObject"> GameObject that is validated </param>
    protected abstract void Validate(GameObject gameObject);

    #endregion
}

}
