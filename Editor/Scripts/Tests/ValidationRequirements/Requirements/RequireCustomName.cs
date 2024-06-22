#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireCustomName
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireCustomNaming);

        yield return RequirementTests.TestRequirement(
            settings,
            null,
            ValidationState.Ok,
            false);
    }

    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireCustomNaming1);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.gameObject.name = "Custom Name";},
            ValidationState.Warning,
            false);
    }

    [UnityTest] [Order(2)]
    public IEnumerator Test2()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireCustomNaming1);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.gameObject.name = "[Test] Custom Name";},
            ValidationState.Ok,
            false);
    }

    [UnityTest] [Order(3)]
    public IEnumerator Test3()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireCustomNaming2);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.gameObject.name = "Custom_Name";},
            ValidationState.Warning,
            false);
    }

    [UnityTest] [Order(4)]
    public IEnumerator Test4()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireCustomNaming2);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.gameObject.name = "Custom Name";},
            ValidationState.Ok,
            false);
    }

    #endregion
}

}
#endif
#endif
