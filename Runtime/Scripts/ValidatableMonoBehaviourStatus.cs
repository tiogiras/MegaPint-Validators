﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MegaPint
{

/// <summary> Behaviour to display the status of a gameObject with <see cref="ValidatableMonoBehaviour" /> </summary>
[AddComponentMenu("")]
internal class ValidatableMonoBehaviourStatus : MonoBehaviour, IComparable <ValidatableMonoBehaviourStatus>
{
    public ValidationState State {get; private set;}

    internal readonly List <InvalidBehaviour> invalidBehaviours = new();
    public Action <ValidatableMonoBehaviourStatus> onStatusChanged;

    public Action <ValidationState> onStatusUpdate;

    private List <ValidatableMonoBehaviour> _behaviours = new();

    #region Unity Event Functions

    private void OnValidate()
    {
        if (ValidatesChildren())
            ListenToChildValidations();
        else
            StopListenToChildValidations();
    }

    #endregion

    #region Public Methods

    /// <summary> Compare against other status </summary>
    /// <param name="other"> Compare against </param>
    /// <returns> Compare value </returns>
    public int CompareTo(ValidatableMonoBehaviourStatus other)
    {
        if ((int)State > (int)other.State)
            return -1;

        return (int)State < (int)other.State ? 1 : string.CompareOrdinal(gameObject.name, other.gameObject.name);
    }

    /// <summary> Add a <see cref="ValidatableMonoBehaviour" /> to this status </summary>
    /// <param name="behaviour"> Behaviour to add </param>
    public void AddValidatableMonoBehaviour(ValidatableMonoBehaviour behaviour)
    {
        if (_behaviours.Contains(behaviour))
            return;

        _behaviours.Add(behaviour);
    }

    /// <summary> Fix all possible occured issues </summary>
    public void FixAll()
    {
        ValidationError[] errors = invalidBehaviours.SelectMany(invalidBehaviour => invalidBehaviour.errors).ToArray();

        for (var i = errors.Length - 1; i >= 0; i--)
        {
            ValidationError error = errors[i];
            error.fixAction?.Invoke(error.gameObject);
        }

        ValidateStatus();
    }

    /// <summary> If any <see cref="ValidatableMonoBehaviour" /> on this gameObject requires to validate children </summary>
    /// <returns> True when children are validated </returns>
    public bool ValidatesChildren()
    {
        return _behaviours.Any(behaviour => behaviour.ValidatesChildren());
    }

    /// <summary> Validate the current status </summary>

    // Resharper disable once CognitiveComplexity
    public void ValidateStatus()
    {
        ValidationState previousState = State;

        State = ValidationState.Ok;
        invalidBehaviours.Clear();

        if (gameObject.scene.name == null) // If prefab asset in project
            _behaviours = gameObject.GetComponents <ValidatableMonoBehaviour>().ToList();

        if (_behaviours.Count == 0)
        {
            onStatusUpdate?.Invoke(State);

            if (previousState != State)
                onStatusChanged?.Invoke(this);

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
                    new InvalidBehaviour
                    {
                        behaviourName = $"{GetFullBehaviourName(behaviour)} : {behaviour.GetType()}",
                        shortBehaviourName = behaviour.GetType().Name,
                        errors = errors
                    });
            }

            if (behaviourState > State)
                State = behaviourState;
        }

        onStatusUpdate?.Invoke(State);

        if (previousState != State)
            onStatusChanged?.Invoke(this);
    }

    #endregion

    #region Private Methods

    /// <summary> Get full name of a specific behaviour </summary>
    /// <param name="behaviour"> Targeted behaviour </param>
    /// <returns> Full name of the behaviour </returns>
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

    /// <summary> Listen to the validation events of any child <see cref="ValidatableMonoBehaviourStatus" /> </summary>
    private void ListenToChildValidations()
    {
        ValidatableMonoBehaviourStatus[] children =
            gameObject.GetComponentsInChildren <ValidatableMonoBehaviourStatus>(true);

        if (children.Length == 0)
            return;

        foreach (ValidatableMonoBehaviourStatus status in children)
            status.onStatusChanged += OnChildStatusChanged;
    }

    /// <summary> Callback when a child <see cref="ValidatableMonoBehaviourStatus" /> changed their status </summary>
    /// <param name="_"> Callback event </param>
    private void OnChildStatusChanged(ValidatableMonoBehaviourStatus _)
    {
        ValidateStatus();
    }

    /// <summary> Stop listening to the validation events of any child <see cref="ValidatableMonoBehaviourStatus" /> </summary>
    private void StopListenToChildValidations()
    {
        ValidatableMonoBehaviourStatus[] children =
            gameObject.GetComponentsInChildren <ValidatableMonoBehaviourStatus>(true);

        if (children.Length == 0)
            return;

        foreach (ValidatableMonoBehaviourStatus status in children)
            status.onStatusChanged -= OnChildStatusChanged;
    }

    #endregion
}

}
