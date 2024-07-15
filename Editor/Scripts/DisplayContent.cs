#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.GUI;
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts
{

internal static partial class DisplayContent
{
    private static class ValidatorsLogic
    {
        private static class ValidatorsAPIData
        {
            public class APIData
            {
                public string displayName;
                public int indentLevel;
                public List <APIData> subAPIs;
                public string title;
            }

            private static readonly List <APIData> s_data = new()
            {
                new APIData
                {
                    title = "ValidationRequirementNameAttribute",
                    displayName = "ValidationRequirementNameAttribute",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new()
                        {
                            title = "bool : ValidationRequirementNameAttribute.allowMultiple",
                            displayName = "allowMultiple",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "List<Type> : ValidationRequirementNameAttribute.incompatibleRequirements",
                            displayName = "incompatibleRequirements",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "int[] : ValidationRequirementNameAttribute.menuOrder",
                            displayName = "menuOrder",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "string : ValidationRequirementNameAttribute.name",
                            displayName = "name",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Type : ValidationRequirementNameAttribute.requirementType",
                            displayName = "requirementType",
                            indentLevel = 1
                        }
                    }
                },
                new APIData
                {
                    title = "ScriptableValidationRequirement",
                    displayName = "ScriptableValidationRequirement",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new()
                        {
                            title = "void : ScriptableValidationRequirement.AddError()",
                            displayName = "AddError()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ScriptableValidationRequirement.AddErrorIf()",
                            displayName = "AddErrorIf()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ScriptableValidationRequirement.AddErrors()",
                            displayName = "AddErrors()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ScriptableValidationRequirement.OnRequirementValidation()",
                            displayName = "OnRequirementValidation()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ScriptableValidationRequirement.Validate()",
                            displayName = "Validate()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "bool : ScriptableValidationRequirement.initialized",
                            displayName = "initialized",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Object : ScriptableValidationRequirement.objectReference",
                            displayName = "objectReference",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Object : ScriptableValidationRequirement.preventListHeaderIssue",
                            displayName = "preventListHeaderIssue",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Object : ScriptableValidationRequirement.severityOverwrite",
                            displayName = "severityOverwrite",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Object : ScriptableValidationRequirement.targetGameObject",
                            displayName = "targetGameObject",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "Object : ScriptableValidationRequirement.uniqueID",
                            displayName = "uniqueID",
                            indentLevel = 1
                        }
                    }
                },
                new APIData
                {
                    title = "ValidatableMonoBehaviour",
                    displayName = "ValidatableMonoBehaviour",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new()
                        {
                            title = "struct : ValidatableMonoBehaviour.SettingPriority",
                            displayName = "SettingPriority",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ValidatableMonoBehaviour.BeforeValidation()",
                            displayName = "BeforeValidation()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "string[] : ValidatableMonoBehaviour.DefaultImports()",
                            displayName = "DefaultImports()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "List<ValidatorSettings> : ValidatableMonoBehaviour.GetImportedSettings()",
                            displayName = "GetImportedSettings()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : ValidatableMonoBehaviour.ImportSetting(ValidatorSettings)",
                            displayName = "ImportSetting()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "void : ValidatableMonoBehaviour.RemoveImportedSetting(ValidatorSettings)",
                            displayName = "RemoveImportedSetting()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "List <ScriptableValidationRequirement> : ValidatableMonoBehaviour.Requirements(bool = false)",
                            displayName = "Requirements()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "void : ValidatableMonoBehaviour.SetRequirements(List <ScriptableValidationRequirement>)",
                            displayName = "SetRequirements()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "ValidationState : ValidatableMonoBehaviour.Validate(out List <ValidationError>)",
                            displayName = "Validate()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "bool : ValidatableMonoBehaviour.HasImportedSettings",
                            displayName = "HasImportedSettings",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "List <ValidatorSettings> : ValidatableMonoBehaviour.defaultSettings",
                            displayName = "defaultSettings",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "bool : ValidatableMonoBehaviour.importedSettingsFoldout",
                            displayName = "importedSettingsFoldout",
                            indentLevel = 1
                        },
                        new()
                        {
                            title =
                                "List <SettingPriority> : ValidatableMonoBehaviour.settingPriorities",
                            displayName = "settingPriorities",
                            indentLevel = 1
                        }
                    }
                },
                new APIData
                {
                    title = "ValidationError",
                    displayName = "ValidationError",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new() {title = "ValidationError.errorName", displayName = "errorName", indentLevel = 1},
                        new() {title = "ValidationError.errorText", displayName = "errorText", indentLevel = 1},
                        new() {title = "ValidationError.fixAction", displayName = "fixAction", indentLevel = 1},
                        new()
                        {
                            title = "ValidationError.gameObject",
                            displayName = "gameObject",
                            indentLevel = 1
                        },
                        new() {title = "ValidationError.severity", displayName = "severity", indentLevel = 1}
                    }
                },
                new APIData
                {
                    title = "ValidationRequirementMetaData",
                    displayName = "ValidationRequirementMetaData",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new()
                        {
                            title = "void : ValidationRequirementMetaData.OnInitialization()",
                            displayName = "OnInitialization()",
                            indentLevel = 1
                        }
                    }
                },
                new APIData
                {
                    title = "ValidatorSettings",
                    displayName = "ValidatorSettings",
                    indentLevel = 0,
                    subAPIs = new List <APIData>
                    {
                        new()
                        {
                            title =
                                "List <ScriptableValidationRequirement> : Requirements(bool = false)",
                            displayName = "Requirements()",
                            indentLevel = 1
                        },
                        new()
                        {
                            title = "void : SetRequirements(List <ScriptableValidationRequirement>)",
                            displayName = "SetRequirements()",
                            indentLevel = 1
                        }
                    }
                },
                new APIData
                {
                    title = "SerializeReferenceDropdownAttribute",
                    displayName = "SerializeReferenceDropdownAttribute",
                    indentLevel = 0
                },
                new APIData
                {
                    title = "ValidationRequirementTooltipAttribute",
                    displayName = "ValidationRequirementTooltipAttribute",
                    indentLevel = 0
                },
                new APIData {title = "ToggleableSetting", displayName = "ToggleableSetting", indentLevel = 0},
                new APIData {title = "ValidationState", displayName = "ValidationState", indentLevel = 0}
            };

            #region Public Methods

            public static List <APIData> Get()
            {
                return s_data;
            }

            #endregion
        }

        private static readonly List <ValidatorsAPIData.APIData> s_openAPIs = new();
        private static List <ValidatorsAPIData.APIData> s_topLevelAPI = new();
        private static List <ValidatorsAPIData.APIData> s_displayedAPIs = new();

        private static ValidatorsAPIData.APIData s_selectedAPI;

        #region Public Methods

        /// <summary> Activate the links needed in the display content </summary>
        /// <param name="root"> Root visual element of the display content </param>
        public static void ValidatorsActivateLinks(VisualElement root)
        {
            root.ActivateLinks(
                evt =>
                {
                    switch (evt.linkID)
                    {
                        case "validatorView":
                            EditorApplication.ExecuteMenuItem(
                                Constants.Validators.Links.ValidatorView);

                            break;

                        case "validatorSettings":
                            EditorApplication.ExecuteMenuItem(
                                Constants.Validators.Links.CreateValidatorSettings);

                            break;

                        case "componentOrderConfig":
                            EditorApplication.ExecuteMenuItem(
                                Constants.Validators.Links.CreateComponentOrderConfig);

                            break;
                    }
                });
        }

        /// <summary> Logic for displaying the api tab of the validators package </summary>
        /// <param name="root"> RootVisualElement </param>
        public static void ValidatorsAPILogic(VisualElement root)
        {
            var leftPane = root.Q <VisualElement>("LeftPane");
            var rightPane = root.Q <VisualElement>("RightPane");

            //leftPane.parent.parent.style.flexGrow = 1f;
            rightPane.style.display = DisplayStyle.None;

            var entriesListView = leftPane.Q <ListView>("Entries");
            var title = rightPane.Q <Label>("Title");
            var content = rightPane.Q <VisualElement>("Content");

            var listItemTemplate =
                Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.APIItem);

            entriesListView.makeItem = () => GUIUtility.Instantiate(listItemTemplate);

            entriesListView.bindItem = (element, i) =>
            {
                var api = (ValidatorsAPIData.APIData)entriesListView.itemsSource[i];

                var hierarchy = element.Q <VisualElement>("Hierarchy");
                hierarchy.Clear();

                if (!s_topLevelAPI.Contains(api))
                {
                    ValidatorsAPIData.APIData nextAPI = i == entriesListView.itemsSource.Count - 1
                        ? null
                        : (ValidatorsAPIData.APIData)entriesListView.itemsSource[i + 1];

                    var nextApiIsSameClass = api.indentLevel == nextAPI?.indentLevel;

                    for (var j = 0; j < api.indentLevel; j++)
                    {
                        if (j == api.indentLevel - 1)
                        {
                            if (nextApiIsSameClass)
                                hierarchy.Add(DrawFull());
                            else
                                hierarchy.Add(DrawCorner());
                        }
                        else
                        {
                            //if (hasNonSubApiWithSameIndent)
                            hierarchy.Add(DrawLine());

                            //else 
                            //hierarchy.Add(DrawEmpty());
                        }
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
            };

            entriesListView.selectedIndicesChanged += _ =>
            {
                var index = entriesListView.selectedIndex;

                if (index < 0)
                    return;

                var api = (ValidatorsAPIData.APIData)entriesListView.itemsSource[index];

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
                DisplayRightPane(api, rightPane, title, content);
                entriesListView.RefreshItems();

                entriesListView.selectedIndex = -1;
            };

            s_displayedAPIs = ValidatorsAPIData.Get();

            s_topLevelAPI = s_displayedAPIs.Where(api => api.indentLevel == 0).ToList();

            entriesListView.selectedIndex = -1;
            entriesListView.itemsSource = s_displayedAPIs;
        }

        #endregion

        private static void DisplayRightPane(ValidatorsAPIData.APIData data, VisualElement rightPane, Label title, VisualElement content)
        {
            rightPane.style.display = DisplayStyle.Flex;

            title.text = data.title;
        }

        #region Private Methods

        private static void AddSubAPIs(ValidatorsAPIData.APIData data)
        {
            if (data.subAPIs is not {Count: > 0})
                return;

            var index = s_displayedAPIs.IndexOf(data) + 1;

            s_displayedAPIs.InsertRange(index, data.subAPIs);
        }

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

        private static VisualElement DrawEmpty()
        {
            return new VisualElement {style = {width = 20, flexGrow = 0, flexDirection = FlexDirection.Row}};
        }

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

        private static void RemoveSubAPIs(ValidatorsAPIData.APIData data)
        {
            if (data.subAPIs is not {Count: > 0})
                return;

            if (!s_openAPIs.Contains(data))
                return;

            s_openAPIs.Remove(data);

            foreach (ValidatorsAPIData.APIData subAPI in data.subAPIs)
            {
                s_displayedAPIs.Remove(subAPI);
                RemoveSubAPIs(subAPI);
            }
        }

        #endregion
    }

    #region Private Methods

    // Called by reflection
    // ReSharper disable once UnusedMember.Local
    private static void Validators(DisplayContentReferences refs)
    {
        InitializeDisplayContent(
            refs,
            new TabSettings {info = true, guides = true, help = true, api = true},
            new TabActions
            {
                info = ValidatorsLogic.ValidatorsActivateLinks,
                guides = ValidatorsLogic.ValidatorsActivateLinks,
                help = ValidatorsLogic.ValidatorsActivateLinks,
                api = ValidatorsLogic.ValidatorsAPILogic
            });
    }

    #endregion
}

}
#endif
