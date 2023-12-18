#if UNITY_EDITOR

using UnityEditor;

namespace Editor.Scripts.Internal
{

// This class is needed since the requirements only draw in custom editors
[CustomEditor(typeof(ValidatorSettings), true)]
internal class ValidatorSettingsDrawer : UnityEditor.Editor
{
}

}

#endif
