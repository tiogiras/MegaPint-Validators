using System;
using System.Collections.Generic;
using UnityEngine;

public class ValidatableMonoBehaviourStatus : MonoBehaviour, IComparable <ValidatableMonoBehaviourStatus>
{
    private List <ValidatableMonoBehaviour> _behaviours = new();

    public ValidationState State {get; private set;}

    public Action <ValidationState> onStatusUpdate;

    public readonly List <InvalidBehaviour> invalidBehaviours = new();

    public void AddValidatableMonoBehaviour(ValidatableMonoBehaviour behaviour)
    {
        if (_behaviours.Contains(behaviour))
            return;

        _behaviours.Add(behaviour);
    }

    public void ValidateStatus()
    {
        State = ValidationState.Ok;
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
                    invalidBehaviours.Add(
                        new InvalidBehaviour
                        {
                            behaviourName = $"{GetFullBehaviourName(behaviour)} : {behaviour.GetType()}",
                            errors = errors
                        });
                }

                if (behaviourState > State)
                    State = behaviourState;
            }
        }

        onStatusUpdate?.Invoke(State);
    }

    private static string GetFullBehaviourName(Component behaviour)
    {
        if (behaviour.transform.parent == null)
            return behaviour.name;

        List <string> nameParts = new() {behaviour.name};

        Transform parent = behaviour.transform.parent;

        while (parent != null)
        {
            nameParts.Add(parent.name);
            parent = parent.transform.parent;
        }

        nameParts.Reverse();

        return string.Join(".", nameParts);
    }

    public int CompareTo(ValidatableMonoBehaviourStatus other)
    {
        if ((int)State > (int)other.State)
            return -1;

        if ((int)State < (int)other.State)
            return 1;

        return string.CompareOrdinal(gameObject.name, other.gameObject.name);
    }
}
