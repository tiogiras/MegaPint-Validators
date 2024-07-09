// TODO commenting

using System;
using System.Collections.Generic;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class ObjectRequirements
{
    #region Public Methods

    public static bool Validate(
        ref bool isValid,
        Variable.Properties variable,
        object value,
        Variable.ObjectRequirement requirement,
        object objectEquals,
        out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        ValidationError error;

        switch (requirement)
        {
            case Variable.ObjectRequirement.None:
                break;

            case Variable.ObjectRequirement.NotNull:
                if (!ValidateNotNull(ref isValid, variable, value, out error))
                    errors.Add(error);

                break;

            case Variable.ObjectRequirement.Equals:
                if (!ValidateEquals(ref isValid, variable, value, objectEquals, out error))
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
        object objectEquals,
        out ValidationError error)
    {
        error = new ValidationError();

        if (value.Equals(objectEquals))
            return true;

        isValid = false;

        error = new ValidationError
        {
            errorName = "Not Equal",
            errorText = $"The variable {variable.name} is not equal to {objectEquals}",
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

        if (value != null && !value.Equals(null))
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
