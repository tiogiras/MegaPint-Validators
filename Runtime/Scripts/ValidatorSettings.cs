using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEngine;

namespace MegaPint
{

/// <summary> Settings of a <see cref="ValidatableMonoBehaviour" /> </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/Validator Settings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    internal static Action <string, bool> onRequirementsChanged;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <ScriptableValidationRequirement> _requirements;

    private int _lastRequirementCount;
    private bool _lastRequirementCountInitialized;

    #region Unity Event Functions

    private void OnValidate()
    {
        if (!_lastRequirementCountInitialized)
        {
            _lastRequirementCount = _requirements.Count;
            _lastRequirementCountInitialized = true;
        }
        else
        {
            if (_lastRequirementCount != _requirements.Count)
            {
                onRequirementsChanged?.Invoke(name, _lastRequirementCount < _requirements.Count);
                _lastRequirementCount = _requirements.Count;
            }
        }

        foreach (ScriptableValidationRequirement requirement in _requirements)
            requirement?.OnValidate(this);
    }

    #endregion

    #region Public Methods

    public List <ScriptableValidationRequirement> Requirements(bool excludeNulls = false)
    {
        return excludeNulls ? _requirements.Where(requirement => requirement != null).ToList() : _requirements;
    }

    /// <summary> Overwrite the saved requirements </summary>
    /// <param name="requirements"> New requirements </param>
    public void SetRequirements(List <ScriptableValidationRequirement> requirements)
    {
        _requirements = requirements;
    }

    #endregion
}

}
