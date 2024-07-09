namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

public static class StringRequirements
{
    private static void ValidateNotNull(ref bool isValid, Variable.Properties variable, object value, out ValidationError error)
    {
        error = new ValidationError();

        if (value is not string stringValue)
            return;
        
        if (!string.IsNullOrEmpty(stringValue))
            return;

        isValid = false;
            
        error = new ValidationError
        {
            errorName = "Null Reference",
            errorText = $"The variable {variable.name} is Null",
            severity = ValidationState.Error,
            fixAction = null
        };
    }
}

}
