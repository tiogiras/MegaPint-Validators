#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="ValidatableMonoBehaviourStatus" /> class </summary>
[CustomEditor(typeof(ValidatableMonoBehaviourStatus), true)]
internal class ValidationDrawer : UnityEditor.Editor
{
    public static Action <string> onValidateButton;
    public static Action <string> onFixAll;
    public static Action <string, string> onIssueFixed;
    
    private static readonly string s_basePath = Constants.Validators.UserInterface.Status;

    private VisualTreeAsset _behaviourEntry;
    private VisualElement _error;
    private VisualTreeAsset _errorEntry;

    private Foldout _errorFoldout;

    private Button _fixAllButton;

    private ListView _invalidBehaviours;
    private VisualElement _noFixAction;

    private VisualElement _ok;

    private ValidatableMonoBehaviourStatus _status;
    private VisualElement _warning;

    #region Public Methods

    public override VisualElement CreateInspectorGUI()
    {
        var statusFile = Resources.Load <VisualTreeAsset>(s_basePath);

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

        root.Q <Button>("BTN_Validate").clicked += () =>
        {
            onValidateButton?.Invoke(_status.gameObject.name);
            
            _status.ValidateStatus();
        };

        _fixAllButton = root.Q <Button>("BTN_FixAll");
        UpdateFixAllButton();

        _fixAllButton.clicked += () =>
        {
            onFixAll?.Invoke(_status.gameObject.name);
            
            _status.FixAll();
        };

        _noFixAction = root.Q <VisualElement>("NoFixAction");
        UpdateNoFixAction();

        _behaviourEntry = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusBehaviour);
        _errorEntry = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusError);

        _errorFoldout = root.Q <Foldout>("ErrorFoldout");

        _invalidBehaviours = root.Q <ListView>("ErrorView");

        _invalidBehaviours.makeItem = () => GUIUtility.Instantiate(_behaviourEntry);

        _invalidBehaviours.bindItem = (element, i) =>
        {
            InvalidBehaviour invalidBehaviour = _status.invalidBehaviours[i];

            var foldout = element.Q <Foldout>();
            foldout.text = invalidBehaviour.shortBehaviourName;
            foldout.tooltip = invalidBehaviour.behaviourName;

            var errors = element.Q <ListView>("Errors");

            errors.makeItem = () => GUIUtility.Instantiate(_errorEntry);

            errors.bindItem = (visualElement, j) =>
            {
                if (j >= invalidBehaviour.errors.Count)
                    return;

                ValidationError error = invalidBehaviour.errors[j];

                var label = visualElement.Q <Button>("Name");
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

                var hasFixAction = error.fixAction != null;

                var button = visualElement.Q <Button>("BTN_Fix");
                button.style.display = hasFixAction ? DisplayStyle.Flex : DisplayStyle.None;

                var noFixAction = visualElement.Q <VisualElement>("NoFixAction");
                noFixAction.style.display = hasFixAction ? DisplayStyle.None : DisplayStyle.Flex;

                if (hasFixAction)
                {
                    button.clicked += () =>
                    {
                        onIssueFixed?.Invoke(_status.gameObject.name, error.errorName);
                        
                        error.fixAction.Invoke(error.gameObject);
                        _status.ValidateStatus();
                    };
                }
            };

            errors.itemsSource = invalidBehaviour.errors;
            errors.RefreshItems();
        };

        _status.ValidateStatus();

        root.schedule.Execute(
            () =>
            {
                root.parent.styleSheets.Add(StyleSheetValues.BaseStyleSheet);
                root.parent.styleSheets.Add(StyleSheetValues.AttributesStyleSheet);

                GUIUtility.ApplyRootElementTheme(root.parent);
                root.parent.AddToClassList(StyleSheetClasses.Background.Color.Secondary);
            });

        return root;
    }

    #endregion

    #region Private Methods

    /// <summary> Update the displayed status </summary>
    /// <param name="state"> New status to display </param>
    private void StatusUpdate(ValidationState state)
    {
        _ok.style.display = state == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;
        _warning.style.display = state == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;
        _error.style.display = state == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;

        _errorFoldout.style.display = _status.invalidBehaviours.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        UpdateFixAllButton();
        UpdateNoFixAction();

        List <InvalidBehaviour> invalidBehaviours = _status.invalidBehaviours;
        invalidBehaviours.Sort();

        _invalidBehaviours.itemsSource = invalidBehaviours;
        _invalidBehaviours.RefreshItems();
    }

    /// <summary> Update the visibility of the fix all button </summary>
    private void UpdateFixAllButton()
    {
        if (_status.State == ValidationState.Ok)
        {
            _fixAllButton.style.display = DisplayStyle.None;

            return;
        }

        var hasFixActions =
            _status.invalidBehaviours.Any(behaviour => behaviour.errors.Any(error => error.fixAction != null));

        _fixAllButton.style.display = hasFixActions ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary> Update the visibility of the no fix action message </summary>
    private void UpdateNoFixAction()
    {
        if (_status.State == ValidationState.Ok)
        {
            _noFixAction.style.display = DisplayStyle.None;

            return;
        }

        var missingFixAction =
            _status.invalidBehaviours.Any(behaviour => behaviour.errors.Any(error => error.fixAction == null));

        _noFixAction.style.display = missingFixAction ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion
}

}
#endif
