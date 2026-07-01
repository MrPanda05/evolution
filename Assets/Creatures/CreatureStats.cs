using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Evolution.Creatures
{
    [CreateAssetMenu(menuName ="CreatureData", fileName ="Creatures")]
    public class CreatureStats: ScriptableObject
    {
        public bool Sex;//0-male 1-female
        public float MaxHealth;
        public float Speed;
        public float MaxHunger;
        public float MaxThirst;
        public float MaxEnergy;
        public float Atractivess;
        public float Inteligence;
        public float HungerDrainRate;
        public float ThirstDrainRate;
        public float EnergyDrainRate;
        public float SightRange;
        public float MaxAliveTime;
        /// <summary>
        /// The point at which the creature is considered hungry
        /// </summary>
        public float HungerTreshHold;
        /// <summary>
        /// The point where the creature is thirsty
        /// </summary>
        public float ThirstTreshHold;
        /// <summary>
        /// The point where the creature is sleepy
        /// </summary>
        public float EnergyTreshHold;

        public Color Color;
        public LayerMask SightLayer;

        public void RandomSex()
        {
            Sex = Random.Range(0, 2) == 0 ? false : true;
        }
    }
}
