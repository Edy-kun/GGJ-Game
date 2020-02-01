using UnityEngine;

namespace GlobalGameJam.Hovercraft
{
    [System.Serializable]
    public class DownThrusterController
    {
        private Transform[] _hoverCraftFloatForcePoints;
        [SerializeField] private float _minDistance = 0;
        [SerializeField] private float _maxDistance = 2;
        [SerializeField] private float _force = 1;
        [SerializeField] private AnimationCurve _forceMultiplier = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private LayerMask _hoverOverLayer;
        [SerializeField, Range(0,1)] private float[] _thrusterEffectiveness;
        [SerializeField, Range(0,1)] private float _power = 0;

        public float Power
        {
            get => _power;
            set => _power = Mathf.Clamp01(value);
        }

        public float[] Forces { get; private set; }

        public float[] ThrusterEffectiveness
        {
            get => _thrusterEffectiveness;
            private set => _thrusterEffectiveness = value;
        }

        public Transform[] HoverCraftFloatForcePoints
        {
            get => _hoverCraftFloatForcePoints;
            set
            {
                _hoverCraftFloatForcePoints = value;
                Forces = new float[_hoverCraftFloatForcePoints.Length];
                ThrusterEffectiveness = new float[_hoverCraftFloatForcePoints.Length];
                for (var i = 0; i < ThrusterEffectiveness.Length; i++)
                {
                    ThrusterEffectiveness[i] = 1f;
                }
            }
        }

        public void ApplyThrustUpwards(Rigidbody rigidBody)
        {
            for (var i = 0; i < HoverCraftFloatForcePoints.Length; i++)
            {
                var thruster = HoverCraftFloatForcePoints[i];
                if (Physics.Raycast(thruster.position, Vector3.down, out var hit, _maxDistance, _hoverOverLayer.value))
                {
                    var d = Mathf.Clamp(hit.distance, _minDistance, _maxDistance);
                    d = Mathf.InverseLerp(_minDistance, _maxDistance, d);
                    var upForceAmount = ThrusterEffectiveness[i] * _forceMultiplier.Evaluate(d) * _force * Power;
                    rigidBody.AddForceAtPosition(Vector3.up * upForceAmount, thruster.position);
                    Forces[i] = upForceAmount;
                }
            }
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < HoverCraftFloatForcePoints.Length; i++)
            {
                Gizmos.DrawRay(HoverCraftFloatForcePoints[i].position, Vector3.down * Forces[i]);
            }
        }
    }
}