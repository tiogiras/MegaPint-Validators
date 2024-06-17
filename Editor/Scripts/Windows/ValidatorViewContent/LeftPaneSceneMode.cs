using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

// TODO commenting
internal static class LeftPaneSceneMode
{
    private static LeftPaneReferences s_refs;

    public static bool CollectInvalidObjects(out List<ValidatableMonoBehaviourStatus> errors, out List<ValidatableMonoBehaviourStatus> warnings, out List <ValidatableMonoBehaviourStatus> ok)
    {
        ValidatableMonoBehaviourStatus[] behaviours = Resources.FindObjectsOfTypeAll <ValidatableMonoBehaviourStatus>();
        behaviours = behaviours.Where(behaviour => behaviour.gameObject.scene.isLoaded).ToArray();
        
        if (!SaveValues.Validators.ShowChildren)
            behaviours = behaviours.Where(behaviour => !IsChildValidation(behaviour.transform)).ToArray();
        
        errors = new List<ValidatableMonoBehaviourStatus>();
        warnings = new List <ValidatableMonoBehaviourStatus>();
        ok = new List <ValidatableMonoBehaviourStatus>();

        if (behaviours.Length == 0)
            return false;

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            switch (behaviour.State)
            {
                case ValidationState.Unknown: break;

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

    private static bool IsChildValidation(Transform transform)
    {
        if (transform.parent == null)
            return false;

        ValidatableMonoBehaviourStatus[] behaviours =
            transform.parent.GetComponentsInParent <ValidatableMonoBehaviourStatus>();

        return behaviours.Length != 0 && behaviours.Any(behaviour => behaviour.ValidatesChildren());
    }

    public static void SetReferences(LeftPaneReferences refs)
    {
        s_refs = refs;
    }
    
    public static void UpdateGUI()
    {
        s_refs.showChildren.value = SaveValues.Validators.ShowChildren;
        
        s_refs.searchMode.style.display = DisplayStyle.None;
        s_refs.parentChangePath.style.display = DisplayStyle.None;
        s_refs.btnChangePath.style.display = DisplayStyle.None;
        s_refs.path.style.display = DisplayStyle.None;
    }
    
    public static void RegisterCallbacks()
    {
        s_refs.showChildren.RegisterValueChangedCallback(OnShowChildrenChanged);
    }

    public static void UnRegisterCallbacks()
    {
        s_refs.showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);
    }
    
    private static void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        if (evt.newValue == SaveValues.Validators.ShowChildren)
            return;
        
        SaveValues.Validators.ShowChildren = evt.newValue;
        ValidatorView.onRefresh?.Invoke();
    }
}

}
