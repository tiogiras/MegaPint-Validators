namespace ValidationRequirement
{

public abstract class ValidationRequirementMetaData
{
    private bool _initialized;

    protected void TryInitialize()
    {
        if (_initialized)
            return;

        Initialize();
        _initialized = true;
    }

    protected abstract void Initialize();
}

}
