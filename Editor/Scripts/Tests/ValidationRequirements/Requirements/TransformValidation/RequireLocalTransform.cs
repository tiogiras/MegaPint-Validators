#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements.TransformValidation
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireLocalTransform
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireLocalTransform);

        yield return RequirementTests.TestRequirement(
            settings,
            null,
            ValidationState.Ok,
            true);
    }

    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireLocalTransform1);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.localPosition = new Vector3(94, 68, 1);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(2)]
    public IEnumerator Test2()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireLocalTransform2);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.localRotation = Quaternion.Euler(new Vector3(40, 7, 93));},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(3)]
    public IEnumerator Test3()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireLocalTransform3);

        yield return RequirementTests.TestRequirement(
            settings,
            o => {o.transform.localScale = new Vector3(8, 64, 82);},
            ValidationState.Warning,
            true);
    }

    [UnityTest] [Order(4)]
    public IEnumerator Test4()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireLocalTransform4);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                o.transform.localPosition = new Vector3(4, 28, 7);
                o.transform.localRotation = Quaternion.Euler(new Vector3(12, 3, 40));
                o.transform.localScale = new Vector3(0, 25, 77);
            },
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
