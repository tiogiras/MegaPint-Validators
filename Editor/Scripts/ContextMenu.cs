#if UNITY_EDITOR
using MegaPint.Editor.Scripts.Windows;
using UnityEditor;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class used to store MenuItems </summary>
internal static partial class ContextMenu
{
    #region Private Methods

    [MenuItem(MenuItemPackages + "/Validator View", false, 13)]
    private static void OpenValidatorView()
    {
        TryOpen <Validators>(false);
    }

    #endregion
}

}
#endif
