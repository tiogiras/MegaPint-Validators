using System;
using System.Collections.Generic;
using UnityEngine;

public class ValidatableMonoBehaviourStatus : MonoBehaviour
{
    private readonly List <ValidatableMonoBehaviour> _behaviours = new ();
    private ValidationState _state;

    public Action <ValidationState> onStatusUpdate;

    public void AddValidatableMonoBehaviour(ValidatableMonoBehaviour behaviour)
    {
        if (_behaviours.Contains(behaviour))
            return;
        
        _behaviours.Add(behaviour);
    }

    public void ValidateStatus()
    {
        Debug.Log("validating");
        
        ValidationState originalState = _state;
        
        _state = ValidationState.Ok;

        if (_behaviours.Count > 0)
        {
            for (var i = _behaviours.Count - 1; i >= 0; i--)
            {
                ValidatableMonoBehaviour behaviour = _behaviours[i];
            
                if (behaviour == null)
                {
                    _behaviours.RemoveAt(i);
                    continue;
                }

                ValidationState behaviourState = behaviour.Validate();

                if (behaviourState > _state)
                    _state = behaviourState;
            }
        }

        if (originalState != _state)
            onStatusUpdate?.Invoke(_state);
    }
}