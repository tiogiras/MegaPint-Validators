using System;
using System.Text;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements.Transform
{

[Serializable]
[SerializeReferenceDropdownName("Transform/Default", -10, 0)]
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

    protected override void Validate(UnityEngine.GameObject gameObject)
    {
        UnityEngine.Transform transform = gameObject.transform;

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
            errorText.AppendLine("Scale should be (1,1,1)");

        AddError(
            "Non-Default Transform",
            errorText.ToString(),
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(UnityEngine.GameObject gameObject)
    {
        UnityEngine.Transform transform = gameObject.transform;

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
