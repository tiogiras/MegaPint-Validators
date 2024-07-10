// TODO commenting
using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class IntRequirements
{
    #region Public Methods

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

            default:
                throw new ArgumentOutOfRangeException(nameof(requirement), requirement, null);
        }

        return errors.Count == 0;
    }

    #endregion

    #region Private Methods

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
