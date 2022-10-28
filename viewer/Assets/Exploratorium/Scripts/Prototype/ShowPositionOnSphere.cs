// //////////////////////////////////////////////////////
// THIS IS PROTOTYPE CODE AND NOT INTENDED FOR PRODUCTION
// TODO: Make this production ready
// //////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exploratorium.Prototype
{
    [Obsolete]
    public class ShowPositionOnSphere : MonoBehaviour
    {
        [SerializeField] private GameObject pinPrefab;
        [SerializeField] private Renderer sphereRenderer;
        [SerializeField] private Camera cam;

        [Min(1)]
        [SerializeField] private int maxPinCount = 1;

        [SerializeField] private Vector2 offset;

        [SerializeField] private bool debug;
        [SerializeField] private Vector2 debugCoordinates;

        private void OnValidate()
        {
            if (!Application.isPlaying && debug)
                ShowPosition(debugCoordinates);
        }

        private readonly List<GameObject> _pins = new List<GameObject>();
        [SerializeField] private float cameraDistance = 15f;
        private Vector3 _camPos;
        private Quaternion _pinRot;

        public void ShowPosition(float longitude, float latitude) => ShowPosition(new Vector2(longitude, latitude));

        public void ShowPosition(Vector2 coordinates)
        {
            if (sphereRenderer == null)
                return;
            if (pinPrefab == null)
                return;

            TruncateTo(maxPinCount - 1);
            var bounds = sphereRenderer.bounds;
            var center = bounds.center;
            var radius = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);
            _pinRot = Quaternion.Euler(coordinates.y + offset.y, coordinates.x + offset.x, 0);
            var pinPos = _pinRot * (Vector3.back * radius);
            var pin = Instantiate(pinPrefab, transform);
            pin.transform.position = pinPos + bounds.center;
            pin.transform.LookAt(bounds.center, pin.transform.up);
            _pins.Add(pin);
        }

        private void Update()
        {
            if (cam != null)
            {
                cam.transform.position = _pinRot * (Vector3.forward * cameraDistance);
                cam.transform.LookAt(sphereRenderer.bounds.center, transform.up);
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < _pins.Count; i++)
                DestroyPin(i);

            _pins.Clear();
        }

        private void TruncateTo(int count)
        {
            if (count >= 0 && _pins.Count > count)
            {
                var toRemove = _pins.Count - count;
                for (int i = 0; i < toRemove; i++)
                    DestroyPin(i);
                _pins.RemoveRange(0, toRemove); // fifo
            }
        }

        private void DestroyPin(int i)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (_pins[i] != null)
                    DestroyImmediate(_pins[i]);
            }
            else
            {
                if (_pins[i] != null)
                    Destroy(_pins[i]);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (sphereRenderer == null)
                return;

            foreach (var pin in _pins)
            {
                if (pin == null)
                    continue;
                var position = pin.transform.position;
                Gizmos.DrawRay(position, (position - sphereRenderer.bounds.center).normalized);
            }
        }

        private void OnDestroy()
        {
            ClearAll();
        }
    }
}