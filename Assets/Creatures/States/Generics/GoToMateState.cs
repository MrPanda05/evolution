using Evolution.Commons.Components;
using Evolution.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

namespace Evolution.Creatures.States.Generics
{
    public class GoToMateState : CreatureState
    {
        private BaseCreature[] _creaturesOnSight;
        private BaseCreature _targetMate;
        private Vector2 _direction;
        public bool _hasReachTarget;
        private List<BaseCreature> _excludeCreatures = new List<BaseCreature>();
        private TimerComponent _forgetExcludedTimer;


        public override void Ready()
        {
            base.Ready();
            _forgetExcludedTimer = GetComponent<TimerComponent>();
        }
        public override void Enter()
        {
            List<BaseCreature> resourcesInSight = new List<BaseCreature>();
            foreach (var item in _creature.SightData)
            {
                if (item == null) continue;
                if (item.TryGetComponent<BaseCreature>(out BaseCreature creature))
                {
                    if (creature.Stats.Sex == _creature.Stats.Sex) continue;
                    if(_excludeCreatures.Contains(creature)) continue;
                    resourcesInSight.Add(creature);
                }
            }
            _creaturesOnSight = resourcesInSight.ToArray();
            _targetMate = CalculateFastest();
            _hasReachTarget = false;
            _forgetExcludedTimer.StartTimer();
            _forgetExcludedTimer.Loop();
            _forgetExcludedTimer.OnTimeFinished += OnTimeOut;
        }

        private void OnTimeOut()
        {
            if(!IsActive) return;
            _excludeCreatures.Clear();
        }

        public override void Exit()
        {
            _forgetExcludedTimer.StopLooping();
            _forgetExcludedTimer.OnTimeFinished -= OnTimeOut;
        }
        public override void Process()
        {
            
            if (_creature.HasJustMated || _creaturesOnSight.Length == 0)
            {
                StateMachine.ChangeState("WanderState");
                return;
            }
            if (_hasReachTarget)
            {
                bool success = false;
                if (_creature.IsFemale)
                {
                    success = _creature.Reproduce(_creature, _targetMate, _targetMate.CurrentPopulation, _creature.CurrentPopulation);
                }
                else
                {
                    success = _creature.Reproduce(_targetMate, _creature, _creature.CurrentPopulation, _targetMate.CurrentPopulation);
                }
                if (success)
                {
                    _creature.SetHasReproduced(true);
                    _targetMate.SetHasReproduced(true);
                    _creature.HungerComponent.DecreaseHunger(10);
                    _creature.ThirstComponent.DecreaseThirst(10);
                    _creature.EnergyComponent.ReduceEnergy(10);
                    _targetMate.HungerComponent.DecreaseHunger(10);
                    _targetMate.ThirstComponent.DecreaseThirst(10);
                    _targetMate.EnergyComponent.ReduceEnergy(10);
                }
                else
                {
                    _excludeCreatures.Add(_targetMate);
                }
                StateMachine.ChangeState("WanderState");
                return;
            }
            if(_targetMate == null && _creaturesOnSight.Length == 0)
            {
                StateMachine.ChangeState("WanderState");
                return;
            }
            if(_targetMate == null)
            {
                _targetMate = CalculateFastest();
                if (_targetMate == null)
                {
                    StateMachine.ChangeState("WanderState");
                    return;
                }
            }
            if (Vector2.Distance(_creature.transform.position, _targetMate.transform.position) < 0.5f)
            {
                _hasReachTarget = true;
            }
            _direction = (_targetMate.transform.position - _creature.transform.position).normalized;
            _creature.Move(_direction);
            //print("Going to Mate: " + _targetMate.name);

        }

        public BaseCreature CalculateFastest()
        {
            if (_creaturesOnSight.Length == 0) return null;
            BaseCreature closestResource = null;
            float closestDistance = float.MaxValue;
            foreach (var resource in _creaturesOnSight)
            {
                if (resource == null) continue;
                float distance = Vector2.Distance(_creature.transform.position, ((MonoBehaviour)resource).transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestResource = resource;
                }
            }
            return closestResource;
        }
    }
}
