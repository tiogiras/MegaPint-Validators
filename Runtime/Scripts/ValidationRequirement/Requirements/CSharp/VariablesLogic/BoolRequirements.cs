using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

/// <summary> Requirement for booleans </summary>
internal static class BoolRequirements
{
    #region Public Methods

    /// <summary> Validate the bool variable </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="requirement"> Current set requirement for the variable </param>
    /// <param name="targetedValue"> Targeted value for equals requirement </param>
    /// <param name="errors"> Validation errors </param>
    /// <returns> If any errors occured </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Requirement not found </exception>
    public static bool Validate(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        Variable.BooleanRequirement requirement,
        bool targetedValue,
        out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        switch (requirement)
        {
            case Variable.BooleanRequirement.None:
                break;

            case Variable.BooleanRequirement.Equals:
                if (!ValidateEquals(ref isValid, variable, value, targetedValue, out ValidationError error))
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
        bool targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not bool boolValue)
            return true;

        if (boolValue == targetedValue)
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

    #endregion
}

}
