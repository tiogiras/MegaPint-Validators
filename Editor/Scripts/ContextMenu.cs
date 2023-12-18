#if UNITY_EDITOR
using UnityEditor;

namespace Editor.Scripts
{

internal static partial class ContextMenu
{
    #region Private Methods

    [MenuItem(MenuItemPackages + "/Validators", false, 13)]
    private static void OpenValidatorView()
    {
        TryOpen <MegaPintValidators>(false);
    }

    #endregion
}

}
#endif
