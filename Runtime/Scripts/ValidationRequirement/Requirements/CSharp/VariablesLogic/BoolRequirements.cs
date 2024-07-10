// TODO commenting
using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class BoolRequirements
{
    #region Public Methods

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
                if(!ValidateEquals(ref isValid, variable, value, targetedValue, out ValidationError error))
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
