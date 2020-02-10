﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace GlobalGameJam.Hovercraft
{
    [Serializable]
    /*public class HoverCraftEngine
    {
        [SerializeField] private Transform _thrustersRoot;
        [SerializeField] private DownThrusterController _downThrusterController;

        public HoverCraftEngine LeftEngine => _leftEngine;
        public HoverCraftEngine RightEngine => _rightEngine;
        }*/

    public class ThirdPersonHoverCraftController : MonoBehaviour ,IControlled
    {
        [SerializeField] private Transform _thrustersRoot;
        [SerializeField] private DownThrusterController _downThrusterController;
       
        public HoverCraftEngine LeftEngine => _leftEngine;
        public HoverCraftEngine RightEngine => _rightEngine;

        [SerializeField] 
        private Rigidbody _rigidbody;
        private HoverCraftEngine[] _engines = new HoverCraftEngine[0];

        private LayerMask _hoverOverLayer;
        
        [SerializeField, Range(0,1)] 
        private float _powerDistribution;
        
        private float _leftThrustUp;
        private float _rightThrustUp;
        public Player ControlledBy { get; set; }

        [SerializeField] 
        private HoverCraftEngine _leftEngine;
        [SerializeField] 
        private HoverCraftEngine _rightEngine;

        private void OnValidate()
        {
            if(_downThrusterController != null)
                _downThrusterController.HoverCraftFloatForcePoints = _thrustersRoot.GetComponentsInChildren<Transform>();
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

        private void FixedUpdate()
        {
            _downThrusterController.ApplyThrustUpwards(_rigidbody);
            //ApplyEngineThrust();
            LeftEngine.RotateThruster();
            LeftEngine.UpdateParticles();
            RightEngine.RotateThruster();
            RightEngine.UpdateParticles();
        }
/*
        public void ControlLeftThruster(InputAction.CallbackContext value)
        {
            ControlThruster(LeftEngine, value.ReadValue<Vector2>());
        }
*/
        /*
        public void ControlRightThruster(InputAction.CallbackContext value)
        {
            ControlThruster(RightEngine, value.ReadValue<Vector2>());
        }
        */


        /*
        public void ControlThrustUpLeft(InputAction.CallbackContext value)
        {
            _leftThrustUp = value.ReadValue<float>();
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            ThrusterController.PowerSetting = totalPower;
        }

        public void ControlThrustUpRight(InputAction.CallbackContext value)
        {
            _rightThrustUp = value.ReadValue<float>();
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            ThrusterController.PowerSetting = totalPower;
        }
      

        public void RequestEndControl(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                OnControlEnd?.Invoke(this);
            }
        }  
        */

        

        public void StartControl(Player _controlledby)
        {
            ControlledBy = _controlledby;
            
            foreach (var engine in _engines)
            {
                _rigidbody.AddForceAtPosition(engine.Thruster.forward * engine.Thrust, engine.Thruster.position);
            }
        }

        public event Action<IControlled> OnControlEnd;
        public void EndControl()
        {
            LeftEngine.EnginePower = 0;
            RightEngine.EnginePower = 0;
            _downThrusterController.PowerSetting = 0;
           // ThrusterController.PowerSetting = 0;
        }

        public void StickLeft(Vector2 move)
        {
           // throw new NotImplementedException();
            ControlThruster(LeftEngine, move);
        }

        public void StickRight(Vector2 vec)
        {
        //    throw new NotImplementedException();
            ControlThruster(RightEngine, vec);
            
        }
        private void ControlThruster(HoverCraftEngine thruster, Vector2 direction)
        {
            thruster.EnginePower = direction.magnitude;
            thruster.Direction = new Vector3(direction.x, 0, direction.y);
        }

        public void OnTriggerLeft(float f)
        {
            _leftThrustUp = f;
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            _downThrusterController.PowerSetting = totalPower;
        }

        public void OnTriggerRight(float f)
        {

            _rightThrustUp = f;
            var totalPower = _leftThrustUp * (_powerDistribution) + _rightThrustUp * (1f - _powerDistribution);
            _downThrusterController.PowerSetting = totalPower;
        }

        public void HandleInteract()
        {
            //throw new NotImplementedException();
        }

        public void HandleStopInteract()
        {
            Debug.Log("INTERACTIONG" );
            OnControlEnd?.Invoke(this);
        }

        public void HandleRepair()
        {
         //   throw new NotImplementedException();
        }
    }
}