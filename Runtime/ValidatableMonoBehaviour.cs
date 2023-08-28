using System.Collections.Generic;
using System.ComponentModel;
using SerializeReferenceDropdown.Runtime;
using UnityEditor.EditorTools;
using UnityEngine;

public class ValidatableMonoBehaviour : MonoBehaviour
{
    [SerializeReferenceDropdown] [SerializeReference] private List<IValidationRequirement> _requirements;

    // TODO custom inspector and stuff
}