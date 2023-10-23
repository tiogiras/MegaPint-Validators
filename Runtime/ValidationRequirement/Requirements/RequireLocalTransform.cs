using System;
using System.Text;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Transform/Custom Local", default, 1)]
public class RequireLocalTransform : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("If true the transform position requirement is used")]
    private bool _requireLocalPosition;

    [SerializeField] [Tooltip("If true the transform position requirement is used")]
    private bool _requireLocalRotation;

    [SerializeField] [Tooltip("If true the transform position requirement is used")]
    private bool _requireLocalScale;

    [Space]
    [SerializeField] [Tooltip("The transform is required to have this specified local position")]
    private Vector3 _localPosition;

    [SerializeField] [Tooltip("The transform is required to have this specified local rotation")]
    private Vector3 _localRotation;

    [SerializeField] [Tooltip("The transform is required to have this specified local scale")]
    private Vector3 _localScale;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _localScale = Vector3.one;
    }

    protected override void Validate(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        var validPosition = transform.localPosition == _localPosition || !_requireLocalPosition;
        var validRotation = transform.localRotation == Quaternion.Euler(_localRotation) || !_requireLocalRotation;
        var validScale = transform.localScale == _localScale || !_requireLocalScale;

        if (validPosition && validRotation && validScale)
            return;

        var errorText = new StringBuilder();

        if (!validPosition)
            errorText.AppendLine($"Position should be {_localPosition}");

        if (!validRotation)
            errorText.AppendLine($"Rotation should be {_localRotation}");

        if (!validScale)
            errorText.AppendLine($"Scale should be {_localScale}");

        AddError("Transform differs from specifications", errorText.ToString(), ValidationState.Warning, FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(GameObject gameObject)
    {
        Transform transform = gameObject.transform;

        if (_requireLocalPosition)
            transform.localPosition = _localPosition;

        if (_requireLocalRotation)
            transform.localRotation = Quaternion.Euler(_localRotation);

        if (_requireLocalScale)
            transform.localScale = _localScale;
    }

    #endregion
}

}
