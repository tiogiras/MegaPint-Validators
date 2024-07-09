using System;
using System.Text;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.TransformValidation
{

/// <summary> Validation requirement for custom values on a transform component </summary>
[Serializable]
[SerializeReferenceDropdownName("Transform/Custom Global", typeof(RequireGlobalTransform), -29, 2)]
public class RequireGlobalTransform : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("The transform is required to have this specified global position")]
    private ToggleableSetting <Vector3> _globalPosition;

    [SerializeField] [Tooltip("The transform is required to have this specified global rotation")]
    private ToggleableSetting <Vector3> _globalRotation;

    [SerializeField] [Tooltip("The transform is required to have this specified global scale")]
    private ToggleableSetting <Vector3> _globalScale;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _globalScale.value = Vector3.one;
    }

    protected override void Validate(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        var validPosition = transform.position == _globalPosition.value || !_globalPosition.enabled;
        var validRotation = transform.rotation == Quaternion.Euler(_globalRotation.value) || !_globalRotation.enabled;
        var validScale = transform.lossyScale == _globalScale.value || !_globalScale.enabled;

        if (validPosition && validRotation && validScale)
            return;

        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine($"Position should be {_globalPosition}");

        if (!validRotation)
            errorText.AppendLine($"Rotation should be {_globalRotation}");

        if (!validScale)
            errorText.Append($"Scale should be {_globalScale}");

        AddError("Transform differs from specifications", errorText.ToString(), ValidationState.Warning, FixAction);
    }

    #endregion

    #region Private Methods

    /// <summary> Fix the values of the transform component </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
    private void FixAction(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        if (_globalPosition.enabled)
            transform.position = _globalPosition.value;

        if (_globalRotation.enabled)
            transform.rotation = Quaternion.Euler(_globalRotation.value);

        if (!_globalScale.enabled)
            return;

        Transform originalParent = transform.parent;
        transform.SetParent(null);
        transform.localScale = _globalScale.value;
        transform.SetParent(originalParent);
    }

    #endregion
}

}
