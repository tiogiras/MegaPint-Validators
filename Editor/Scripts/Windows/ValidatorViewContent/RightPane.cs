#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

/// <summary> Handles the display of the right pane of <see cref="ValidatorView" /> </summary>
internal static class RightPane
{
    private static Label s_gameObjectName;
    private static Label s_gameObjectPath;

    private static VisualElement s_content;
    private static VisualElement s_errorPanel;
    private static VisualElement s_noIssue;

    private static ListView s_invalidBehavioursView;

    private static ValidatableMonoBehaviourStatus s_status;
    private static List <InvalidBehaviour> s_invalidBehaviours;

    private static VisualTreeAsset s_invalidBehaviourItem;
    private static VisualTreeAsset s_errorItem;

    private static Button s_btnFixAll;

    private static readonly Dictionary <Foldout, bool> s_foldoutStates = new();

    private static bool s_ignoreUnbindEvent;

    #region Public Methods

    /// <summary> Clear the right pane </summary>
    public static void Clear()
    {
        s_foldoutStates.Clear();

        UnregisterCallbacks();

        s_content.style.display = DisplayStyle.None;

        s_invalidBehavioursView.itemsSource = null;
        s_invalidBehavioursView.RefreshItems();
    }

    /// <summary> Instantiate the right pane </summary>
    /// <param name="root"> VisualRootElement of the right pane </param>
    /// <param name="invalidBehaviourItem"> Template item for the invalidBehaviour entries </param>
    /// <param name="errorItem"> Template item for the error entries </param>
    public static void CreateGUI(VisualElement root, VisualTreeAsset invalidBehaviourItem, VisualTreeAsset errorItem)
    {
        s_content = root.Q <VisualElement>("RightPaneContent");

        s_gameObjectName = s_content.Q <Label>("GameObjectName");
        s_gameObjectPath = s_content.Q <Label>("Path");
        s_errorPanel = s_content.Q <VisualElement>("ErrorPanel");
        s_noIssue = s_content.Q <VisualElement>("NoIssue");

        s_invalidBehavioursView = s_content.Q <ListView>("InvalidBehaviours");

        s_invalidBehaviourItem = invalidBehaviourItem;
        s_errorItem = errorItem;

        s_btnFixAll = s_content.Q <Button>("BTN_FixAll");

        s_content.style.display = DisplayStyle.None;
    }

    /// <summary> Display the given <see cref="ValidatableMonoBehaviourStatus" /> in the right pane </summary>
    /// <param name="status"> Targeted <see cref="ValidatableMonoBehaviourStatus" /> </param>
    /// <param name="path"> Path to the status object </param>
    public static void Display(ValidatableMonoBehaviourStatus status, string path)
    {
        Clear();

        s_status = status;

        var name = status.gameObject.name;
        s_gameObjectName.text = name;
        s_gameObjectName.tooltip = name;

        s_gameObjectPath.text = path;
        s_gameObjectPath.tooltip = path;

        RegisterCallbacks();

        s_content.style.display = DisplayStyle.Flex;

        s_content.schedule.Execute(Refresh);
    }

    #endregion

    #region Private Methods

    /// <summary> Fix all issues </summary>
    private static void FixAll()
    {
        s_status.FixAll();
        ValidatorView.UpdateBehaviourBasedOnState(s_status);

        Refresh();
    }

    /// <summary> Refresh the right pane </summary>
    private static void Refresh()
    {
        var hasErrors = s_status.invalidBehaviours.Count > 0;

        s_errorPanel.style.display = hasErrors ? DisplayStyle.Flex : DisplayStyle.None;
        s_noIssue.style.display = hasErrors ? DisplayStyle.None : DisplayStyle.Flex;

        if (!hasErrors)
            return;

        s_btnFixAll.style.display = s_status.invalidBehaviours.Any(
            invalidBehaviour => invalidBehaviour.errors.Any(error => error.fixAction != null))
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        s_invalidBehaviours = s_status.invalidBehaviours;

        // Set correct items and await scheduled event
        s_ignoreUnbindEvent = true;
        s_invalidBehavioursView.itemsSource = s_invalidBehaviours;
        s_invalidBehavioursView.style.display = DisplayStyle.None;
        s_ignoreUnbindEvent = false;

        s_invalidBehavioursView.schedule.Execute(
            () =>
            {
                // For some odd reason the list view updates but doesn't show the items in some cases
                // By setting the display to none and then back to flex it forces the list view to update
                s_ignoreUnbindEvent = true;
                s_invalidBehavioursView.style.display = DisplayStyle.Flex;
                s_invalidBehavioursView.RefreshItems();
                s_ignoreUnbindEvent = false;
            });
    }

    /// <summary> Register all callbacks </summary>
    private static void RegisterCallbacks()
    {
        s_btnFixAll.clicked += FixAll;

        s_invalidBehavioursView.makeItem = () => GUIUtility.Instantiate(s_invalidBehaviourItem);

        s_invalidBehavioursView.bindItem = (element, i) =>
        {
            InvalidBehaviour invalidBehaviour = s_invalidBehaviours[i];

            var foldout = element.Q <Foldout>();

            foldout.text = invalidBehaviour.shortBehaviourName;
            foldout.Q(className: "unity-foldout__text").tooltip = invalidBehaviour.behaviourName;

            if (s_foldoutStates.TryGetValue(foldout, out var state))
                foldout.value = state;
            else
            {
                s_foldoutStates.Add(foldout, false);
                foldout.value = false;
            }

            foldout.RegisterValueChangedCallback(
                evt => {s_foldoutStates[foldout] = evt.newValue;});

            var errorsView = element.Q <ListView>("Errors");

            RegisterErrorCallbacks(errorsView, invalidBehaviour.errors);

            errorsView.itemsSource = invalidBehaviour.errors;
        };

        s_invalidBehavioursView.unbindItem = (element, _) =>
        {
            if (!s_ignoreUnbindEvent)
                s_foldoutStates.Remove(element.Q <Foldout>());
        };
    }

    /// <summary> Register all callbacks for an error listView </summary>
    /// <param name="errorsView"> Targeted listView </param>
    /// <param name="errors"> List of errors to display </param>
    private static void RegisterErrorCallbacks(ListView errorsView, IReadOnlyList <ValidationError> errors)
    {
        errorsView.makeItem = () => GUIUtility.Instantiate(s_errorItem);

        errorsView.bindItem = (element, i) =>
        {
            if (i >= errors.Count)
                return;

            ValidationError error = errors[i];

            var name = element.Q <Button>("Name");
            name.text = error.errorName;
            name.tooltip = error.errorText;

            element.Q <VisualElement>("Error").style.display =
                error.severity == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;

            element.Q <VisualElement>("Warning").style.display =
                error.severity == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;

            element.Q <VisualElement>("Ok").style.display =
                error.severity == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;

            Action <GameObject> fixAction = error.fixAction;
            var hasFixAction = fixAction != null;

            var noFixAction = element.Q <VisualElement>("NoFixAction");
            noFixAction.style.display = !hasFixAction ? DisplayStyle.Flex : DisplayStyle.None;

            var fixButton = element.Q <Button>("BTN_Fix");
            fixButton.style.display = hasFixAction ? DisplayStyle.Flex : DisplayStyle.None;

            fixButton.clickable = new Clickable(
                () =>
                {
                    ValidationState status = s_status.State;

                    error.fixAction.Invoke(error.gameObject);
                    s_status.ValidateStatus();

                    if (status != s_status.State)
                        ValidatorView.UpdateBehaviourBasedOnState(s_status);

                    Refresh();
                });
        };
    }

    /// <summary> Unregister all callbacks </summary>
    private static void UnregisterCallbacks()
    {
        s_btnFixAll.clicked -= FixAll;
    }

    #endregion
}

}
#endif
