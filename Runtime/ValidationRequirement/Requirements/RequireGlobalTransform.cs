using System;
using System.Collections.Generic;
using System.Text;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Custom Global Transform")]
public class RequireGlobalTransform : ValidationRequirementMetaData, IValidationRequirement
{
    [SerializeField, Tooltip("If true the transform position requirement is used")]
    private bool _requireGlobalPosition;
    
    [SerializeField, Tooltip("If true the transform position requirement is used")]
    private bool _requireGlobalRotation;
    
    [SerializeField, Tooltip("If true the transform position requirement is used")]
    private bool _requireGlobalScale;
    
    [Space]
    [SerializeField, Tooltip("The transform is required to have this specified global position")]
    private Vector3 _globalPosition;
    
    [SerializeField, Tooltip("The transform is required to have this specified global rotation")]
    private Vector3 _globalRotation;

    [SerializeField, Tooltip("The transform is required to have this specified global scale")]
    private Vector3 _globalScale;

    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();
        Transform transform = gameObject.transform;

        var validPosition = transform.position == _globalPosition || !_requireGlobalPosition;
        var validRotation = transform.rotation == Quaternion.Euler(_globalRotation) || !_requireGlobalRotation;
        var validScale = transform.lossyScale == _globalScale || !_requireGlobalScale;

        if (validPosition && validRotation && validScale)
            return ValidationState.Ok;
        
        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine($"Position should be {_globalPosition}");

        if (!validRotation)
            errorText.AppendLine($"Rotation should be {_globalRotation}");

        if (!validScale)
            errorText.AppendLine($"Scale should be {_globalScale}");
        
        errors.Add(new ValidationError
        {
            errorName = "Transform differs from specifications",
            errorText = errorText.ToString(),
            fixAction = FixAction,
            gameObject = gameObject,
            severity = ValidationState.Warning
        });

        return ValidationState.Warning;
    }

    private void FixAction(GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        
        if (_requireGlobalPosition)
            transform.position = _globalPosition;
        
        if (_requireGlobalRotation)
            transform.rotation = Quaternion.Euler(_globalRotation);

        if (!_requireGlobalScale)
            return;

        Transform originalParent = transform.parent;
        transform.SetParent(null);
        transform.localScale = _globalScale;
        transform.SetParent(originalParent);
    }

    public void OnValidate()
    {
        TryInitialize();
    }

    protected override void Initialize()
    {
        _globalScale = Vector3.one;
    }
}

}
