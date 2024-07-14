using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Validation requirement requires specific static flags on the gameObject </summary>
[Serializable]
[ValidationRequirementTooltip("This requirement enforces the static flags of the gameObject.")]
[ValidationRequirementName("GameObject/Is Static", typeof(RequireIsStatic), -30, 3)]
internal class RequireIsStatic : ScriptableValidationRequirement
{
#if UNITY_EDITOR
    public StaticEditorFlags staticFlags;
#endif

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
#if UNITY_EDITOR

        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(gameObject);

        AddErrorIf(
            flags != staticFlags,
            "Incorrect Static Flags",
            $"Expected flags {staticFlags}, but the flags are {flags}",
            ValidationState.Warning,
            FixAction);
#endif
    }

    #endregion

    #region Private Methods

    /// <summary> Set the correct static flags on the gameObject </summary>
    /// <param name="gameObject"> Target gameObject </param>
    private void FixAction(GameObject gameObject)
    {
#if UNITY_EDITOR
        GameObjectUtility.SetStaticEditorFlags(gameObject, staticFlags);
#endif
    }

    #endregion
}

}
