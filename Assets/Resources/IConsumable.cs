using Evolution.Creatures;
using System;
using UnityEngine;

namespace Evolution.Resources
{
    //For consumable, things that creatures can consume, such as water, fruits, vegetables, even other creatures.
    public interface IConsumable
    {
        //When consuming something
        void Consume();
        //When consuming something, but pass who is consuming, so that maybe the one being consumed to do something
        void Consume(BaseCreature creature);
        //If it is a plant, maybe the fruits regrow
        void Regrow();
        event Action OnCosume;
    }
}
