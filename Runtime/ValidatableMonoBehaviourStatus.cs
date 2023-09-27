using System;
using System.Collections.Generic;
using UnityEngine;

public class ValidatableMonoBehaviourStatus : MonoBehaviour
{
    private List <ValidatableMonoBehaviour> _behaviours = new ();
    private ValidationState _state;

    public Action <ValidationState> onStatusUpdate;

    public readonly List <InvalidBehaviour> invalidBehaviours = new ();

    public void AddValidatableMonoBehaviour(ValidatableMonoBehaviour behaviour)
    {
        if (_behaviours.Contains(behaviour))
            return;
        
        _behaviours.Add(behaviour);
    }

    public void ValidateStatus()
    {
        _state = ValidationState.Ok;
        invalidBehaviours.Clear();

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

                ValidationState behaviourState = behaviour.Validate(out List <ValidationError> errors);

                if (behaviourState != ValidationState.Ok)
                {
                    invalidBehaviours.Add(new InvalidBehaviour
                    {
                        behaviourName = $"{behaviour.name}.{behaviour.GetType()}",
                        errors = errors
                    });
                }

                if (behaviourState > _state)
                    _state = behaviourState;
            }
        }

        onStatusUpdate?.Invoke(_state);
    }
}