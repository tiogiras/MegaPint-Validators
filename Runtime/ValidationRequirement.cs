using UnityEngine;

public interface IValidationRequirement
{
    public bool Validate(GameObject gameObject);

    public void Fix(GameObject gameObject);
}