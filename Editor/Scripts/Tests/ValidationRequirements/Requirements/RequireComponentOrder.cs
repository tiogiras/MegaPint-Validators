#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireComponentOrder
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireComponentOrder);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                // TODO
            },
            ValidationState.Warning,
            true);
    }
    
    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireComponentOrder1);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                // TODO
            },
            ValidationState.Warning,
            true);
    }
    
    [UnityTest] [Order(2)]
    public IEnumerator Test2()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireComponentOrder2);

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
