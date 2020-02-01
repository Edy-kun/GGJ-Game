using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace GlobalGameJam.Hovercraft
{
    [Serializable]
    public class HoverCraftEngine
    {
        [SerializeField] private Transform _hoverCraft;
        [Header("Direction")] [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _thruster;
        [SerializeField] private float _engineRotationRate;

        public Transform Pivot => _pivot;

        public Transform Thruster => _thruster;

        [Header("Power")] public ForceMode forceMode;
        [SerializeField] private float _minThrust;
        [SerializeField] private float _maxThrust;
        private float _enginePower;

        [Header("Visuals")] 
        [SerializeField] private ParticleSystem _particleSystem;

        [SerializeField] private float _minLifeTime;
        [SerializeField] private float _maxLifeTime;


        private float DirectionalModifier =>
            Vector3.Dot(_hoverCraft.InverseTransformDirection(_pivot.forward), Direction);
        public float Thrust => Mathf.Lerp(_minThrust, _maxThrust, EnginePower) * DirectionalModifier;

        public float EnginePower
        {
            get => _enginePower;
            set => _enginePower = value;
        }

        public Vector3 Direction { get; set; }

        public float EngineRotationRate
        {
            get => _engineRotationRate;
            set => _engineRotationRate = value;
        }

        public void RotateThruster()
        {
            if (Direction.magnitude < 0.02) Direction = Vector3.forward;
            var localDirection = Direction;
            var old = Pivot.localRotation;
            var goal = Quaternion.LookRotation(localDirection, Vector3.up);
            var current = Quaternion.RotateTowards(old, goal, EngineRotationRate * Time.fixedDeltaTime);
            Pivot.localRotation = current;
        }

        public void UpdateParticles()
        {
            var main = _particleSystem.main;
            main.startLifetimeMultiplier = Mathf.Lerp(_minLifeTime, _maxLifeTime, EnginePower * DirectionalModifier);
        }

        public void DrawGizmo()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_thruster.position, _thruster.forward * Thrust);

            Gizmos.color = Color.red;
            var dir = new Vector3(Direction.x, 0, Direction.y);
            Gizmos.DrawRay(_pivot.position, _hoverCraft.TransformDirection(dir) * 10);
        }
    }

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

    public class ThirdPersonHoverCraftController : MonoBehaviour
    {
        [SerializeField] private Transform _thrustersRoot;
        [SerializeField] private DownThrusterController _downThrusterController;
        [SerializeField] private HoverCraftEngine LeftEngine;
        [SerializeField] private HoverCraftEngine RightEngine;

        [SerializeField] private Rigidbody _rigidbody;
        private HoverCraftEngine[] _engines = new HoverCraftEngine[0];

        private LayerMask _hoverOverLayer;
        
        [SerializeField, Range(0,1)] private float _powerDistribution;
        private float _leftThrustUp;
        private float _rightThrustUp;

        private void OnValidate()
        {
            if(_downThrusterController != null)_downThrusterController.HoverCraftFloatForcePoints = _thrustersRoot.GetComponentsInChildren<Transform>();
        }

        private void Awake()
        {
            _engines = new[] {LeftEngine, RightEngine};
            _downThrusterController.HoverCraftFloatForcePoints = _thrustersRoot.GetComponentsInChildren<Transform>();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var engine in _engines)
            {
                engine.DrawGizmo();
            }

            _downThrusterController?.DrawGizmos();
        }

        private void Update()
        {
            ControlThrustUp();
        }
        private void FixedUpdate()
        {
            _downThrusterController.ApplyThrustUpwards(_rigidbody);
            ApplyEngineThrust();
            LeftEngine.RotateThruster();
            LeftEngine.UpdateParticles();
            RightEngine.RotateThruster();
            RightEngine.UpdateParticles();
        }

        public void ControlLeftThruster(InputAction.CallbackContext value)
        {
            ControlThruster(LeftEngine, value.ReadValue<Vector2>());
        }

        public void ControlRightThruster(InputAction.CallbackContext value)
        {
            ControlThruster(RightEngine, value.ReadValue<Vector2>());
        }

        private void ControlThruster(HoverCraftEngine thruster, Vector2 direction)
        {
            thruster.EnginePower = direction.magnitude;
            thruster.Direction = new Vector3(direction.x, 0, direction.y);
        }

        public void ControlThrustUpLeft(InputAction.CallbackContext value)
        {
            _leftThrustUp = value.ReadValue<float>();
        }

        public void ControlThrustUpRight(InputAction.CallbackContext value)
        {
            _rightThrustUp = value.ReadValue<float>();
        }
        public void ControlThrustUp()
        {
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            _downThrusterController.Power = totalPower;
        }

        

        private void ApplyEngineThrust()
        {
            foreach (var engine in _engines)
            {
                _rigidbody.AddForceAtPosition(engine.Thruster.forward * engine.Thrust, engine.Thruster.position);
            }
        }
    }
}