using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement
{

public interface IValidationRequirement
{
    public ValidationState Validate(GameObject gameObject, out List <InvalidBehaviour.ValidationError> errors);

    public void Fix(GameObject gameObject);

    public void OnValidate();
}

}
