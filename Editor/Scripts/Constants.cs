#if UNITY_EDITOR
using System.IO;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial lookup table for constants containing Validators values  </summary>
internal static partial class Constants
{
    public static class Validators
    {
        public static class Images
        {
            private static readonly string s_images = Path.Combine(s_base, "Images");
            public static readonly string ManualIssue = Path.Combine(s_images, "Manual Issue");
            public static readonly string Refresh = Path.Combine(s_images, "Refresh");
        }

        public static class Links
        {
            public const string CreateValidatorSettings = "Assets/Create/MegaPint/Validators/Validator Settings";
            public const string CreateComponentOrderConfig = "Assets/Create/MegaPint/Validators/Component Order Config";

            public static readonly string ValidatorView = Utility.CombineMenuItemPath(
                ContextMenu.MenuItemPackages,
                "Validator View");
        }

        public static class Tests
        {
            private static readonly string s_tests = Path.Combine(s_base, "Tests");

            public static readonly string RequireGameObjectActive = Path.Combine(
                s_tests,
                "GameObject",
                "[Test] Require Game Object Active");

            public static readonly string RequireChildrenValidation = Path.Combine(
                s_tests,
                "[Test] Require Children Validation");

            private static readonly string s_customNaming = Path.Combine(s_tests, "Custom Naming");

            public static readonly string RequireCustomNaming = Path.Combine(
                s_customNaming,
                "[Test] Require Custom Naming");

            public static readonly string RequireCustomNaming1 = Path.Combine(
                s_customNaming,
                "[Test] Require Custom Naming 1");

            public static readonly string RequireCustomNaming2 = Path.Combine(
                s_customNaming,
                "[Test] Require Custom Naming 2");

            private static readonly string s_transform = Path.Combine(s_tests, "Transform");
            private static readonly string s_defaultTransform = Path.Combine(s_transform, "Default");

            public static readonly string RequireDefaultTransform = Path.Combine(
                s_defaultTransform,
                "[Test] Require Default Transform");

            public static readonly string RequireDefaultTransform1 = Path.Combine(
                s_defaultTransform,
                "[Test] Require Default Transform 1");

            public static readonly string RequireDefaultTransform2 = Path.Combine(
                s_defaultTransform,
                "[Test] Require Default Transform 2");

            public static readonly string RequireDefaultTransform3 = Path.Combine(
                s_defaultTransform,
                "[Test] Require Default Transform 3");

            public static readonly string RequireDefaultTransform4 = Path.Combine(
                s_defaultTransform,
                "[Test] Require Default Transform 4");

            private static readonly string s_globalTransform = Path.Combine(s_transform, "Global");

            public static readonly string RequireGlobalTransform = Path.Combine(
                s_globalTransform,
                "[Test] Require Global Transform");

            public static readonly string RequireGlobalTransform1 = Path.Combine(
                s_globalTransform,
                "[Test] Require Global Transform 1");

            public static readonly string RequireGlobalTransform2 = Path.Combine(
                s_globalTransform,
                "[Test] Require Global Transform 2");

            public static readonly string RequireGlobalTransform3 = Path.Combine(
                s_globalTransform,
                "[Test] Require Global Transform 3");

            public static readonly string RequireGlobalTransform4 = Path.Combine(
                s_globalTransform,
                "[Test] Require Global Transform 4");

            private static readonly string s_localTransform = Path.Combine(s_transform, "Local");

            public static readonly string RequireLocalTransform = Path.Combine(
                s_localTransform,
                "[Test] Require Local Transform");

            public static readonly string RequireLocalTransform1 = Path.Combine(
                s_localTransform,
                "[Test] Require Local Transform 1");

            public static readonly string RequireLocalTransform2 = Path.Combine(
                s_localTransform,
                "[Test] Require Local Transform 2");

            public static readonly string RequireLocalTransform3 = Path.Combine(
                s_localTransform,
                "[Test] Require Local Transform 3");

            public static readonly string RequireLocalTransform4 = Path.Combine(
                s_localTransform,
                "[Test] Require Local Transform 4");
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
            
            public static readonly string Variable = Path.Combine(s_windows, "Variable");
            
            public static readonly string APIItem = Path.Combine(s_userInterface, "Display Content", "API", "Api Item");
        }

        private static readonly string s_base = Path.Combine("MegaPint", "Validators");
        private static readonly string s_userInterface = Path.Combine(s_base, "User Interface");
    }
}

}
#endif
