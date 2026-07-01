using UnityEngine;
using System;

namespace Evolution.Commons.Components
{
    public class EnergyComponent : MonoBehaviour
    {
        [field: SerializeField]
        public float MaxEnergy { get; private set; }
        [field: SerializeField]
        public float Energy { get; private set; }

        public event Action OnEnergyChanged;
        public event Action OnOutEnergy;


        public void Initialize(float maxEnergy, float energy)
        {
            MaxEnergy = maxEnergy;
            Energy = energy;
        }

        public void ReduceEnergy(float amout)
        {
            Energy -= amout;
            if (Energy <= 0)
            {
                Energy = 0;
                OnOutEnergy?.Invoke();
            }
            OnEnergyChanged?.Invoke();
        }
        public void IncreaseEnergy(float amount)
        {
            Energy += amount;
            OnEnergyChanged?.Invoke();
            if (Energy > MaxEnergy)
            {
                Energy = MaxEnergy;
            }
        }
    }
}
