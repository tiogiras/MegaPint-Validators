// TODO commenting

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

[Serializable]
public class Variable
{
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
    }

    public enum BooleanRequirement
    {
        None, Equals
    }

    public enum FloatRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range
    }

    public enum IntegerRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range
    }

    public enum ObjectRequirement
    {
        None, NotNull, Equals
    }

    public enum StringRequirement
    {
        None, NotNull, Equals
    }

    public enum Type
    {
        Object, String, Bool, Int, Float
    }

    public Properties properties;

    #region Public Methods

    public bool Validate(object value, out List <ValidationError> errors)
    {
        var isValid = true;

        errors = new List <ValidationError>();

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
