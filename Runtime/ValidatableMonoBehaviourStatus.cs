using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValidatableMonoBehaviourStatus : MonoBehaviour, IComparable <ValidatableMonoBehaviourStatus>
{
    public ValidationState State {get; private set;}

    public readonly List <InvalidBehaviour> invalidBehaviours = new();

    public Action <ValidationState> onStatusUpdate;
    private List <ValidatableMonoBehaviour> _behaviours = new();

    #region Public Methods

    public void FixAll()
    {
        ValidationError[] errors = invalidBehaviours.SelectMany(invalidBehaviour => invalidBehaviour.errors).ToArray();

        for (var i = errors.Length - 1; i >= 0; i--)
        {
            ValidationError error = errors[i];
            
            if (error.fixAction == null)
            {
                if (!error.errorName.Equals("Invalid monoBehaviours in children"))
                    Debug.LogWarning($"No FixAction specified for [{error.errorName}], requires manual attention!");
            }
            else
                error.fixAction.Invoke(error.gameObject);
        }

        ValidateStatus();
    }

    public int CompareTo(ValidatableMonoBehaviourStatus other)
    {
        if ((int)State > (int)other.State)
            return -1;

        if ((int)State < (int)other.State)
            return 1;

        return string.CompareOrdinal(gameObject.name, other.gameObject.name);
    }

    public void AddValidatableMonoBehaviour(ValidatableMonoBehaviour behaviour)
    {
        if (_behaviours.Contains(behaviour))
            return;

        _behaviours.Add(behaviour);
    }

    public bool ValidatesChildren()
    {
        return _behaviours.Any(behaviour => behaviour.ValidatesChildren());
    }

    public void ValidateStatus()
    {
        State = ValidationState.Ok;
        invalidBehaviours.Clear();

        if (gameObject.scene.name == null) // If prefab asset in project
            _behaviours = gameObject.GetComponents <ValidatableMonoBehaviour>().ToList();

        if (_behaviours.Count == 0)
        {
            onStatusUpdate?.Invoke(State);

            return;
        }

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
                    new InvalidBehaviour {behaviourName = $"{GetFullBehaviourName(behaviour)} : {behaviour.GetType()}", errors = errors});
            }

            if (behaviourState > State)
                State = behaviourState;
        }

        onStatusUpdate?.Invoke(State);
    }

    #endregion

    #region Private Methods

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

    #endregion
    
    
}
