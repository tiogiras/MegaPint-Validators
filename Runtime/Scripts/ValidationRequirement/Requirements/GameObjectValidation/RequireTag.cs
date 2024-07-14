using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

[Serializable]
[TypeTooltip("This requirement enforces the tag of the gameObject.")]
[SerializeReferenceDropdownName("GameObject/Tag", typeof(RequireTag), -30, 2)]
public class RequireTag : ScriptableValidationRequirement
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

    private void FixAction(GameObject gameObject)
    {
        if (!IsValidTag())
        {
            Debug.LogWarning("Could not set the gameObject tag, as the targeted tag does not exist.");

            return;
        }

        gameObject.tag = _tagNameInternal;
    }

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
