using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class StringRequirements
{
    #region Public Methods

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
