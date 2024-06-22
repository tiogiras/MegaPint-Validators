#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using MegaPint.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements.Requirements
{

/// <summary> Unit tests for the corresponding requirement </summary>
internal class RequireChildrenValidation
{
    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator Test()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireChildrenValidation);

        yield return RequirementTests.TestRequirement(
            settings,
            null,
            ValidationState.Ok,
            true);
    }

    [UnityTest] [Order(1)]
    public IEnumerator Test1()
    {
        var settings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireChildrenValidation);
        var childSettings = Resources.Load <ValidatorSettings>(Constants.Validators.Tests.RequireGameObjectActive);

        yield return RequirementTests.TestRequirement(
            settings,
            o =>
            {
                var child = new GameObject();
                child.SetActive(false);

                var validatableMonoBehaviour = child.AddComponent <TestBehaviour>();

                validatableMonoBehaviour.SetImportedSettings(childSettings);
                validatableMonoBehaviour.Validate(out List <ValidationError> _);

                child.transform.SetParent(o.transform);
            },
            ValidationState.Warning,
            true);
    }

    #endregion
}

}
#endif
#endif
