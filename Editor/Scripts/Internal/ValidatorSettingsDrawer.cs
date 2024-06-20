#if UNITY_EDITOR
using UnityEditor;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> This class is needed to allow the requirements to draw in custom editors and inspector</summary>
[CustomEditor(typeof(ValidatorSettings), true)]
internal class ValidatorSettingsDrawer : UnityEditor.Editor
{
}

}
#endif
