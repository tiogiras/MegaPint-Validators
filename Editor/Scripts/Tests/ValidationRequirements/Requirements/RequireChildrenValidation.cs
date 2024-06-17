#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireChildrenValidation
{
    #region Tests

    [UnityTest]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireChildrenValidation);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                // TODO
            },
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
