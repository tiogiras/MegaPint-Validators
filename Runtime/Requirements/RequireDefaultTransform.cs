using System;
using UnityEngine;

namespace Requirements
{
    [Serializable]
    public class RequireDefaultTransform : IValidationRequirement
    {
        public bool defaultPosition = true;
        public bool defaultRotation = true;
        public bool defaultScale;

        public bool Validate(GameObject gameObject)
        {
            var transform = gameObject.transform;

            return (transform.position == Vector3.zero || !defaultPosition) &&
                   (transform.rotation == Quaternion.identity || !defaultRotation) &&
                   (transform.localScale == Vector3.one || !defaultScale);
        }

        public void Fix(GameObject gameObject)
        {
            var transform = gameObject.transform;

            if (defaultPosition)
                transform.position = Vector3.zero;

            if (defaultRotation)
                transform.rotation = Quaternion.identity;
            
            if (defaultScale)
                transform.localScale = Vector3.one;
        }
    }
}