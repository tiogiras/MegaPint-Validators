#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements.GameObjectValidation
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireGameObjectActive
{
    #region Tests

    [UnityTest]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGameObjectActive);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.SetActive(false);},
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
