using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{

[CustomEditor(typeof(ValidatableMonoBehaviourStatus), true)]
public class ValidationDrawer : UnityEditor.Editor
{
    private const string Status = "User Interface/Status";

    private VisualElement _ok;
    private VisualElement _warning;
    private VisualElement _error;

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var statusFile = Resources.Load <VisualTreeAsset>(Status);
        TemplateContainer status = statusFile.Instantiate();
        root.Add(status);

        _ok = root.Q <VisualElement>("Ok");
        _warning = root.Q <VisualElement>("Warning");
        _error = root.Q <VisualElement>("Error");

        _ok.style.display = DisplayStyle.None;
        _warning.style.display = DisplayStyle.None;
        _error.style.display = DisplayStyle.None;

        var castedTarget = ((ValidatableMonoBehaviourStatus)target);
        
        castedTarget.onStatusUpdate += state =>
        {
            _ok.style.display = state == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;
            _warning.style.display = state == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;
            _error.style.display = state == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;
        };
        
        castedTarget.ValidateStatus();

        return root;
    }
}

}
