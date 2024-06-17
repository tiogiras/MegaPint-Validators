﻿#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Threading.Tasks;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using MegaPint.Editor.Scripts.Tests.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Tests
{

/// <summary> Unit tests regarding the general structure and settings of the package </summary>
internal class PackageTests
{
    private static bool s_initialized;

    #region Tests

    [UnityTest] [Order(0)]
    public IEnumerator InitializePackageCache()
    {
        Task <bool> task = TestsUtility.CheckCacheInitialization();

        yield return task.AsIEnumeratorReturnNull();

        s_initialized = task.Result;
        Assert.IsTrue(task.Result);
    }

    [Test] [Order(1)]
    public void PackageStructure()
    {
        if (!s_initialized)
            Assert.Fail("FAILED ===> Missing packageCache initialization!");

        TestsUtility.CheckStructure(PackageKey.Validators);
    }

    [Test] [Order(1)]
    public void Resources()
    {
        var isValid = true;

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.Validators.UserInterface.ComponentOrderConfig);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.Validators.UserInterface.ComponentOrderTypeEntry);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.Validators.UserInterface.Status);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.Validators.UserInterface.StatusBehaviour);

        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.Validators.UserInterface.StatusError);
        TestsUtility.ValidateResource <VisualTreeAsset>(ref isValid, Constants.Validators.UserInterface.ValidatorView);

        TestsUtility.ValidateResource <VisualTreeAsset>(
            ref isValid,
            Constants.Validators.UserInterface.ValidatorViewItem);

        TestsUtility.ValidateResource <Texture2D>(ref isValid, Constants.Validators.Images.ManualIssue);

        TestsUtility.ValidateResource <ValidatorSettings>(
            ref isValid,
            Constants.Validators.Tests.RequireGameObjectActive);

        Assert.IsTrue(isValid);
    }

    #endregion
}

}
#endif
#endif
