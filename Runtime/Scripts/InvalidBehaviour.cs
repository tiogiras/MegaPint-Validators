using System;
using System.Collections.Generic;

namespace MegaPint
{

/// <summary> Invalid behaviour of <see cref="ValidatableMonoBehaviour" /> </summary>
public struct InvalidBehaviour : IComparable <InvalidBehaviour>
{
    public string behaviourName;
    public string shortBehaviourName;
    public List <ValidationError> errors;

    /// <summary> Compare method </summary>
    /// <param name="other"> Compare against this </param>
    /// <returns> Compare value </returns>
    public int CompareTo(InvalidBehaviour other)
    {
        return string.Compare(behaviourName, other.behaviourName, StringComparison.Ordinal);
    }
}

}
