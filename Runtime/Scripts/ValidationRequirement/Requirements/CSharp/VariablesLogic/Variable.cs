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

    // TODO Drawer for this class (The drawer then decides what to draw based oin the requirement enum)

    public enum Type
    {
        Object, String, Bool, Int, Float
    }

    public Properties properties;

    #region Public Methods

    public bool Validate(object value, out List <ValidationError> errors)
    {
        var isValid = true;

        List <ValidationError> newErrors;

        errors = new List <ValidationError>();

        switch (properties.type)
        {
            case Type.Object:
                if (!ObjectRequirements.Validate(
                        ref isValid,
                        properties,
                        value,
                        properties.objectRequirement,
                        properties.targetObjectValue,
                        out newErrors))
                    errors.AddRange(newErrors);

                break;

            case Type.String:
                if (!StringRequirements.Validate(
                        ref isValid,
                        properties,
                        value,
                        properties.stringRequirement,
                        properties.targetStringValue,
                        out newErrors))
                    errors.AddRange(newErrors);

                break;

            case Type.Bool:
                break;

            case Type.Int:
                break;

            case Type.Float:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return isValid;
    }

    #endregion
}

}
