using System;
using UnityEngine;

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
        public float Effectiveness { get; set; } = 1f;

        [Header("Visuals")] 
        [SerializeField] private ParticleSystem _particleSystem;

        [SerializeField] private float _minLifeTime;
        [SerializeField] private float _maxLifeTime;


        private float DirectionalModifier =>
            Vector3.Dot(_hoverCraft.InverseTransformDirection(_pivot.forward), Direction);
        public float Thrust => Mathf.Lerp(_minThrust, _maxThrust, EnginePower) * DirectionalModifier * Effectiveness;

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

        public void ApplyThrust(Rigidbody rigidbody)
        {
            rigidbody.AddForceAtPosition(Thruster.forward * Thrust, Thruster.position);
        }
    }
}