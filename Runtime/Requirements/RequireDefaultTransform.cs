using System;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace Requirements
{
    [Serializable]
    [SerializeReferenceDropdownName("Require Default Transform")]
    public class RequireDefaultTransform : ValidationRequirementMetaData, IValidationRequirement
    {
        public bool defaultPosition;
        public bool defaultRotation;
        public bool defaultScale;

        public ValidationState Validate(GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            
            return (transform.position == Vector3.zero || !defaultPosition) &&
                   (transform.rotation == Quaternion.identity || !defaultRotation) &&
                   (transform.localScale == Vector3.one || !defaultScale)
                ? ValidationState.Ok
                : ValidationState.Warning;
        }

        public void Fix(GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            if (defaultPosition)
                transform.position = Vector3.zero;

            if (defaultRotation)
                transform.rotation = Quaternion.identity;
            
            if (defaultScale)
                transform.localScale = Vector3.one;
        }

        public void OnValidate()
        {
            TryInitialize();
        }

        protected override void Initialize()
        {
            defaultPosition = true;
            defaultRotation = true;
            defaultScale = false;
        }
    }
}