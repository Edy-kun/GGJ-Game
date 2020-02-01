﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

namespace GlobalGameJam.Hovercraft
{
    public class ThirdPersonHoverCraftController : MonoBehaviour, IControlled
    {
        public bool IsActivelyControlled
        {
            get => _isActivelyControlled;
            private set
            {
                if(_isActivelyControlled != value)
                {
                    _isActivelyControlled = value;
                    Debug.Log($"{nameof(IsActivelyControlled)} changed to {value}");
                }
            }
        }

        [SerializeField] private bool _isActivelyControlled = false;
        [SerializeField] private Transform _thrustersRoot;
        [SerializeField] private DownThrusterController _downThrusterController;

        
        public HoverCraftEngine LeftEngine => _leftEngine;


        public HoverCraftEngine RightEngine => _rightEngine;

        public DownThrusterController ThrusterController => _downThrusterController;

        [SerializeField] private Rigidbody _rigidbody;
        private HoverCraftEngine[] _engines = new HoverCraftEngine[0];

        private LayerMask _hoverOverLayer;

        [SerializeField, Range(0,1)] private float _powerDistribution;
        private float _leftThrustUp;
        private float _rightThrustUp;
        
        [FormerlySerializedAs("LeftEngine"), SerializeField] private HoverCraftEngine _leftEngine;
        [FormerlySerializedAs("RightEngine"), SerializeField] private HoverCraftEngine _rightEngine;

        private void OnValidate()
        {
            if(ThrusterController != null)ThrusterController.HoverCraftFloatForcePoints = _thrustersRoot.GetComponentsInChildren<Transform>();
        }

        private void Awake()
        {
            _engines = new[] {LeftEngine, RightEngine};
            ThrusterController.HoverCraftFloatForcePoints = _thrustersRoot.GetComponentsInChildren<Transform>();
            StartControl();
            
            //for debug purposes. pretend the exit control was accepted
            OnControlEnd += (a) => EndControl();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var engine in _engines)
            {
                engine.DrawGizmo();
            }

            ThrusterController?.DrawGizmos();
        }

        private void Update()
        {
            ControlThrustUp();
        }
        private void FixedUpdate()
        {
            ThrusterController.ApplyThrustUpwards(_rigidbody);
            
            LeftEngine.RotateThruster();
            LeftEngine.UpdateParticles();
            RightEngine.RotateThruster();
            RightEngine.UpdateParticles();
            LeftEngine.ApplyThrust(_rigidbody);
            RightEngine.ApplyThrust(_rigidbody);
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
            if (!IsActivelyControlled) return;
            
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

        public void RequestEndControl(InputAction.CallbackContext value)
        {
            if (IsActivelyControlled)
            {
                OnControlEnd?.Invoke(this);
            }
            else StartControl();
        }

        private void ControlThrustUp()
        {
            if (!IsActivelyControlled) return;
            
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            ThrusterController.PowerSetting = totalPower;
        }


        public void StartControl()
        {
            IsActivelyControlled = true;
            Debug.Log("Winding down engines");
            ThrusterController.PowerSetting = 0;
            foreach (var engine in _engines)
            {
                engine.Direction = Vector3.forward;
                engine.EnginePower = 0;
            }
        }

        public event Action<IControlled> OnControlEnd;
        public void EndControl()
        {
            IsActivelyControlled = false;
            LeftEngine.EnginePower = 0;
            RightEngine.EnginePower = 0;
            ThrusterController.PowerSetting = 0;
        }
    }
}