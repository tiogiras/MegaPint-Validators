using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("tiogiras.megapint.validators.editor.tests")]

namespace MegaPint.Tests
{

/// <summary> Test class used by unit tests to add a <see cref="ValidatableMonoBehaviour" /> to a gameObject </summary>
[AddComponentMenu("")]
internal class TestBehaviour : ValidatableMonoBehaviour
{
}

}
