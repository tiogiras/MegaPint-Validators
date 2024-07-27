#if UNITY_EDITOR
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Windows;
using UnityEditor;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class used to store MenuItems </summary>
internal static partial class ContextMenu
{
    public static class Validators
    {
        private static readonly MenuItemSignature s_validatorViewSignature = new()
        {
            package = PackageKey.PlayModeStartScene, signature = "Validator View"
        };

        #region Private Methods

        [MenuItem(MenuItemPackages + "/Validator View", false, 13)]
        private static void OpenValidatorView()
        {
            TryOpen <ValidatorView>(false, s_validatorViewSignature);
        }

        #endregion
    }
}

}
#endif
