using Evolution.Neural;
using UnityEngine;

namespace Evolution.Creatures
{
    public interface ICreature
    {
        CreatureStats Stats { get; }
        void Move(Vector2 direction);
        //Sexual
        bool Reproduce(ICreature creature1, ICreature creature2, int creature1Population, int creature2Population);
        //Asexual
        void Reproduce();
    }
}
