namespace MegaPint.ValidationRequirement
{

/// <summary>
///     Class to store data of a <see cref="ScriptableValidationRequirement" /> and initialize it when it has not
///     been initialized
/// </summary>
public abstract class ValidationRequirementMetaData
{
    private bool _initialized;

    #region Protected Methods

    /// <summary>
    ///     Called once when the <see cref="ValidatableMonoBehaviour" /> is validated for the first time. Used to set
    ///     default values for the <see cref="ScriptableValidationRequirement" />
    /// </summary>
    protected abstract void OnInitialization();

    /// <summary>
    ///     Calls <see cref="OnInitialization" /> when the <see cref="ScriptableValidationRequirement" /> has not been
    ///     initialized
    /// </summary>
    protected void TryInitialize()
    {
        if (_initialized)
            return;

        OnInitialization();
        _initialized = true;
    }

    #endregion
}

}
