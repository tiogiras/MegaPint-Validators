namespace MegaPint.ValidationRequirement
{

/// <summary>
///     Class to store data of a <see cref="ScriptableValidationRequirement" /> and initialize it when it has not
///     been initialized
/// </summary>
public abstract class ValidationRequirementMetaData
{
    #region Protected Methods

    /// <summary>
    ///     Called once when the <see cref="ValidatableMonoBehaviour" /> is validated for the first time. Used to set
    ///     default values for the <see cref="ScriptableValidationRequirement" />
    /// </summary>
    protected abstract void OnInitialization();

    #endregion

    #region Internal Methods

    /// <summary>
    ///     Calls <see cref="OnInitialization" /> when the <see cref="ScriptableValidationRequirement" /> has not been
    ///     initialized
    /// </summary>
    /// <param name="requirement"> The requirement that is validated </param>
    internal void TryInitialize(ScriptableValidationRequirement requirement)
    {
        if (requirement.initialized)
            return;

        OnInitialization();

        requirement.initialized = true;
        requirement.GenerateUniqueID();
        requirement.SetDirty();
    }

    #endregion
}

}
