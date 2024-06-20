using System;
using System.Text;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.TransformValidation
{

/// <summary> Validation requirement for default values on a transform component </summary>
[Serializable]
[SerializeReferenceDropdownName("Transform/Default", typeof(RequireDefaultTransform), -10, 0)]
public class RequireDefaultTransform : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("If true the transform is required to have a local position of (0,0,0)")]
    private bool _defaultPosition;

    [SerializeField] [Tooltip("If true the transform is required to have a local rotation of (0,0,0)")]
    private bool _defaultRotation;

    [SerializeField] [Tooltip("If true the transform is required to have a local scale of (1,1,1)")]
    private bool _defaultScale;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _defaultPosition = true;
        _defaultRotation = true;
        _defaultScale = false;
    }

    protected override void Validate(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        var validPosition = transform.localPosition == Vector3.zero || !_defaultPosition;
        var validRotation = transform.localRotation == Quaternion.identity || !_defaultRotation;
        var validScale = transform.localScale == Vector3.one || !_defaultScale;

        if (validPosition && validRotation && validScale)
            return;

        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine("Position should be (0,0,0)");

        if (!validRotation)
            errorText.AppendLine("Rotation should be (0,0,0)");

        if (!validScale)
            errorText.Append("Scale should be (1,1,1)");

        AddError(
            "Non-Default Transform",
            errorText.ToString(),
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    /// <summary> Fix the values of the transform component </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
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

    #endregion
}

}
