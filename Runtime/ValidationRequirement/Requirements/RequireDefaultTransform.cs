using System;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Require Default Transform")]
public class RequireDefaultTransform : ValidationRequirementMetaData, IValidationRequirement
{
    [SerializeField] private bool _defaultPosition;
    [SerializeField] private bool _defaultRotation;
    [SerializeField] private bool _defaultScale;

    public ValidationState Validate(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        return (transform.position == Vector3.zero || !_defaultPosition) &&
               (transform.rotation == Quaternion.identity || !_defaultRotation) &&
               (transform.localScale == Vector3.one || !_defaultScale)
            ? ValidationState.Ok
            : ValidationState.Warning;
    }

    public void Fix(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        if (_defaultPosition)
            transform.position = Vector3.zero;

        if (_defaultRotation)
            transform.rotation = Quaternion.identity;

        if (_defaultScale)
            transform.localScale = Vector3.one;
    }

    public void OnValidate()
    {
        TryInitialize();
    }

    protected override void Initialize()
    {
        _defaultPosition = true;
        _defaultRotation = true;
        _defaultScale = false;
    }
}

}
