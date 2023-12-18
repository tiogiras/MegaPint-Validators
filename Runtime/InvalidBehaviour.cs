using System;
using System.Collections.Generic;

public struct InvalidBehaviour : IComparable <InvalidBehaviour>
{
    public string behaviourName;
    public List <ValidationError> errors;

    public int CompareTo(InvalidBehaviour other)
    {
        return string.Compare(behaviourName, other.behaviourName, StringComparison.Ordinal);
    }
}
