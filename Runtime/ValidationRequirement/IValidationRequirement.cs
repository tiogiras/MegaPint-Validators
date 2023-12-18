using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement
{

/// <summary> Interface to call the validation methods on the <see cref="ScriptableValidationRequirement" /> </summary>
public interface IValidationRequirement
{
    #region Unity Event Functions

    /// <summary> Called when the <see cref="ValidatableMonoBehaviour" /> is validated by unity  </summary>
    public void OnValidate();

    #endregion

    #region Public Methods

    /// <summary> Called when validating a <see cref="ValidatableMonoBehaviour" /> </summary>
    /// <param name="gameObject"> The gameObject the <see cref="ValidatableMonoBehaviour" /> is added to </param>
    /// <param name="errors">
    ///     All <see cref="ValidationError" /> found while validating all
    ///     <see cref="ScriptableValidationRequirement" />
    /// </param>
    /// <returns> The highest <see cref="ValidationState" /> in the found <see cref="ValidationError" /> </returns>
    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors);

    #endregion
}

}
