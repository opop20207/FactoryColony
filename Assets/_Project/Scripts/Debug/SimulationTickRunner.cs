using System;
using UnityEngine;

namespace FactoryColony
{
    public sealed class SimulationTickRunner : MonoBehaviour
    {
        [SerializeField] private float tickIntervalSeconds = 1f;
        [SerializeField] private bool startRunning = true;

        private float _elapsedSeconds;
        private FactorySimulation _simulation;

        public event Action<int> OnTickExecuted;

        public FactorySimulation Simulation
        {
            get { return _simulation; }
        }

        public bool IsRunning { get; private set; }
        public int TickCount { get; private set; }

        public void Initialize(FactorySimulation simulation)
        {
            _simulation = simulation;
            EnsureValidTickInterval();

            if (startRunning)
            {
                StartSimulation();
            }
        }

        public void StartSimulation()
        {
            IsRunning = true;
        }

        public void StopSimulation()
        {
            IsRunning = false;
        }

        public void StepOnce()
        {
            if (_simulation == null)
            {
                return;
            }

            _simulation.SimulateTick();
            TickCount++;
            OnTickExecuted?.Invoke(TickCount);
        }

        private void Update()
        {
            if (!IsRunning || _simulation == null)
            {
                return;
            }

            EnsureValidTickInterval();
            _elapsedSeconds += Time.deltaTime;

            while (_elapsedSeconds >= tickIntervalSeconds)
            {
                _elapsedSeconds -= tickIntervalSeconds;
                StepOnce();
            }
        }

        private void EnsureValidTickInterval()
        {
            if (tickIntervalSeconds > 0f)
            {
                return;
            }

            Debug.LogWarning("Simulation tick interval must be greater than 0. Falling back to 1 second.");
            tickIntervalSeconds = 1f;
        }
    }
}
