using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
using System.Linq;
#endif

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Requirement requires specific tag on the gameObject </summary>
[Serializable]
[ValidationRequirementTooltip("This requirement enforces the tag of the gameObject.")]
[ValidationRequirement("GameObject/Tag", typeof(RequireTag), -30, 2)]
internal class RequireTag : ScriptableValidationRequirement
{
    [SerializeField] private string _tagName;

    private string _tagNameInternal;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        _tagNameInternal = _tagName;

        if (string.IsNullOrEmpty(_tagNameInternal))
            _tagNameInternal = "Untagged";

        AddErrorIf(
            !gameObject.tag.Equals(_tagNameInternal),
            "Incorrect Tag",
            $"Expected tag {_tagNameInternal}, but found {gameObject.tag}",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    /// <summary> Set the tag of the gameObject </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
    private void FixAction(GameObject gameObject)
    {
        if (!IsValidTag())
        {
            Debug.LogWarning("Could not set the gameObject tag, as the targeted tag does not exist.");

            return;
        }

        gameObject.tag = _tagNameInternal;
    }

    /// <summary> Check if the tag is valid </summary>
    /// <returns> If the tag exists </returns>
    private bool IsValidTag()
    {
#if UNITY_EDITOR
        return InternalEditorUtility.tags.Contains(_tagNameInternal);
#else
        return false;
#endif
    }

    #endregion
}

}
