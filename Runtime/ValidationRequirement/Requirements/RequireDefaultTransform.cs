using System;
using System.Collections.Generic;
using System.Text;
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

    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();
        Transform transform = gameObject.transform;

        var validPosition = transform.position == Vector3.zero || !_defaultPosition;
        var validRotation = transform.rotation == Quaternion.identity || !_defaultRotation;
        var validScale = transform.localScale == Vector3.one || !_defaultScale;

        var valid = validPosition && validRotation && validScale;

        if (valid)
            return ValidationState.Ok;

        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine("Position should be (0,0,0)");

        if (!validRotation)
            errorText.AppendLine("Rotation should be (0,0,0)");

        if (!validScale)
            errorText.AppendLine("Scale should be (1,1,1)");
        
        errors.Add(new ValidationError
        {
            fixAction = FixAction,
            gameObject = gameObject,
            severity = ValidationState.Warning,
            errorName = "Non-Default Transform",
            errorText = errorText.ToString()
        });

        return ValidationState.Warning;
    }

    public void FixAction(GameObject gameObject)
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
