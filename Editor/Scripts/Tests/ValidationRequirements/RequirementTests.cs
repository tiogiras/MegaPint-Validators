#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System;
using System.Collections;
using System.Collections.Generic;
using MegaPint.Tests;
using MegaPint.ValidationRequirement;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegaPint.Editor.Scripts.Tests.ValidationRequirements
{

/// <summary> Utility class storing testing functions for <see cref="ScriptableValidationRequirement" /> </summary>
internal static class RequirementTests
{
    #region Public Methods

    public static IEnumerator TestRequirement(
        ValidatorSettings settings,
        Action <GameObject> setup,
        ValidationState expectedStateAfterValidation,
        bool canBeFixed)
    {
        if (settings == null)
            Assert.Fail("Settings are null!");

        // Create test object
        var testObject = new GameObject();
        var validatableMonoBehaviour = testObject.AddComponent <TestBehaviour>();

        // First validation of the object should return Ok
        ValidationState state = validatableMonoBehaviour.Validate(out List <ValidationError> _);

        if (state != ValidationState.Ok)
            Assert.Fail("Failed before adding requirement!");

        // Import the required settings for this requirement
        validatableMonoBehaviour.SetImportedSettings(settings);

        // Invoke the setup... used to set the specific values of the object so the next validation returns false
        setup?.Invoke(testObject);

        // Second validation of the object should return the expected state
        state = validatableMonoBehaviour.Validate(out List <ValidationError> errors);

        Assert.AreEqual(expectedStateAfterValidation, state, "Validation did not return the expected state!");

        if (!canBeFixed)
            Assert.Pass();

        yield return null;

        // Fix all occured issues
        foreach (ValidationError error in errors)
            error.fixAction?.Invoke(error.gameObject);

        // Last validation of the object should return Ok
        state = validatableMonoBehaviour.Validate(out List <ValidationError> _);

        Object.DestroyImmediate(testObject);

        Assert.AreEqual(
            ValidationState.Ok,
            state,
            "AutoFix action did not fix the issue but the requirement expected it to!");

        Assert.Pass("Successfully tested requirement!");
    }

    #endregion
}

}
#endif
#endif
