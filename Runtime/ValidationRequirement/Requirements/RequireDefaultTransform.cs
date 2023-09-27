using System;
using System.Collections.Generic;
using System.Text;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Default Transform")]
public class RequireDefaultTransform : ValidationRequirementMetaData, IValidationRequirement
{
    [SerializeField, Tooltip("If true the transform is required to have a local position of (0,0,0)")] 
    private bool _defaultPosition;
    
    [SerializeField, Tooltip("If true the transform is required to have a local rotation of (0,0,0)")] 
    private bool _defaultRotation;
    
    [SerializeField, Tooltip("If true the transform is required to have a local scale of (1,1,1)")] 
    private bool _defaultScale;

    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();
        Transform transform = gameObject.transform;

        var validPosition = transform.localPosition == Vector3.zero || !_defaultPosition;
        var validRotation = transform.localRotation == Quaternion.identity || !_defaultRotation;
        var validScale = transform.localScale == Vector3.one || !_defaultScale;
        
        if (validPosition && validRotation && validScale)
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

    private void FixAction(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        if (_defaultPosition)
            transform.localPosition = Vector3.zero;

        if (_defaultRotation)
            transform.localRotation = Quaternion.identity;

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
