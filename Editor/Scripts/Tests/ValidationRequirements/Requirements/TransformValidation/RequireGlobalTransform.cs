#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements.TransformValidation
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireGlobalTransform
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGlobalTransform);

        yield return RequirementTests.TestRequirement(
            settings,
            null,
            ValidationState.Ok,
            true);
    }

    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGlobalTransform1);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.position = new Vector3(14, 42, 93);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(2)]
    public IEnumerator Test2()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGlobalTransform2);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.rotation = Quaternion.Euler(new Vector3(24, 89, 96));},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(3)]
    public IEnumerator Test3()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGlobalTransform3);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.localScale = new Vector3(69, 87, 99);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(4)]
    public IEnumerator Test4()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGlobalTransform4);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                o.transform.position = new Vector3(38, 27, 46);
                o.transform.rotation = Quaternion.Euler(new Vector3(5, 44, 79));
                o.transform.localScale = new Vector3(2, 98, 12);
            },
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
