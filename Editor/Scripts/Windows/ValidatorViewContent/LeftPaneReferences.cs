#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

/// <summary> Stores visual references for the leftPane tabs of the <see cref="ValidatorView" /> </summary>
internal struct LeftPaneReferences
{
    public Toggle showChildren;
    public DropdownField searchMode;
    public VisualElement parentChangePath;
    public Button btnChangePath;
    public Label path;
}

}
#endif
