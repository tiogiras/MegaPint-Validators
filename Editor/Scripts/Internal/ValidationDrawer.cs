#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Internal
{

[CustomEditor(typeof(ValidatableMonoBehaviourStatus), true)]
internal class ValidationDrawer : UnityEditor.Editor
{
    private const string Status = "User Interface/Status";
    private const string BehaviourEntry = "User Interface/ValidatableMonoBehaviour";
    private const string ErrorEntry = "User Interface/ValidationError";

    private VisualTreeAsset _behaviourEntry;
    private VisualElement _error;
    private VisualTreeAsset _errorEntry;

    private Foldout _errorFoldout;

    private ListView _errorView;

    private Button _fixAllButton;

    private VisualElement _ok;

    private ValidatableMonoBehaviourStatus _status;
    private VisualElement _warning;

    #region Public Methods

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

        _status = (ValidatableMonoBehaviourStatus)target;

        _status.onStatusUpdate += StatusUpdate;

        root.Q <Button>("BTN_Validate").clicked += () => {_status.ValidateStatus();};

        _fixAllButton = root.Q <Button>("BTN_FixAll");
        _fixAllButton.style.display = _status.State == ValidationState.Ok ? DisplayStyle.None : DisplayStyle.Flex;

        _fixAllButton.clicked += FixAll;

        _behaviourEntry = Resources.Load <VisualTreeAsset>(BehaviourEntry);
        _errorEntry = Resources.Load <VisualTreeAsset>(ErrorEntry);

        _errorFoldout = root.Q <Foldout>("ErrorFoldout");

        _errorView = root.Q <ListView>("ErrorView");

        _errorView.makeItem = () => _behaviourEntry.Instantiate();

        _errorView.bindItem = (element, i) =>
        {
            InvalidBehaviour invalidBehaviour = _status.invalidBehaviours[i];

            element.Q <Foldout>().text = invalidBehaviour.behaviourName;
            var errors = element.Q <ListView>("Errors");

            errors.makeItem = () => _errorEntry.Instantiate();

            errors.bindItem = (visualElement, j) =>
            {
                ValidationError error = invalidBehaviour.errors[j];

                var label = visualElement.Q <Label>("Name");
                label.text = error.errorName;
                label.tooltip = error.errorText;

                visualElement.Q <Label>("Ok").style.display = error.severity == ValidationState.Ok
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                visualElement.Q <Label>("Warning").style.display = error.severity == ValidationState.Warning
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                visualElement.Q <Label>("Error").style.display = error.severity == ValidationState.Error
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                var button = visualElement.Q <Button>();

                if (error.fixAction == null)
                    button.style.display = DisplayStyle.None;
                else
                {
                    button.clicked += () =>
                    {
                        error.fixAction.Invoke(error.gameObject);
                        _status.ValidateStatus();
                    };
                }
            };

            errors.itemsSource = invalidBehaviour.errors;
            errors.RefreshItems();
        };

        _status.ValidateStatus();

        return root;
    }

    #endregion

    #region Private Methods

    private void FixAll()
    {
        foreach (ValidationError error in _status.invalidBehaviours.SelectMany(invalidBehaviour => invalidBehaviour.errors))
        {
            if (error.fixAction == null)
                Debug.LogWarning($"No FixAction specified for [{error.errorName}], requires manual attention!");
            else
                error.fixAction.Invoke(error.gameObject);
        }

        _status.ValidateStatus();
    }

    private void StatusUpdate(ValidationState state)
    {
        _ok.style.display = state == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;
        _warning.style.display = state == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;
        _error.style.display = state == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;

        _errorFoldout.style.display = _status.invalidBehaviours.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        _fixAllButton.style.display = _status.State == ValidationState.Ok ? DisplayStyle.None : DisplayStyle.Flex;

        List <InvalidBehaviour> invalidBehaviours = _status.invalidBehaviours;
        invalidBehaviours.Sort();

        _errorView.itemsSource = invalidBehaviours;
        _errorView.RefreshItems();
    }

    #endregion
}

}
#endif
