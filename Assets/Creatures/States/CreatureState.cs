using Evolution.Commons.StateMachine;
using Evolution.Creatures;
using UnityEngine;

namespace Evolution.Creatures.States
{
    /// <summary>
    /// Base state for a creature, inherit if to create new generic states
    /// 
    /// Depending on the creature, inherit this to create a new state class for that type of creature. For example, if you have a "Wolf" creature, you might create a "WolfState" class that inherits from "CreatureState". This allows you to define specific behaviors and logic for that creature type while still leveraging the common functionality provided by the base "CreatureState" class.
    /// </summary>
    public class CreatureState : State
    {
        protected BaseCreature _creature;

        public override void Ready()
        {
            if(_creature == null)
            {
                _creature = StateMachine.GetComponentInParent<BaseCreature>();
            }
        }
    }
}
