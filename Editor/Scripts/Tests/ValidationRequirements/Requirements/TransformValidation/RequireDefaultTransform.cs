#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements.TransformValidation
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireDefaultTransform
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireDefaultTransform);

        yield return RequirementTests.TestRequirement(
            settings,
            null,
            ValidationState.Ok,
            false);
    }

    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireDefaultTransform1);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.position = new Vector3(10, 23, 9);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(2)]
    public IEnumerator Test2()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireDefaultTransform2);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.rotation = Quaternion.Euler(new Vector3(2, 7, 100));},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(3)]
    public IEnumerator Test3()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireDefaultTransform3);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.localScale = new Vector3(10, 3, 8);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(4)]
    public IEnumerator Test4()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireDefaultTransform4);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                o.transform.position = new Vector3(4, 8, 7);
                o.transform.rotation = Quaternion.Euler(new Vector3(12, 19, 35));
                o.transform.localScale = new Vector3(34, 68, 97);
            },
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
