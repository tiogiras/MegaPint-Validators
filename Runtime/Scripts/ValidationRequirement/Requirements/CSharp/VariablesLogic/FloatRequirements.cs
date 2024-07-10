// TODO commenting
using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class FloatRequirements
{
    #region Public Methods

    // ReSharper disable once CognitiveComplexity
    public static bool Validate(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        Variable.FloatRequirement requirement,
        float targetedValue,
        float referenceValue,
        float rangeMin,
        float rangeMax,
        out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        ValidationError error;

        switch (requirement)
        {
            case Variable.FloatRequirement.None:
                break;

            case Variable.FloatRequirement.Equals:
                if (!ValidateEquals(ref isValid, variable, value, targetedValue, out error))
                    errors.Add(error);

                break;

            case Variable.FloatRequirement.GreaterThan:
                if (!ValidateGreaterThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.FloatRequirement.GreaterEqualsThan:
                if (!ValidateGreaterEqualsThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.FloatRequirement.LesserThan:
                if (!ValidateLesserThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.FloatRequirement.LesserEqualsThan:
                if (!ValidateLesserEqualsThan(ref isValid, variable, value, referenceValue, out error))
                    errors.Add(error);

                break;

            case Variable.FloatRequirement.Range:
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
        float targetedValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (floatValue == targetedValue)
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
        float referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        if (floatValue >= referenceValue)
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
        float referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        if (floatValue > referenceValue)
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
        float referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        if (floatValue <= referenceValue)
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
        float referenceValue,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        if (floatValue < referenceValue)
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
        float rangeMin,
        float rangeMax,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value is not float floatValue)
            return true;

        if (floatValue >= rangeMin && floatValue <= rangeMax)
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
