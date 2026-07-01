using Evolution.Commons.Components;
using Evolution.Commons.Managers;
using Evolution.Commons.StateMachine;
using Evolution.Resources;
using UnityEditorInternal;
using UnityEngine;

namespace Evolution.Creatures.States.Generics
{
    /// <summary>
    /// Generic wander state
    /// </summary>
    public class WanderState : CreatureState
    {
        private Vector2 _target;
        private Vector2 _direction;
        private bool _hasReachTarget;
        [SerializeField]
        private float _sightMultiplayer = 1.5f;
        [SerializeField]
        private float _minGiveUpTime = 2f;
        [SerializeField]
        private float _maxGiveUpTime = 5f;
        private TimerComponent _giveUpTimer;

        public override void Ready()
        {
            base.Ready();
            _giveUpTimer = GetComponent<TimerComponent>();
        }
        public override void Enter()
        {
            _target = GetRandomTarget();
            //print("Target: " + _target);
            _giveUpTimer.StartTimer(Random.Range(_minGiveUpTime, _maxGiveUpTime));
            _giveUpTimer.OnTimeFinished += OnTimerTimeOut;
            _creature.OnResourceNear += OnResourceNear;
            _creature.OnCreatureNear += OnCreatureNear;
            _creature.OnCreatureSight += OnCreatureSight;
            _creature.OnResourceSight += OnResourceSight;
            _hasReachTarget = false;
        }

        public override void Exit()
        {
            _giveUpTimer.OnTimeFinished -= OnTimerTimeOut;
            _creature.OnResourceNear -= OnResourceNear;
            _creature.OnCreatureNear -= OnCreatureNear;
            _creature.OnCreatureSight -= OnCreatureSight;
            _creature.OnResourceSight -= OnResourceSight;
        }
        private void OnResourceSight(IConsumable[] obj)
        {
            if(!IsActive) return;
            if(obj.Length == 0) return;
            if (!_creature.IsHungry && !_creature.IsThirsty) return;
            //Go to closest resource
            StateMachine.ChangeState("GoToResourceState");
        }

        private void OnCreatureSight(BaseCreature[] obj)
        {
            if(!IsActive) return;
            if(obj.Length == 0) return;
            if(_creature.HasJustMated) return;
            if(_creature.IsSleepy) return;
            if (_creature.IsHungry || _creature.IsThirsty) return;
            StateMachine.ChangeState("GoToMateState");
        }

        private void OnCreatureNear(BaseCreature creature)
        {
            if(!IsActive) return;
            if (_creature.HasJustMated || creature.HasJustMated) return;
            if(_creature.Stats.Sex == creature.Stats.Sex) return;
        }

        private void OnResourceNear(IConsumable consumable)
        {
            if(!IsActive) return;
            if (!_creature.IsHungry && !_creature.IsThirsty) return;
        }
       
        public override void Process()
        {
            if (_hasReachTarget)
            {
                _target = GetRandomTarget();
                //print("Target: " + _target);
                _hasReachTarget = false;
                _giveUpTimer.StartTimer(Random.Range(_minGiveUpTime, _maxGiveUpTime));
                return;
            }
            if(Vector2.Distance(_creature.transform.position, _target) < 0.1f)
            {
                _hasReachTarget = true;
                //print("I have reached my target!");
                return;
            }
            _direction = (_target - (Vector2)_creature.transform.position);
            _creature.Move(_direction.normalized);
        }
        private void OnTimerTimeOut()
        {
            if (!IsActive) return;
            _hasReachTarget = true;
            print("I give up on this target, I will look for a new one");
        }
        private Vector2 GetRandomTarget()
        {
            bool coinFlip = GameManager.Instance.CoinFlip();
            Vector2 target = Vector2.zero;
            //Get a target on all directions
            if (coinFlip || (_direction == Vector2.zero || _direction.sqrMagnitude < 0.01f))
            {
                target = new Vector2(Random.Range(-_creature.Stats.SightRange * _sightMultiplayer, _creature.Stats.SightRange * _sightMultiplayer), Random.Range(-_creature.Stats.SightRange * _sightMultiplayer, _creature.Stats.SightRange * _sightMultiplayer));
                target += (Vector2)_creature.transform.position;
                return target;
            }
            //Get a target in the direction the creature was previously facing
            target += (_direction.normalized * _creature.Stats.SightRange) + (Vector2)_creature.transform.position;
            return target;
        }
    }
}
