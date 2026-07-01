using Evolution.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Evolution.Creatures.States.Generics
{
    public class GoToResourceState : CreatureState
    {
        private IConsumable[] _resourcesOnSight;
        private GameObject _targetResource;
        private Vector2 _direction;
        public bool _hasReachTarget;

        public override void Enter()
        {
            List<IConsumable> resourcesInSight = new List<IConsumable>();
            foreach (var item in _creature.SightData)
            {
                if (item == null) continue;
                if (item.TryGetComponent<IConsumable>(out IConsumable consumable))
                    resourcesInSight.Add(consumable);
            }
            _resourcesOnSight = resourcesInSight.ToArray();
            _targetResource = CalculateFastest();
            _hasReachTarget = false;
        }
        public override void Process()
        {
            if (!_creature.IsThirsty && !_creature.IsHungry)
            {
                StateMachine.ChangeState("WanderState");
                return;
            }
            if (_hasReachTarget)
            {
                StateMachine.ChangeState("WanderState");
                return;
            }
            if(_targetResource == null && _resourcesOnSight.Length == 0)
            {
                StateMachine.ChangeState("WanderState");
                return;
            }
            if(_targetResource == null)
            {
                _targetResource = CalculateFastest();
                if (_targetResource == null)
                {
                    StateMachine.ChangeState("WanderState");
                    return;
                }
            }
            if (Vector2.Distance(_creature.transform.position, _targetResource.transform.position) < 0.5f)
            {
                _hasReachTarget = true;
                if (_targetResource.TryGetComponent<IConsumable>(out IConsumable consumable))
                {
                    consumable.Consume(_creature);
                    StateMachine.ChangeState("WanderState");
                }
            }
            _direction = (_targetResource.transform.position - _creature.transform.position).normalized;
            _creature.Move(_direction);
            //print("Going to resource: " + _targetResource.name);

        }

        public GameObject CalculateFastest()
        {
            if (_resourcesOnSight.Length == 0) return null;
            GameObject closestResource = null;
            float closestDistance = float.MaxValue;
            foreach (var resource in _resourcesOnSight)
            {
                if (resource == null) continue;
                if(!_creature.IsThirsty && resource is Water) continue;
                if(!_creature.IsHungry && resource is Fruit) continue;
                float distance = Vector2.Distance(_creature.transform.position, ((MonoBehaviour)resource).transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestResource = ((MonoBehaviour)resource).gameObject;
                }
            }
            return closestResource;
        }
    }
}
