using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

/// <summary> Requirement for integers </summary>
internal static class IntRequirements
{
    #region Public Methods

    /// <summary> Validate the integer variable </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="requirement"> Current set requirement for the variable </param>
    /// <param name="targetedValue"> Targeted value for equals requirement </param>
    /// <param name="rangeMax"> Max value for the range requirement </param>
    /// <param name="errors"> Validation errors </param>
    /// <param name="referenceValue"> Value to check against (Greater, GreaterEquals, Lesser, LesserEquals) </param>
    /// <param name="rangeMin"> Min value for the range requirement </param>
    /// <returns> If any errors occured </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Requirement not found </exception>

    // ReSharper disable once CognitiveComplexity
    public static bool Validate(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        Variable.IntegerRequirement requirement,
        int targetedValue,
        int referenceValue,
        int rangeMin,
        int rangeMax,
        out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        ValidationError error;

        switch (requirement)
        {
            case Variable.IntegerRequirement.None:
                break;

            case Variable.IntegerRequirement.Equals:
                if (!ValidateEquals(ref isValid, variable, value, targetedValue, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.GreaterThan:
                if (!ValidateGreaterThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.GreaterEqualsThan:
                if (!ValidateGreaterEqualsThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.LesserThan:
                if (!ValidateLesserThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.LesserEqualsThan:
                if (!ValidateLesserEqualsThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.Range:
                if (!ValidateRange(ref isValid, variable, value, rangeMin, rangeMax, out error))
                    errors.Add(error);

                break;

            case Variable.IntegerRequirement.IsNot:
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
        int targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue == targetedValue)
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

    /// <summary> Validate if the variable is greater or equal to the reference value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="referenceValue"> Value to check against </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateGreaterEqualsThan(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        int referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue >= referenceValue)
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Greater Or Equal",
            errorText = $"The variable {variable.name} is not greater or equal to [{referenceValue}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    /// <summary> Validate if the variable is greater than the reference value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="referenceValue"> Value to check against </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateGreaterThan(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        int referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue > referenceValue)
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Greater",
            errorText = $"The variable {variable.name} is not greater than [{referenceValue}]",
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
        int targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue != targetedValue)
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

    /// <summary> Validate if the variable is lesser or equal to the reference value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="referenceValue"> Value to check against </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateLesserEqualsThan(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        int referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue <= referenceValue)
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Lesser Or Equal",
            errorText = $"The variable {variable.name} is not lesser or equal to [{referenceValue}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    /// <summary> Validate if the variable is lesser than the reference value </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="referenceValue"> Value to check against </param>
    /// <param name="error"> Validation error </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateLesserThan(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        int referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue < referenceValue)
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Lesser",
            errorText = $"The variable {variable.name} is not lesser than [{referenceValue}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    /// <summary> Validate if the variable is in the specified range </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="variable"> Properties of the variable </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="rangeMax"> Range maximum </param>
    /// <param name="error"> Validation error </param>
    /// <param name="rangeMin"> Range minimum </param>
    /// <returns> If an error occured </returns>
    private static bool ValidateRange(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        int rangeMin,
        int rangeMax,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not int intValue)
            return true;

        if (intValue >= rangeMin && intValue <= rangeMax)
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Out Of Range",
            errorText = $"The variable {variable.name} is not in the range [{rangeMin} | {rangeMax}]",
            severity = ValidationState.Error,
            fixAction = null
        };

        return false;
    }

    #endregion
}

}
