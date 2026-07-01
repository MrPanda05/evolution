using Evolution.Creatures;
using UnityEngine;

namespace Evolution.Resources
{
    public class Water : MonoBehaviour, IConsumable
    {
        public event System.Action OnCosume;

        public void Consume()
        {
            print("Drunk water");
        }

        public void Consume(BaseCreature creature)
        {
            creature.ThirstComponent.IncreaseThirst(creature.ThirstComponent.MaxThirst);
        }

        public void Regrow()
        {
        }

        void Start()
        {
            float radius = Random.Range(4f, 10f);
            transform.localScale = new Vector3(radius,radius);
        }

    }
}
