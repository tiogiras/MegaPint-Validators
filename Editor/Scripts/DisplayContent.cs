#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace Editor.Scripts
{

internal static partial class DisplayContent
{
    private const string BasePath = "Validators/User Interface/Display Content Tabs/";
    
    #region Private Methods

    private static void Unsubscribe()
    {
        s_onSelectedTabChanged -= ValidatorsOnTabChanged;
        s_onSelectedPackageChanged -= Unsubscribe;
    }

    // Called by reflection
    private static void Validators(VisualElement root)
    {
        var tabs = root.Q <GroupBox>("Tabs");
        var tabContentParent = root.Q <GroupBox>("TabContent");

        RegisterTabCallbacks(tabs, tabContentParent, 3);

        SetTabContentLocations(BasePath + "Tab0", BasePath + "Tab1", BasePath + "Tab2");

        s_onSelectedTabChanged += ValidatorsOnTabChanged;
        s_onSelectedPackageChanged += Unsubscribe;

        SwitchTab(tabContentParent, 0);
    }

    private static void ValidatorsOnTabChanged(int tab, VisualElement root)
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
