using UnityEngine;

namespace ValidationRequirement
{

public interface IValidationRequirement
{
    public ValidationState Validate(GameObject gameObject);

    public void Fix(GameObject gameObject);

    public void OnValidate();
}

}
