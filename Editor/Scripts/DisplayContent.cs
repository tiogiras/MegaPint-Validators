#if UNITY_EDITOR
using MegaPint.Editor.Scripts.GUI.Utility;
using UnityEditor;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts
{

internal static partial class DisplayContent
{
    #region Private Methods

    // Called by reflection
    // ReSharper disable once UnusedMember.Local
    private static void Validators(DisplayContentReferences refs)
    {
        InitializeDisplayContent(
            refs,
            new TabSettings {info = true, guides = true, help = true},
            new TabActions
            {
                info = ValidatorsActivateLinks, guides = ValidatorsActivateLinks, help = ValidatorsActivateLinks
            });
    }

    /// <summary> Activate the links needed in the display content </summary>
    /// <param name="root"> Root visual element of the display content </param>
    private static void ValidatorsActivateLinks(VisualElement root)
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

    #endregion
}

}
#endif
