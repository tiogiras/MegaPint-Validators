#if UNITY_EDITOR
namespace Editor.Scripts
{

internal static class MegaPintValidatorsSaveData
{
    public struct SettingsValue <T>
    {
        public string key;
        public T defaultValue;
    }

    public static SettingsValue <bool> showChildren = new() {key = "showChildren", defaultValue = false};

    public static SettingsValue <bool> showChildrenProject = new() {key = "showChildrenProject", defaultValue = false};

    public static SettingsValue <int> searchMode = new() {key = "searchMode", defaultValue = 0};

    public static SettingsValue <string> searchFolder = new() {key = "searchFolder", defaultValue = ""};

    public static string SettingsName => "MegaPint.Validators";
}

}
#endif
