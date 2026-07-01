using UnityEngine;
using System;
namespace Evolution.Commons.Components
{
    public class HungerComponent : MonoBehaviour
    {
        [field: SerializeField]
        public float MaxHunger { get; private set; }
        [field: SerializeField]
        public float Hunger { get; private set; }

        public event Action OnHungerChanged;
        public event Action OnFullyHungry;


        public void Initialize(float maxHunger, float hunger)
        {
            MaxHunger = maxHunger;
            Hunger = hunger;
        }

        public void DecreaseHunger(float amout)
        {
            Hunger -= amout;
            if (Hunger <= 0)
            {
                Hunger = 0;
                OnFullyHungry?.Invoke();
            }
            OnHungerChanged?.Invoke();
        }
        public void IncreaseHunger(float amount)
        {
            Hunger += amount;
            OnHungerChanged?.Invoke();
            if (Hunger > MaxHunger)
            {
                Hunger = MaxHunger;
            }
        }
    }
}
