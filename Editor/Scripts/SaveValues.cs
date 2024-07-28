#if UNITY_EDITOR
using MegaPint.Editor.Scripts.Settings;

namespace MegaPint.Editor.Scripts
{

/// <summary> Partial class storing saveData values (Validators) </summary>
internal static partial class SaveValues
{
    public static class Validators
    {
        private static CacheValue <bool> s_showChildren = new() {defaultValue = false};
        private static CacheValue <bool> s_showChildrenProject = new() {defaultValue = false};
        private static CacheValue <int> s_searchMode = new() {defaultValue = 0};
        private static CacheValue <string> s_searchFolder = new() {defaultValue = ""};
        private static CacheValue <bool> s_applyPSValidatorView = new() {defaultValue = true};

        private static SettingsBase s_settings;

        public static bool ShowChildren
        {
            get => ValueProperty.Get("showChildren", ref s_showChildren, _Settings);
            set => ValueProperty.Set("showChildren", value, ref s_showChildren, _Settings);
        }

        public static bool ShowChildrenProject
        {
            get => ValueProperty.Get("showChildrenProject", ref s_showChildrenProject, _Settings);
            set => ValueProperty.Set("showChildrenProject", value, ref s_showChildrenProject, _Settings);
        }

        public static int SearchMode
        {
            get => ValueProperty.Get("searchMode", ref s_searchMode, _Settings);
            set => ValueProperty.Set("searchMode", value, ref s_searchMode, _Settings);
        }

        public static string SearchFolder
        {
            get => ValueProperty.Get("searchFolder", ref s_searchFolder, _Settings);
            set => ValueProperty.Set("searchFolder", value, ref s_searchFolder, _Settings);
        }

        public static bool ApplyPSValidatorView
        {
            get => ValueProperty.Get("ApplyPS_ValidatorView", ref s_applyPSValidatorView, _Settings);
            set => ValueProperty.Set("ApplyPS_ValidatorView", value, ref s_applyPSValidatorView, _Settings);
        }

        private static SettingsBase _Settings
        {
            get
            {
                if (MegaPintMainSettings.Exists())
                    return s_settings ??= MegaPintMainSettings.instance.GetSetting("Validators");

                return null;
            }
        }
    }
}

}
#endif
