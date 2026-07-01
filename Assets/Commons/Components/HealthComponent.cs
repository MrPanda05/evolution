using System;
using UnityEngine;

namespace Evolution.Commons.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [field:SerializeField]
        public float MaxHealth { get; private set; }
        [field:SerializeField]
        public float Health { get; private set; }

        public event Action OnHealthChanged;
        public event Action OnDeath;


        public void Initialize(float maxHealth, float health)
        {
            MaxHealth = maxHealth;
            Health = health;
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if(Health <= 0)
            {
                Health = 0;
                OnDeath?.Invoke();
            }
            OnHealthChanged?.Invoke();
        }
        public void Heal(float heal)
        {
            Health += heal;
            OnHealthChanged?.Invoke();
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

    }
}
