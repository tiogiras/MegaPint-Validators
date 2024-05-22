#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Internal
{

[CustomEditor(typeof(ValidatableMonoBehaviourStatus), true)]
internal class ValidationDrawer : UnityEditor.Editor
{
    private const string BasePath = "Validators/User Interface/Status";

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
        var statusFile = Resources.Load <VisualTreeAsset>(BasePath);

        VisualElement root = GUIUtility.Instantiate(statusFile);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;

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

        _fixAllButton.clicked += _status.FixAll;

        _behaviourEntry = Resources.Load <VisualTreeAsset>(Path.Combine(BasePath, "Behaviour"));
        _errorEntry = Resources.Load <VisualTreeAsset>(Path.Combine(BasePath, "Error"));

        _errorFoldout = root.Q <Foldout>("ErrorFoldout");

        _errorView = root.Q <ListView>("ErrorView");

        _errorView.makeItem = () => GUIUtility.Instantiate(_behaviourEntry);

        _errorView.bindItem = (element, i) =>
        {
            InvalidBehaviour invalidBehaviour = _status.invalidBehaviours[i];

            element.Q <Foldout>().text = invalidBehaviour.behaviourName;
            var errors = element.Q <ListView>("Errors");

            errors.makeItem = () => GUIUtility.Instantiate(_errorEntry);

            errors.bindItem = (visualElement, j) =>
            {
                try
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
                }
                catch (Exception)
                {
                    // ignored
                }
            };

            errors.itemsSource = invalidBehaviour.errors;
            errors.RefreshItems();
        };

        _status.ValidateStatus();

        root.schedule.Execute(
            () =>
            {
                root.parent.styleSheets.Add(Resources.Load <StyleSheet>(StyleSheetClasses.BaseStyleSheetPath));
                root.parent.styleSheets.Add(Resources.Load <StyleSheet>(StyleSheetClasses.AttributeStyleSheetPath));

                GUIUtility.ApplyRootElementTheme(root.parent);
                root.parent.AddToClassList(StyleSheetClasses.Background.Color.Secondary);
            });
        
        return root;
    }

    #endregion

    #region Private Methods

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
