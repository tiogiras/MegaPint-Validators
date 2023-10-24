#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace Editor.Scripts
{

public static partial class DisplayContent
{
    
    #region Private Methods

    private static partial void Validators(VisualElement root)
    {
        var tabs = root.Q <GroupBox>("Tabs");
        var tabContentParent = root.Q <GroupBox>("TabContent");
        
        RegisterTabCallbacks(tabs, tabContentParent, 3);
        
        SetTabContentLocations(
            "User Interface/Display Content Tabs/Tab0",
            "User Interface/Display Content Tabs/Tab1",
            "User Interface/Display Content Tabs/Tab2");
        
        SwitchTab(tabContentParent, 0);
    }

    #endregion
}

}
#endif
