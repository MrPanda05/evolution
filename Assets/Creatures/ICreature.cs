using Evolution.Neural;
using UnityEngine;

namespace Evolution.Creatures
{
    public interface ICreature
    {
        CreatureStats Stats { get; }
        void Move(Vector2 direction);
        //Sexual
        bool Reproduce(ICreature mom, ICreature dad, int dadPopulation, int momPopulation);
        //Asexual
        void Reproduce();
    }
}
