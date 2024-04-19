#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts
{

internal static partial class DisplayContent
{
    
    // Called by reflection
    // ReSharper disable once UnusedMember.Local
    private static void Validators(DisplayContentReferences refs)
    {
        InitializeDisplayContent(
            refs,
            new TabSettings
            {
                info = true,
                guides = true,
                help = true
            },
            new TabActions
            {
                info = ValidatorsActivateLinks,
                guides = ValidatorsActivateLinks,
                help = ValidatorsActivateLinks
            });
    }

    private static void ValidatorsActivateLinks(VisualElement root)
    {
        GUIUtility.ActivateLinks(root,
                                 evt =>
                                 {
                                     switch (evt.linkID)
                                     {
                                         case "validatorView":
                                             EditorApplication.ExecuteMenuItem(
                                                 "MegaPint/Packages/Validator View");
                                             break;
                                         
                                         case "validatorSettings":
                                             EditorApplication.ExecuteMenuItem(
                                                 "Assets/Create/MegaPint/Validators/Validator Settings");
                                             break;
                                         
                                         case "componentOrderConfig":
                                             EditorApplication.ExecuteMenuItem(
                                                 "Assets/Create/MegaPint/Validators/Component Order Config");
                                             break;
                                     }
                                 });
    }
    
    /*#region Private Methods

    private static void UnsubscribeValidators()
    {
        s_onSelectedTabChanged -= OnTabChangedValidators;
        s_onSelectedPackageChanged -= UnsubscribeValidators;
    }


    private static void Validators(VisualElement root)
    {
        var tabs = root.Q <GroupBox>("Tabs");
        var tabContentParent = root.Q <GroupBox>("TabContent");

        RegisterTabCallbacks(tabs, tabContentParent, 3);

        SetTabContentLocations(BasePathValidators + "Tab0", BasePathValidators + "Tab1", BasePathValidators + "Tab2");

        s_onSelectedTabChanged += OnTabChangedValidators;
        s_onSelectedPackageChanged += UnsubscribeValidators;

        SwitchTab(tabContentParent, 0);
    }

    private static void OnTabChangedValidators(int tab, VisualElement root)
    {
        if (tab != 0)
            return;

        root.Q <Button>("BTN_ValidatorView").clickable = new Clickable(
            _ => {ContextMenu.TryOpen <MegaPintValidators>(false);});
    }

    #endregion*/
}

}
#endif
