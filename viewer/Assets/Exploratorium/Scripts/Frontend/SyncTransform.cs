using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Exploratorium.Frontend
{
    /// <summary>
    /// Performs simple interpolated transform synchronization based on a series of sync-events
    /// </summary>
    public class SyncTransform : MonoBehaviour
    {
        [SerializeField] private Vector3Event positionReport;
        [SerializeField] private Vector3Event positionSync;
        [SerializeField] private Vector3Event scaleReport;
        [SerializeField] private Vector3Event scaleSync;
        [SerializeField] private QuaternionEvent rotationReport;
        [SerializeField] private QuaternionEvent rotationSync;
        [SerializeField] private float maxUpdateRate = 30f;
        [SerializeField] private float interpolationSharpness = 5f;
        [SerializeField] private bool debug = false;

        public bool PermitSync { get; set; } = false;
        public bool IsPublisher { get; set; } = false;
        public bool IsConsumer { get; set; } = false;

        private float _nextUpdate;
        private Vector3 _prevPos;
        private Vector3 _positionTarget;
        private Vector3 _scaleTarget;
        private Vector3 _prevScale;
        private Quaternion _rotationTarget;
        private Quaternion _prevRot;

        private void Awake()
        {
            Debug.Assert(!debug, "!debug", this);
            Debug.Assert(
                positionReport == null && positionSync == null || positionReport != null && positionSync != null,
                "XOR both position events are null or both are assigned", this);
            Debug.Assert(
                rotationReport == null && rotationSync == null || rotationReport != null && rotationSync != null,
                "XOR both rotation events are null or both are assigned", this);
            Debug.Assert(scaleReport == null && scaleSync == null || scaleReport != null && scaleSync != null,
                "XOR both scale events are null or both are assigned", this);
            Debug.Assert(positionReport != null ||
                         positionSync != null ||
                         rotationReport != null ||
                         rotationSync != null ||
                         scaleSync != null ||
                         scaleReport != null,
                "Any sync event is assigned", this);
        }

        private void OnEnable()
        {
            if (positionSync != null)
                positionSync.Register(OnPositionSync);
            if (rotationSync != null)
                rotationSync.Register(OnRotationSync);
            if (scaleSync != null)
                scaleSync.Register(OnScaleSync);
        }

        private void OnDisable()
        {
            if (positionSync != null)
                positionSync.Unregister(OnPositionSync);
            if (rotationSync != null)
                rotationSync.Unregister(OnRotationSync);
            if (scaleSync != null)
                scaleSync.Unregister(OnScaleSync);
        }

        private void OnPositionSync(Vector3 localPosition)
        {
            if (debug)
                Debug.Log(
                    $"<{nameof(SyncTransform)}> : Sync position {localPosition:F2} received ({nameof(PermitSync)}: {PermitSync}, {nameof(IsConsumer)}: {IsConsumer})",
                    this);
            _positionTarget = localPosition;
        }

        private void OnRotationSync(Quaternion localRotation)
        {
            if (debug)
                Debug.Log(
                    $"<{nameof(SyncTransform)}> : Sync rotation {localRotation.eulerAngles:F2} received ({nameof(PermitSync)}: {PermitSync}, {nameof(IsConsumer)}: {IsConsumer}",
                    this);
            _rotationTarget = localRotation;
        }

        private void OnScaleSync(Vector3 localScale)
        {
            if (debug)
                Debug.Log(
                    $"<{nameof(SyncTransform)}> : Sync scale {localScale:F2} received ({nameof(PermitSync)}: {PermitSync}, {nameof(IsConsumer)}: {IsConsumer}",
                    this);
            _scaleTarget = localScale;
        }

        private void Update()
        {
            if (!PermitSync)
                return;

            Transform self = transform;

            if (IsConsumer)
            {
                // interpolate for OBSERVER
                
                transform.localPosition = Vector3.Lerp(self.localPosition, _positionTarget,
                    1f - Mathf.Exp(-interpolationSharpness * Time.deltaTime));
                transform.localRotation = Quaternion.Lerp(self.localRotation, _rotationTarget,
                    1f - Mathf.Exp(-interpolationSharpness * Time.deltaTime));
                transform.localScale = Vector3.Lerp(self.localScale, _scaleTarget,
                    1f - Mathf.Exp(-interpolationSharpness * Time.deltaTime));
            }
            else if (IsPublisher && _nextUpdate < Time.time)
            {
                // report for CONTROLLER
                
                _nextUpdate = Time.time + 1f / maxUpdateRate;
                if (positionReport != null)
                {
                    float posDelta = (self.localPosition - _prevPos).sqrMagnitude;
                    if (posDelta > 0.01f)
                    {
                        Vector3 localPosition = self.localPosition;
                        positionReport.Raise(localPosition);
                        _prevPos = localPosition;
                    }
                }

                if (rotationReport != null)
                {
                    float angleDelta = (self.rotation.eulerAngles - _prevRot.eulerAngles).sqrMagnitude;
                    if (angleDelta > 0.1f)
                    {
                        Quaternion localRotation = self.localRotation;
                        rotationReport.Raise(localRotation);
                        _prevRot = localRotation;
                    }
                }

                if (scaleReport != null)
                {
                    float scaleDelta = (self.localScale - _prevScale).sqrMagnitude;
                    if (scaleDelta > 0.01f)
                    {
                        Vector3 localScale = self.localScale;
                        scaleReport.Raise(localScale);
                        _prevScale = localScale;
                    }
                }
            }
        }
    }
}