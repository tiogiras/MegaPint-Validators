#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaPint.Editor.Scripts.GUI;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Logic
{

/// <summary> Logic for the api tab of the validators package </summary>
internal static class APILogic
{
    private static readonly List <APIData.Data> s_openAPIs = new();
    private static List <APIData.Data> s_topLevelAPI = new();
    private static List <APIData.Data> s_displayedAPIs = new();

    private static APIData.Data s_selectedAPI;

    private static VisualTreeAsset s_listItemTemplate;

    private static ListView s_listView;

    private static VisualElement s_rightPane;
    private static VisualElement s_content;

    private static Label s_title;
    private static Label s_description;
    private static Label s_assembly;

    #region Public Methods

    /// <summary> Logic for displaying the api tab of the validators package </summary>
    /// <param name="root"> RootVisualElement </param>
    public static void ValidatorsAPILogic(VisualElement root)
    {
        var leftPane = root.Q <VisualElement>("LeftPane");
        s_rightPane = root.Q <VisualElement>("RightPane");

        s_rightPane.style.display = DisplayStyle.None;

        s_listView = leftPane.Q <ListView>("Entries");
        s_title = s_rightPane.Q <Label>("Title");
        s_description = s_rightPane.Q <Label>("Description");
        s_assembly = s_rightPane.Q <Label>("Assembly");
        s_content = s_rightPane.Q <VisualElement>("Content");

        s_listItemTemplate ??= Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.APIItem);

        s_listView.makeItem = () => GUIUtility.Instantiate(s_listItemTemplate);

        s_listView.bindItem = BindItem;

        s_listView.selectedIndicesChanged += OnSelection;

        s_displayedAPIs = APIData.Get();

        s_topLevelAPI = s_displayedAPIs.Where(api => api.indentLevel == 0).ToList();

        s_listView.selectedIndex = -1;
        s_listView.itemsSource = s_displayedAPIs;
    }

    #endregion

    #region Private Methods

    /// <summary> Add all subAPIs of the target data to the displayed apis </summary>
    /// <param name="data"> Target data </param>
    private static void AddSubAPIs(APIData.Data data)
    {
        if (data.subAPIs is not {Count: > 0})
            return;

        var index = s_displayedAPIs.IndexOf(data) + 1;

        s_displayedAPIs.InsertRange(index, data.subAPIs);
    }

    /// <summary> Bind an list element </summary>
    /// <param name="element"> Target element </param>
    /// <param name="index"> Index of the target element in the listView </param>
    private static void BindItem(VisualElement element, int index)
    {
        var api = (APIData.Data)s_listView.itemsSource[index];

        var hierarchy = element.Q <VisualElement>("Hierarchy");
        hierarchy.Clear();

        if (!s_topLevelAPI.Contains(api))
        {
            APIData.Data nextAPI = index == s_listView.itemsSource.Count - 1
                ? null
                : (APIData.Data)s_listView.itemsSource[index + 1];

            var nextApiIsSameClass = api.indentLevel == nextAPI?.indentLevel;

            for (var j = 0; j < api.indentLevel; j++)
            {
                if (j == api.indentLevel - 1)
                    hierarchy.Add(nextApiIsSameClass ? DrawFull() : DrawCorner());
                else
                    hierarchy.Add(DrawLine());
            }
        }

        var label = element.Q <Label>("Name");
        label.text = api.displayName;

        var open = element.Q <VisualElement>("Open");
        var closed = element.Q <VisualElement>("Closed");

        var hasSubAPIs = api.subAPIs?.Count > 0;
        var opened = s_openAPIs.Contains(api);

        open.style.display = opened && hasSubAPIs ? DisplayStyle.Flex : DisplayStyle.None;
        closed.style.display = !opened && hasSubAPIs ? DisplayStyle.Flex : DisplayStyle.None;

        var isSelectedAPI = s_selectedAPI == api;

        label.style.borderRightWidth = isSelectedAPI ? 3 : 0;
    }

    /// <summary> Display the right pane </summary>
    /// <param name="data"> Target data </param>
    private static void DisplayRightPane(APIData.Data data)
    {
        if (data == null)
            return;

        s_rightPane.style.display = DisplayStyle.Flex;
        s_rightPane.Q <ScrollView>().scrollOffset = Vector2.zero;

        s_title.text = data.title;
        s_description.text = data.description;
        s_assembly.text = data.assembly;

        s_title.ActivateLinks(LinkCallback);
        s_description.ActivateLinks(LinkCallback);

        s_content.Clear();

        var path = Path.Combine(Constants.Validators.UserInterface.APIItems, data.key.ToString());
        var loadedContent = Resources.Load <VisualTreeAsset>(path);

        if (loadedContent == null)
        {
            var label = new Label($"No content found for the key {data.key}");
            s_content.Add(label);

            return;
        }

        VisualElement instantiatedContent = GUIUtility.Instantiate(loadedContent);
        instantiatedContent.ActivateLinks(LinkCallback);

        s_content.Add(instantiatedContent);
    }

    /// <summary> Draw a hierarchy corner </summary>
    /// <returns> Corner as visualElement </returns>
    private static VisualElement DrawCorner()
    {
        VisualElement ve = DrawEmpty();

        var left = new VisualElement {style = {flexGrow = 1}};
        ve.Add(left);

        var right = new VisualElement {style = {flexGrow = 1, flexDirection = FlexDirection.Column}};
        ve.Add(right);

        var rightUp = new VisualElement {style = {flexGrow = 1, borderBottomWidth = 2, borderLeftWidth = 2}};
        rightUp.AddToClassList(StyleSheetClasses.Border.Color.Separator);
        right.Add(rightUp);

        var rightDown = new VisualElement {style = {flexGrow = 1}};
        right.Add(rightDown);

        return ve;
    }

    /// <summary> Draw a hierarchy empty </summary>
    /// <returns> Empty as visualElement </returns>
    private static VisualElement DrawEmpty()
    {
        return new VisualElement {style = {width = 20, flexGrow = 0, flexDirection = FlexDirection.Row}};
    }

    /// <summary> Draw a hierarchy full </summary>
    /// <returns> Full as visualElement </returns>
    private static VisualElement DrawFull()
    {
        VisualElement ve = DrawEmpty();

        var left = new VisualElement {style = {flexGrow = 1, borderRightWidth = 2}};
        left.AddToClassList(StyleSheetClasses.Border.Color.Separator);
        ve.Add(left);

        var right = new VisualElement {style = {flexGrow = 1, flexDirection = FlexDirection.Column}};
        ve.Add(right);

        var rightUp = new VisualElement {style = {flexGrow = 1, borderBottomWidth = 2}};
        rightUp.AddToClassList(StyleSheetClasses.Border.Color.Separator);
        right.Add(rightUp);

        var rightDown = new VisualElement {style = {flexGrow = 1}};
        right.Add(rightDown);

        return ve;
    }

    /// <summary> Draw a hierarchy line </summary>
    /// <returns> Line as visualElement </returns>
    private static VisualElement DrawLine()
    {
        VisualElement ve = DrawEmpty();

        var left = new VisualElement {style = {flexGrow = 1, borderRightWidth = 2}};
        left.AddToClassList(StyleSheetClasses.Border.Color.Separator);
        ve.Add(left);

        var right = new VisualElement {style = {flexGrow = 1, flexDirection = FlexDirection.Column}};
        ve.Add(right);

        return ve;
    }

    /// <summary> Callback when clicking a link </summary>
    /// <param name="evt"> Callback event </param>
    private static void LinkCallback(PointerUpLinkTagEvent evt)
    {
        var link = evt.linkID;

        if (string.IsNullOrEmpty(link))
            return;

        if (!Enum.TryParse(link, out APIData.DataKey key))
        {
            Debug.LogWarning($"No key found for link {link}");

            return;
        }

        s_listView.SetSelectionWithoutNotify(null);
        s_selectedAPI = APIData.Get(key);

        s_listView.RefreshItems();

        DisplayRightPane(s_selectedAPI);
    }

    /// <summary> List View selection event </summary>
    /// <param name="_"> Callback event </param>
    private static void OnSelection(IEnumerable <int> _)
    {
        var index = s_listView.selectedIndex;

        if (index < 0)
            return;

        var api = (APIData.Data)s_listView.itemsSource[index];

        if (s_selectedAPI == api)
        {
            if (s_openAPIs.Contains(api))
                RemoveSubAPIs(api);
            else
            {
                s_openAPIs.Add(api);
                AddSubAPIs(api);
            }
        }

        s_selectedAPI = api;
        DisplayRightPane(api);
        s_listView.RefreshItems();

        s_listView.selectedIndex = -1;
    }

    /// <summary> Remove all subAPIs of the target data from the displayed apis </summary>
    /// <param name="data"> Target data </param>
    private static void RemoveSubAPIs(APIData.Data data)
    {
        if (data.subAPIs is not {Count: > 0})
            return;

        if (!s_openAPIs.Contains(data))
            return;

        s_openAPIs.Remove(data);

        foreach (APIData.Data subAPI in data.subAPIs)
        {
            s_displayedAPIs.Remove(subAPI);
            RemoveSubAPIs(subAPI);
        }
    }

    #endregion
}

}
#endif
