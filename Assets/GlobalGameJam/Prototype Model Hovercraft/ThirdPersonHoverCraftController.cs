using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace GlobalGameJam.Hovercraft
{
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