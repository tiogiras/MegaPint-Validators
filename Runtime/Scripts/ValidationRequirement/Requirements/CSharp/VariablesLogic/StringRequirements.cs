using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

/// <summary> Requirement for strings </summary>
internal static class StringRequirements
{
    #region Public Methods

    /// <summary> Validate the string variable </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="requirement"> Current set requirement for the variable </param>
    /// <param name="targetedValue"> Targeted value for equals requirement </param>
    /// <param name="errors"> Validation errors </param>
    /// <returns> If any errors occured </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Requirement not found </exception>

    // ReSharper disable once CognitiveComplexity
    public static bool Validate(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        Variable.StringRequirement requirement,
        string targetedValue,
        out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        ValidationError error;

        switch (requirement)
        {
            case Variable.StringRequirement.None:
                break;

            case Variable.StringRequirement.NotNull:
                if (!ValidateNotNull(ref isValid, variable, value, out error))
                    errors.Add(error);

                break;

            case Variable.StringRequirement.Equals:
                if (!ValidateEquals(ref isValid, variable, value, targetedValue, out error))
                    errors.Add(error);

                break;

            case Variable.StringRequirement.IsNot:
                if (!ValidateIsNot(ref isValid, variable, value, targetedValue, out error))
                    errors.Add(error);

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(requirement), requirement, null);
        }

        return errors.Count == 0;
    }

    #endregion

    #region Private Methods

    /// <summary> Validate if the variable equals the target value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="targetedValue"> Targeted value </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateEquals(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        string targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not string stringValue)
            return true;

        if (stringValue.Equals(targetedValue))
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Equal",
            errorText = $"The variable {variable.name} is not equal to [{targetedValue}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    /// <summary> Validate if the variable is not the targeted value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="targetedValue"> Targeted value </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateIsNot(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        string targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not string stringValue)
            return true;

        if (!stringValue.Equals(targetedValue))
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Is Forbidden Value",
            errorText = $"The variable {variable.name} is equal to [{targetedValue}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    /// <summary> Validate that the variable is not null </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateNotNull(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not string stringValue)
            return true;

        if (!string.IsNullOrEmpty(stringValue))
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Null Reference",
            errorText = $"The variable {variable.name} is Null",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    #endregion
}

}
