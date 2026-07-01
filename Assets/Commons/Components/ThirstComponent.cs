using UnityEngine;
using System;

namespace Evolution.Commons.Components
{
    public class ThirstComponent : MonoBehaviour
    {
        [field: SerializeField]
        public float MaxThirst { get; private set; }
        [field: SerializeField]
        public float Thirst { get; private set; }

        public event Action OnThirstChanged;
        public event Action OnFullyThirsty;


        public void Initialize(float maxThirst, float thirst)
        {
            MaxThirst = maxThirst;
            Thirst = thirst;
        }

        public void DecreaseThirst(float amout)
        {
            Thirst -= amout;
            if (Thirst <= 0)
            {
                Thirst = 0;
                OnFullyThirsty?.Invoke();
            }
            OnThirstChanged?.Invoke();
        }
        public void IncreaseThirst(float amount)
        {
            Thirst += amount;
            OnThirstChanged?.Invoke();
            if (Thirst > MaxThirst)
            {
                Thirst = MaxThirst;
            }
        }
    }
}
