using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

/// <summary> Class used to store the settings for a variable and validate it's requirement </summary>
[Serializable]
internal class Variable
{
    /// <summary> Class used to store all settings for a variable </summary>
    [Serializable]
    public class Properties
    {
        public string name;
        public Type type;

        public ObjectRequirement objectRequirement;
        public StringRequirement stringRequirement;
        public BooleanRequirement boolRequirement;
        public IntegerRequirement intRequirement;
        public FloatRequirement floatRequirement;

        public Object targetObjectValue;
        public string targetStringValue;
        public bool targetBoolValue;

        public int targetIntValue;
        public int intReferenceValue;
        public int intRangeMin;
        public int intRangeMax;

        public float targetFloatValue;
        public float floatReferenceValue;
        public float floatRangeMin;
        public float floatRangeMax;

        // Used to store the height of the list entry instance
        public float propertyHeight;

        // Storing information for the gui to display
        public bool fieldFound;
        public int typeIndex;
    }

    public enum BooleanRequirement
    {
        None, Equals
    }

    public enum FloatRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range, IsNot
    }

    public enum IntegerRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range, IsNot
    }

    public enum ObjectRequirement
    {
        None, NotNull, Equals, IsNot
    }

    public enum StringRequirement
    {
        None, NotNull, Equals, IsNot
    }

    public enum Type
    {
        Object, String, Bool, Int, Float
    }

    public Properties properties;

    #region Public Methods

    /// <summary> Validate the variable based on it's type and requirement </summary>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    /// <returns> If any errors occured </returns>
    /// <exception cref="System.ArgumentOutOfRangeException"> Type not found </exception>
    public bool Validate(object value, out List <ValidationError> errors)
    {
        var isValid = true;

        errors = new List <ValidationError>();

        SetTypeIndex(value);

        switch (properties.type)
        {
            case Type.Object:
                ObjectValidation(ref isValid, value, errors);

                break;

            case Type.String:
                StringValidation(ref isValid, value, errors);

                break;

            case Type.Bool:
                BoolValidation(ref isValid, value, errors);

                break;

            case Type.Int:
                IntValidation(ref isValid, value, errors);

                break;

            case Type.Float:
                FloatValidation(ref isValid, value, errors);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return isValid;
    }

    #endregion

    #region Private Methods

    /// <summary> Validate the variable as a boolean </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    private void BoolValidation(ref bool isValid, object value, List <ValidationError> errors)
    {
        if (!BoolRequirements.Validate(
                ref isValid,
                properties,
                value,
                properties.boolRequirement,
                properties.targetBoolValue,
                out List <ValidationError> newErrors))
            errors.AddRange(newErrors);
    }

    /// <summary> Validate the variable as a float </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    private void FloatValidation(ref bool isValid, object value, List <ValidationError> errors)
    {
        if (!FloatRequirements.Validate(
                ref isValid,
                properties,
                value,
                properties.floatRequirement,
                properties.targetFloatValue,
                properties.floatReferenceValue,
                properties.floatRangeMin,
                properties.floatRangeMax,
                out List <ValidationError> newErrors))
            errors.AddRange(newErrors);
    }

    /// <summary> Validate the variable as an integer </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    private void IntValidation(ref bool isValid, object value, List <ValidationError> errors)
    {
        if (!IntRequirements.Validate(
                ref isValid,
                properties,
                value,
                properties.intRequirement,
                properties.targetIntValue,
                properties.intReferenceValue,
                properties.intRangeMin,
                properties.intRangeMax,
                out List <ValidationError> newErrors))
            errors.AddRange(newErrors);
    }

    /// <summary> Validate the variable as an Object </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    private void ObjectValidation(ref bool isValid, object value, List <ValidationError> errors)
    {
        if (!ObjectRequirements.Validate(
                ref isValid,
                properties,
                value,
                properties.objectRequirement,
                properties.targetObjectValue,
                out List <ValidationError> newErrors))
            errors.AddRange(newErrors);
    }

    /// <summary> Set the index based on the type of the variable value </summary>
    /// <param name="value"> Value of the variable </param>
    private void SetTypeIndex(object value)
    {
        properties.typeIndex = value switch
                               {
                                   Object => 0,
                                   string => 1,
                                   bool => 2,
                                   int => 3,
                                   float => 4,
                                   var _ => properties.typeIndex
                               };
    }

    /// <summary> Validate the variable as a string </summary>
    /// <param name="isValid"> Validation bool reference </param>
    /// <param name="value"> Value of the variable </param>
    /// <param name="errors"> Validation errors </param>
    private void StringValidation(ref bool isValid, object value, List <ValidationError> errors)
    {
        if (!StringRequirements.Validate(
                ref isValid,
                properties,
                value,
                properties.stringRequirement,
                properties.targetStringValue,
                out List <ValidationError> newErrors))
            errors.AddRange(newErrors);
    }

    #endregion
}

}
