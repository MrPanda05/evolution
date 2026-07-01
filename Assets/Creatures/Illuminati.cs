using Evolution.Commons.Components;
using Evolution.Commons.Managers;
using Evolution.Commons.StateMachine;
using Evolution.Neural;
using Evolution.Resources;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Evolution.Creatures
{
    public class Illuminati : BaseCreature
    {
        private int _frameCounter = 0;
        
        private Vector3 _target;
        private bool _hasReachTarget;
        private bool _isWandering;
        private bool _isSearching;
        private bool _isGoingToResource;
        [SerializeField]
        private TimerComponent _hungerTimer, _energyTimer, _thirstTimer, _mateTimer;

        private void Start()
        {
            SetUpStats();
            GameManager.Instance.OnPause += DisableSimulation;
            GameManager.Instance.OnUnPause += EnableSimulation;
            if (IsFemale)
            {
                _spriteRenderer.flipY = true;
            }
        }
        private void OnEnable()
        {
            HealthComponent.OnDeath += HandleDeath;
            _hungerTimer.OnTimeFinished += HandleHungerTimerFinished;
            _energyTimer.OnTimeFinished += HandleEnergyTimerFinished;
            _thirstTimer.OnTimeFinished += HandleThirstTimerFinished;
            _mateTimer.OnTimeFinished += HandleMateTimerFinished;
        }

        private void HandleMateTimerFinished()
        {
            HasJustMated = false;
        }

        private void HandleThirstTimerFinished()
        {
            HealthComponent.TakeDamage(3f);
        }

        private void HandleEnergyTimerFinished()
        {
            HealthComponent.TakeDamage(0.5f);
        }

        private void HandleHungerTimerFinished()
        {
            HealthComponent.TakeDamage(1.5f);
        }

        private void OnDisable()
        {
            HealthComponent.OnDeath -= HandleDeath;
            _hungerTimer.OnTimeFinished -= HandleHungerTimerFinished;
            _energyTimer.OnTimeFinished -= HandleEnergyTimerFinished;
            _thirstTimer.OnTimeFinished -= HandleThirstTimerFinished;
            _mateTimer.OnTimeFinished -= HandleMateTimerFinished;
        }
        
        public override void WakeUp()
        {
            IsSleeping = false;
            EnergyComponent.IncreaseEnergy(EnergyComponent.MaxEnergy);
        }

        protected override void HandleDeath()
        {
            IsDead = true;
            print("I died");
            CreatureRegistry.Unregister(this);
            Destroy(this.gameObject, 5);
        }
        private void FixedUpdate()
        {
            if (IsDead) return;
            _frameCounter++;
            if (_frameCounter > 45)
            {
                _frameCounter = 0;
                HungerComponent.DecreaseHunger(Stats.HungerDrainRate);
                ThirstComponent.DecreaseThirst(Stats.ThirstDrainRate);
                EnergyComponent.ReduceEnergy(Stats.EnergyDrainRate);
                //Start damage timer for each crucial stat
                if (IsThirsty && !_thirstTimer.IsRunning)
                {
                    _thirstTimer.StartTimer(Stats.ThirstDrainRate * 2);
                }
                if (IsHungry && !_hungerTimer.IsRunning)
                {
                    _hungerTimer.StartTimer(Stats.HungerDrainRate * 2);
                }
                if (IsSleepy && !_energyTimer.IsRunning && IsSleeping)
                {
                    _energyTimer.StartTimer(Stats.EnergyDrainRate * 2);
                }
                if (!IsSleeping)
                {
                    _sightData = SightComponent.FindAllInLayer(Stats.SightLayer);
                    List<IConsumable> resourcesInSight = new List<IConsumable>();
                    List<BaseCreature> creaturesInSight = new List<BaseCreature>();
                    foreach (var item in _sightData)
                    {
                        if (item == null) continue;
                        if (item.TryGetComponent<IConsumable>(out IConsumable consumable))
                            resourcesInSight.Add(consumable);
                        if (item.TryGetComponent<BaseCreature>(out BaseCreature creature))
                        {
                            if (creature == this) continue;
                            creaturesInSight.Add(creature);
                        }
                    }
                    OnResourceSight?.Invoke(resourcesInSight.ToArray());
                    OnCreatureSight?.Invoke(creaturesInSight.ToArray());
                }
            }
        }
        public override void SetHasReproduced(bool value)
        {
            base.SetHasReproduced(value);
            _mateTimer.StartTimer();
        }
        private void Wander()
        {
            Vector3 direction;
            if (_hasReachTarget || _target == Vector3.zero)
            {
                float searchRadius = Stats.SightRange;
                _target = new Vector3(UnityEngine.Random.Range(-searchRadius, searchRadius), UnityEngine.Random.Range(-searchRadius, searchRadius), 0);
                _target += transform.position;
                _hasReachTarget = false;
                return;
            }
            direction = (_target - transform.position).normalized;
            transform.position += direction * Stats.Speed * Time.deltaTime;
            if (transform.position == _target || Vector3.Distance(transform.position, _target) < 1)
            {
                _hasReachTarget = true;
            }
        }
        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.TryGetComponent<IConsumable>(out IConsumable consumable))
        //    {
        //        OnResourceNear?.Invoke(consumable);
        //    }
        //    if (collision.TryGetComponent<BaseCreature>(out BaseCreature creature))
        //    {
        //       OnCreatureNear?.Invoke(creature);
        //    }
        //}

        public override void Sleep()
        {
            IsSleeping = true;
            EnergyComponent.IncreaseEnergy(Stats.EnergyDrainRate * 2);
        }
        private void Update()
        {
            if (IsDead) return;
            _timeAlive += Time.deltaTime;
            if(_timeAlive > Stats.MaxAliveTime)
            {
                float age = _timeAlive / Stats.MaxAliveTime;

                float deathChance = Mathf.Pow(age, 5) * 0.01f;

                if (UnityEngine.Random.value < deathChance * Time.deltaTime)
                {
                    HealthComponent.TakeDamage(HealthComponent.MaxHealth);
                    print("Died of old age");
                }
            }
            if (IsSleeping) return;
            if (IsSleepy)
            {
                Fsm.ChangeState("SleepingState");
            }
            ////If tired or exausted, sleep
            //if (IsSleepy || EnergyComponent.Energy == 0)
            //{
            //    _isWandering = false;
            //    _isGoingToResource = false;
            //    Sleep();
            //    return;
            //}
            ////If there is nothing in sight, just wander randomly
            //if (_sightData.Length == 0)
            //{
            //    _isWandering = true;
            //    _isGoingToResource = false;
            //    Wander();
            //    return;
            //}
            ////If there are things I can see
            ////Separete into groups
            //List<BaseCreature> baseCreatures = new();
            //List<GameObject> consumables = new();
            //foreach (var item in _sightData)
            //{
            //    if (item == null) continue;
            //    if (item.TryGetComponent<IConsumable>(out IConsumable consumable))
            //        consumables.Add(item.gameObject);
            //    if (item.TryGetComponent<BaseCreature>(out BaseCreature creature))
            //    {
            //        if (creature == this) continue;
            //        baseCreatures.Add(creature);
            //    }
            //}
            ////Find the closest resource
            //if (IsHungry || IsThirsty)
            //{
            //    GameObject nearestWater = null;
            //    GameObject nearestFood = null;
            //    float distToWater = float.MaxValue;
            //    float distToFood = float.MaxValue;

            //    foreach (var item in consumables)
            //    {
            //        if (item.TryGetComponent<Water>(out Water water))
            //        {
            //            float d = Vector3.Distance(transform.position, water.transform.position);
            //            if (d < distToWater) { distToWater = d; nearestWater = item; }
            //        }
            //        else if (item.TryGetComponent<Fruit>(out Fruit fruit))
            //        {
            //            float d = Vector3.Distance(transform.position, fruit.transform.position);
            //            if (d < distToFood) { distToFood = d; nearestFood = item; }
            //        }
            //    }

            //    GameObject chosenTarget = null;
            //    if (IsHungry && IsThirsty)
            //    {
            //        if (nearestFood != null && nearestWater != null)
            //            chosenTarget = GameManager.Instance.CoinFlip() ? nearestFood : nearestWater;
            //        else
            //            chosenTarget = nearestFood != null ? nearestFood : nearestWater;
            //    }
            //    else if (IsHungry)
            //    {
            //        chosenTarget = nearestFood;
            //    }
            //    else if (IsThirsty)
            //    {
            //        chosenTarget = nearestWater;
            //    }

            //    if (chosenTarget != null)
            //    {
            //        _target = chosenTarget.transform.position;
            //        _isGoingToResource = true;
            //        _isWandering = false;
            //        MoveTowardsTarget();
            //        return;
            //    }
            //}
            ////Find mate
            //if (!IsHungry && !IsThirsty && !HasJustMated)
            //{
            //    BaseCreature nearestMate = null;
            //    float distToMate = float.MaxValue;
            //    foreach (var item in baseCreatures)
            //    {
            //        if (item.TryGetComponent<BaseCreature>(out BaseCreature creature))
            //        {
            //            float d = Vector3.Distance(transform.position, creature.transform.position);
            //            if ((d < distToMate))
            //            {
            //                distToMate = d;
            //                nearestMate = creature;
            //            }
            //        }
            //    }
            //    if (nearestMate != null)
            //    {
            //        _target = nearestMate.transform.position;
            //        _isGoingToResource = true;
            //        _isWandering = false;
            //        MoveTowardsTarget();
            //        return;
            //    }
            //}
            ////Go to target
            //if (_isGoingToResource)
            //{
            //    MoveTowardsTarget();
            //    return;
            //}
            ////Wander if I am not hungry, nor want to mate
            //_isWandering = true;
            //_isGoingToResource = false;
            //Wander();
        }
        public override void Move(Vector2 direction)
        {
            transform.position += (Vector3)direction * Stats.Speed * Time.deltaTime;
        }
    }
}
