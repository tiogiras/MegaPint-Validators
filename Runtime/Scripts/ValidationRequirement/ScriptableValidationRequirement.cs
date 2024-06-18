using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    public virtual void OnValidate()
    {
        TryInitialize();
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
                Undo.RegisterCompleteObjectUndo(o, errorName);
                fixAction.Invoke(o);
                EditorUtility.SetDirty(o);
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

    /// <summary> Validates the gameObject </summary>
    /// <param name="gameObject"> GameObject that is validated </param>
    protected abstract void Validate(GameObject gameObject);

    #endregion
}

}
