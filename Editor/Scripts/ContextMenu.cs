#if UNITY_EDITOR
using UnityEditor;

namespace Editor.Scripts
{
    public static partial class ContextMenu
    {
        [MenuItem(MenuItemPackages + "/Validators", false, 13)]
        private static void OpenValidatorView() => TryOpen<MegaPintValidators>(false);
    }
}
#endif