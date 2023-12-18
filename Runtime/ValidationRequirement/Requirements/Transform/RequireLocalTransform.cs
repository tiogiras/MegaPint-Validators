using System;
using System.Text;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements.Transform
{

[Serializable]
[SerializeReferenceDropdownName("Transform/Custom Local", -10, 1)]
public class RequireLocalTransform : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("The transform is required to have this specified local position")]
    private ToggleableSetting <Vector3> _localPosition;

    [SerializeField] [Tooltip("The transform is required to have this specified local rotation")]
    private ToggleableSetting <Vector3> _localRotation;

    [SerializeField] [Tooltip("The transform is required to have this specified local scale")]
    private ToggleableSetting <Vector3> _localScale;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _localScale.value = Vector3.one;
    }

    protected override void Validate(UnityEngine.GameObject gameObject)
    {
        UnityEngine.Transform transform = gameObject.transform;

        var validPosition = transform.localPosition == _localPosition.value || !_localPosition.enabled;
        var validRotation = transform.localRotation == Quaternion.Euler(_localRotation.value) || !_localRotation.enabled;
        var validScale = transform.localScale == _localScale.value || !_localScale.enabled;

        if (validPosition && validRotation && validScale)
            return;

        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine($"Position should be {_localPosition.value}");

        if (!validRotation)
            errorText.AppendLine($"Rotation should be {_localRotation.value}");

        if (!validScale)
            errorText.AppendLine($"Scale should be {_localScale.value}");

        AddError("Transform differs from specifications", errorText.ToString(), ValidationState.Warning, FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(UnityEngine.GameObject gameObject)
    {
        UnityEngine.Transform transform = gameObject.transform;

        if (_localPosition.enabled)
            transform.localPosition = _localPosition.value;

        if (_localRotation.enabled)
            transform.localRotation = Quaternion.Euler(_localRotation.value);

        if (_localScale.enabled)
            transform.localScale = _localScale.value;
    }

    #endregion
}

}
