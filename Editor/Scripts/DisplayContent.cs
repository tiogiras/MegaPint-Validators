#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace Editor.Scripts
{

internal static partial class DisplayContent
{
    private const string BasePathValidators = "Validators/User Interface/Display Content Tabs/";
    
    #region Private Methods

    private static void UnsubscribeValidators()
    {
        s_onSelectedTabChanged -= OnTabChangedValidators;
        s_onSelectedPackageChanged -= UnsubscribeValidators;
    }

    // Called by reflection
    // ReSharper disable once UnusedMember.Local
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

    #endregion
}

}
#endif
