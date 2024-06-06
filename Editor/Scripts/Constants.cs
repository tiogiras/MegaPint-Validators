#if UNITY_EDITOR
using System.IO;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial lookup table for constants containing Validators values  </summary>
internal static partial class Constants
{
    public static class Validators
    {
        public static class Links
        {
            public const string CreateValidatorSettings = "Assets/Create/MegaPint/Validators/";
            public const string CreateComponentOrderConfig = "Assets/Create/MegaPint/Validators/";

            public static readonly string ValidatorView = Utility.CombineMenuItemPath(
                ContextMenu.MenuItemPackages,
                "Validator View");
        }

        public static class UserInterface
        {
            private static readonly string s_windows = Path.Combine(s_userInterface, "Windows");
            private static readonly string s_componentOrder = Path.Combine(s_windows, "Component Order");
            public static readonly string ComponentOrderConfig = Path.Combine(s_componentOrder, "Config");
            public static readonly string ComponentOrderTypeEntry = Path.Combine(s_componentOrder, "Type Entry");

            public static readonly string Status = Path.Combine(s_windows, "Status");
            public static readonly string StatusBehaviour = Path.Combine(Status, "Behaviour");
            public static readonly string StatusError = Path.Combine(Status, "Error");

            public static readonly string ValidatorView = Path.Combine(s_windows, "Validator View");
            public static readonly string ValidatorViewItem = Path.Combine(ValidatorView, "Item");
        }

        private static readonly string s_base = Path.Combine("MegaPint", "Validators");
        private static readonly string s_userInterface = Path.Combine(s_base, "User Interface");
    }
}

}
#endif
