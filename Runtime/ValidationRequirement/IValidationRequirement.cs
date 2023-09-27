using System.Collections.Generic;
using UnityEngine;

namespace ValidationRequirement
{

public interface IValidationRequirement
{
    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors);

    public void OnValidate();
}

}
