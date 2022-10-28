using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exploratorium
{
    [ExecuteAlways]
    public class CameraFramer : MonoBehaviour
    {
        [Tooltip("Will frame all children for this transform based on any renderers found")] [SerializeField, Required]
        private Transform targetObjectToFrame;

        [Tooltip("The camera to frame")] [SerializeField, Required]
        private Camera targetCamera;

        [Tooltip("Angular offset to apply to the camera look direction in its local space after framing")]
        [SerializeField]
        private Vector3 angleOffset;

        [SerializeField] private float scale = 1.0f;


        [ShowInInspector, ReadOnly]
        private Bounds _bounds;

        [SerializeField] private bool debug;

        [Button]
        public void RecalculateFraming()
        {
            RecalculateFraming(targetObjectToFrame);
        }

        public void RecalculateFraming(Transform parent)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers.Length == 0 ? new Bounds(parent.position, Vector3.one) : renderers[0].bounds;

            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            _bounds = bounds;
        }

        private void LateUpdate()
        {
            if (!targetObjectToFrame && !targetCamera)
                return;

            if (!Application.isPlaying && !debug)
                return;

            if (!Application.isPlaying && debug)
                RecalculateFraming();

            targetCamera.transform.LookAt(_bounds.center, Vector3.up);
            targetCamera.transform.position =
                _bounds.center - targetCamera.transform.forward * (scale * _bounds.size.magnitude);
            targetCamera.transform.Rotate(angleOffset, Space.Self);
        }
    }
}