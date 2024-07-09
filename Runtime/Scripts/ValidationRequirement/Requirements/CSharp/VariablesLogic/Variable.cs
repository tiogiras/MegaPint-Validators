using System;
using System.Collections.Generic;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic
{

[Serializable]
public class Variable
{
    public Properties properties;
    
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

        public UnityEngine.Object targetObjectValue;
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
    


    // TODO Drawer for this class (The drawer then decides what to draw based oin the requirement enum)
        
    public enum Type
    {
        Object, String, Bool, Int, Float
    }
    
    public enum ObjectRequirement
    {
        None, NotNull, Equals
    }
    
    public enum StringRequirement
    {
        None, NotNull, Equals
    }
    
    public enum BooleanRequirement
    {
        None, Equals
    }
    
    public enum IntegerRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range
    }
    
    public enum FloatRequirement
    {
        None, Equals, GreaterThan, GreaterEqualsThan, LesserThan, LesserEqualsThan, Range
    }

    public bool Validate(object value, out List <ValidationError> errors)
    {
        Debug.Log($"Validating variable: {properties.name}"); // TODO remove

        var isValid = true;

        errors = new List <ValidationError>();

        Debug.Log(properties.type);
        
        switch (properties.type)
        {
            case Type.Object:
                if (!ObjectRequirements.Validate(
                        ref isValid,
                        properties,
                        value,
                        properties.objectRequirement,
                        properties.targetObjectValue,
                        out List <ValidationError> newErrors))
                    errors.AddRange(newErrors);

                break;

            case Type.String:
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
}

}
