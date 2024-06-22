#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

/// <summary> Handles the display of the left pane of <see cref="ValidatorView" /> in scene mode </summary>
internal static class LeftPaneSceneMode
{
    private static LeftPaneReferences s_refs;

    #region Public Methods

    /// <summary> Collect all <see cref="ValidatableMonoBehaviourStatus" /> based on the current settings </summary>
    /// <param name="errors"> Found behaviours with severity = error </param>
    /// <param name="warnings"> Found behaviours with severity = warning </param>
    /// <param name="ok"> Found behaviours with severity = ok </param>
    /// <returns> If any behaviours where found </returns>
    /// <exception cref="ArgumentOutOfRangeException"> State of the behaviour not found </exception>
    public static bool CollectValidatableObjects(
        out List <ValidatableMonoBehaviourStatus> errors,
        out List <ValidatableMonoBehaviourStatus> warnings,
        out List <ValidatableMonoBehaviourStatus> ok)
    {
        ValidatableMonoBehaviourStatus[] behaviours = Resources.FindObjectsOfTypeAll <ValidatableMonoBehaviourStatus>();
        behaviours = behaviours.Where(behaviour => behaviour.gameObject.scene.isLoaded).ToArray();

        if (!SaveValues.Validators.ShowChildren)
            behaviours = behaviours.Where(behaviour => !IsChildValidation(behaviour.transform)).ToArray();

        errors = new List <ValidatableMonoBehaviourStatus>();
        warnings = new List <ValidatableMonoBehaviourStatus>();
        ok = new List <ValidatableMonoBehaviourStatus>();

        if (behaviours.Length == 0)
            return false;

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            behaviour.ValidateStatus();

            switch (behaviour.State)
            {
                case ValidationState.Unknown:
                    break;

                case ValidationState.Ok:
                    ok.Add(behaviour);

                    break;

                case ValidationState.Warning:
                    warnings.Add(behaviour);

                    break;

                case ValidationState.Error:
                    errors.Add(behaviour);

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
    }

    /// <summary> Register all callbacks </summary>
    public static void RegisterCallbacks()
    {
        s_refs.showChildren.RegisterValueChangedCallback(OnShowChildrenChanged);
    }

    /// <summary> Set the values of the visual references </summary>
    /// <param name="refs"> Visual references </param>
    public static void SetReferences(LeftPaneReferences refs)
    {
        s_refs = refs;
    }

    /// <summary> Unregister all callbacks </summary>
    public static void UnRegisterCallbacks()
    {
        s_refs.showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);
    }

    /// <summary> Update the gui </summary>
    public static void UpdateGUI()
    {
        s_refs.showChildren.value = SaveValues.Validators.ShowChildren;

        s_refs.searchMode.style.display = DisplayStyle.None;
        s_refs.parentChangePath.style.display = DisplayStyle.None;
        s_refs.btnChangePath.style.display = DisplayStyle.None;
        s_refs.path.style.display = DisplayStyle.None;
    }

    #endregion

    #region Private Methods

    /// <summary> If this object is required for a child validation in any parent </summary>
    /// <param name="transform"> Transform of the targeted object </param>
    /// <returns> If the object is validated by a parent </returns>
    private static bool IsChildValidation(Transform transform)
    {
        if (transform.parent == null)
            return false;

        ValidatableMonoBehaviourStatus[] behaviours =
            transform.parent.GetComponentsInParent <ValidatableMonoBehaviourStatus>();

        return behaviours.Length != 0 && behaviours.Any(behaviour => behaviour.ValidatesChildren());
    }

    /// <summary> Show children toggle callback </summary>
    /// <param name="evt"> Callback event </param>
    private static void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        if (evt.newValue == SaveValues.Validators.ShowChildren)
            return;

        SaveValues.Validators.ShowChildren = evt.newValue;
        ValidatorView.onRefresh?.Invoke();
    }

    #endregion
}

}
#endif
