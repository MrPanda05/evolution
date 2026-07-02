using Evolution.Commons.Components;
using Evolution.Commons.Managers;
using Evolution.Commons.StateMachine;
using Evolution.Resources;
using System;
using UnityEngine;

namespace Evolution.Creatures
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(EnergyComponent))]
    [RequireComponent(typeof(ThirstComponent))]
    [RequireComponent(typeof(HungerComponent))]
    public class BaseCreature : MonoBehaviour, ICreature
    {
        [Header("Creature stats")]
        [field:SerializeField]
        public CreatureStats Stats { get; protected set; }

        [Header("Propreties")]
        [field:SerializeField]
        public bool IsSpawnedIn { get; protected set; } = true;
        [Header("Components")]
        [field:SerializeField]
        public HealthComponent HealthComponent { get; protected set; }
        [field:SerializeField]
        public EnergyComponent EnergyComponent { get; protected set; }
        [field:SerializeField]
        public HungerComponent HungerComponent { get; protected set; }
        [field:SerializeField]
        public ThirstComponent ThirstComponent { get; protected set; }
        [field:SerializeField]
        public SightComponent SightComponent { get; protected set; }
        [field:SerializeField]
        public FiniteStateMachine Fsm { get; protected set; }
        protected SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Canvas _canvas;
        [Header("Self prefab")]
        [SerializeField]
        protected BaseCreature _selfPrefab;

        public int CurrentPopulation { get; protected set; }


        public bool IsFemale => Stats.Sex;
        public bool IsMale => !Stats.Sex;

        public bool IsHungry => HungerComponent.Hunger <= Stats.HungerTreshHold;
        public bool IsThirsty => ThirstComponent.Thirst <= Stats.ThirstTreshHold;
        public bool IsSleepy => EnergyComponent.Energy <= Stats.EnergyTreshHold;

        public bool IsDead { get; protected set; }
        public bool HasJustMated {  get; protected set; }
        public bool IsSleeping { get; protected set; }

        [SerializeField]
        protected Collider2D[] _sightData;
        public Collider2D[] SightData => _sightData;
        [SerializeField]
        protected float _timeAlive;

        public Action<IConsumable> OnResourceNear;
        public Action<IConsumable[]> OnResourceSight;
        public Action<BaseCreature> OnCreatureNear;
        public Action<BaseCreature[]> OnCreatureSight;

        private void Awake()
        {
            Stats = Instantiate(Stats);
            HealthComponent = GetComponent<HealthComponent>();
            EnergyComponent = GetComponent<EnergyComponent>();
            HungerComponent = GetComponent<HungerComponent>();
            ThirstComponent = GetComponent<ThirstComponent>();
            SightComponent = GetComponent<SightComponent>();
            SightComponent.SetSightRadius(Stats.SightRange);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _canvas.worldCamera = Camera.main;
            _canvas.enabled = false;
            CreatureRegistry.Register(this);
        }
        protected virtual void HandleDeath()
        {

        }
        public void SetPopulation(int value)
        {
            CurrentPopulation = value;
        }
        private void Start()
        {
            SetUpStats();
            GameManager.Instance.OnPause += DisableSimulation;
            GameManager.Instance.OnUnPause += EnableSimulation;
        }
        private void OnDestroy()
        {
            GameManager.Instance.OnPause -= DisableSimulation;
            GameManager.Instance.OnUnPause -= EnableSimulation;
        }
        public virtual void Sleep() { }
        public virtual void WakeUp() { }
        protected void DisableSimulation()
        {
            enabled = false;
            Fsm.enabled = false;
        }
        protected void EnableSimulation()
        {
            enabled = true;
            Fsm.enabled = true;
        }
        public virtual void SetUpStats()
        {
            HealthComponent.Initialize(Stats.MaxHealth, Stats.MaxHealth);
            EnergyComponent.Initialize(Stats.MaxEnergy, Stats.MaxEnergy);
            HungerComponent.Initialize(Stats.MaxHunger, Stats.MaxHunger);
            ThirstComponent.Initialize(Stats.MaxThirst, Stats.MaxThirst);
        }
        public virtual void Move(Vector2 direction)
        {
            throw new NotImplementedException();
        }

        public virtual bool Reproduce(ICreature creature1, ICreature creature2, int creature1Population, int creature2Population)
        {

            ICreature mom = null;
            ICreature dad = null;
            if (creature1.Stats.Sex == creature2.Stats.Sex) return false;
            if (creature1.Stats.Sex)
            {
                mom = creature1;
                dad = creature2;
            }
            else
            {
                mom = creature2;
                dad = creature1;
            }
            var colorDiff = Vector4.Distance(mom.Stats.Color, dad.Stats.Color);
            int sameColorBost = 0;
            if(colorDiff <= 0.2f)
            {
                sameColorBost = 20;
            }
            else if(colorDiff > 0.2f && colorDiff <= 0.8)
            {
                sameColorBost = 0;
            }else
            {
                sameColorBost = -15;
            }
                bool dadWants = GameManager.Instance.RollDice((int)mom.Stats.Atractivess + sameColorBost);
            bool momWants = GameManager.Instance.RollDice((int)dad.Stats.Atractivess + sameColorBost);
            //Second Chance for dad
            if (!momWants && dadWants)
            {
                momWants = GameManager.Instance.RollDice((int)dad.Stats.Inteligence);
            }
            //Second chance for mom
            if (momWants && !dadWants)
            {
                dadWants = GameManager.Instance.RollDice((int)mom.Stats.Inteligence);
            }
            if (!momWants || !dadWants)
            {
                print("Mating failed");
                return false;
            }
            print("SEXO");
            CreatureStats offSpringStats = ScriptableObject.CreateInstance<CreatureStats>();
            offSpringStats.MaxEnergy = (mom.Stats.MaxEnergy + dad.Stats.MaxEnergy) /2;
            offSpringStats.MaxHealth = (mom.Stats.MaxHealth + dad.Stats.MaxHealth) /2;
            offSpringStats.MaxHunger = (mom.Stats.MaxHunger + dad.Stats.MaxHunger) /2;
            offSpringStats.MaxThirst = (mom.Stats.MaxThirst + dad.Stats.MaxThirst) /2;

            offSpringStats.Atractivess = (mom.Stats.Atractivess + dad.Stats.Atractivess) /2;
            offSpringStats.Inteligence = (mom.Stats.Inteligence + dad.Stats.Inteligence) /2;
            offSpringStats.SightRange = (mom.Stats.SightRange + dad.Stats.SightRange) /2;

            offSpringStats.HungerDrainRate = (mom.Stats.HungerDrainRate + dad.Stats.HungerDrainRate) /2;
            offSpringStats.ThirstDrainRate = (mom.Stats.ThirstDrainRate + dad.Stats.ThirstDrainRate) /2;
            offSpringStats.EnergyDrainRate = (mom.Stats.EnergyDrainRate + dad.Stats.EnergyDrainRate) /2;

            offSpringStats.HungerTreshHold = (mom.Stats.HungerTreshHold + dad.Stats.HungerTreshHold) /2;
            offSpringStats.ThirstTreshHold = (mom.Stats.ThirstTreshHold + dad.Stats.ThirstTreshHold) /2;
            offSpringStats.EnergyTreshHold = (mom.Stats.EnergyTreshHold + dad.Stats.EnergyTreshHold) /2;

            offSpringStats.SightRange = (mom.Stats.SightRange + dad.Stats.SightRange) /2;

            offSpringStats.Speed = (mom.Stats.Speed + dad.Stats.Speed) /2;

            offSpringStats.MaxAliveTime = (mom.Stats.MaxAliveTime + dad.Stats.MaxAliveTime) /2;

            MutateChild(ref offSpringStats);

            offSpringStats.Color = new Color((mom.Stats.Color.r + dad.Stats.Color.r)/2, (mom.Stats.Color.g + dad.Stats.Color.g) / 2, (mom.Stats.Color.b + dad.Stats.Color.b) / 2);
            var child = Instantiate<BaseCreature>(_selfPrefab);
            child.SetPopulation((Mathf.Max(creature2Population, creature1Population) + 1));
            child.Stats = offSpringStats;
            child.transform.position = transform.position;
            child.SetColor(Stats.Color);
            return true;
        }
        public virtual void SetHasReproduced(bool value)
        {
            HasJustMated = value;
        }
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
        private void OnMouseEnter()
        {
            _canvas.enabled = true;
        }
        private void OnMouseExit()
        {
            _canvas.enabled = false;
        }
        //Used for the first generation
        public void RandomizeAllStats()
        {
            Stats.RandomSex();
            Stats.MaxHealth =  Stats.MaxHealth + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f,5f) : 0);
            Stats.MaxEnergy =  Stats.MaxEnergy + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f,5f) : 0);
            Stats.MaxHunger =  Stats.MaxHunger + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f,5f) : 0);
            Stats.MaxThirst =  Stats.MaxThirst + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f,5f) : 0);

            Stats.Atractivess = Stats.Atractivess + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            Stats.Inteligence = Stats.Inteligence + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);

            Stats.SightRange = Stats.SightRange + (GameManager.Instance.RollDice(35) ? UnityEngine.Random.Range(-5f, 10f) : 0);

            Stats.HungerDrainRate = Stats.HungerDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            Stats.ThirstDrainRate = Stats.ThirstDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            Stats.EnergyDrainRate = Stats.EnergyDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);

            Stats.HungerTreshHold = Stats.HungerTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            Stats.ThirstTreshHold = Stats.ThirstTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            Stats.EnergyTreshHold = Stats.EnergyTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);

            Stats.Color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            //Speed is diffent
            Stats.Speed = Stats.Speed + (GameManager.Instance.RollDice(70) ? UnityEngine.Random.Range(-1f, 5f) : 0);

            Stats.MaxAliveTime = Stats.MaxAliveTime + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 10f) : 0);
            _spriteRenderer.color = Stats.Color;
        }
        public void MutateChild(ref CreatureStats stats)
        {
            stats.RandomSex();
            stats.MaxHealth = stats.MaxHealth + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            stats.MaxEnergy = stats.MaxEnergy + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            stats.MaxHunger = stats.MaxHunger + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            stats.MaxThirst = stats.MaxThirst + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);

            stats.Atractivess = stats.Atractivess + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            stats.Inteligence = stats.Inteligence + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 5f) : 0);
            stats.SightRange = stats.SightRange + (GameManager.Instance.RollDice(20) ? UnityEngine.Random.Range(-5f, 10f) : 0);

            stats.HungerDrainRate = stats.HungerDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            stats.ThirstDrainRate = stats.ThirstDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            stats.EnergyDrainRate = stats.EnergyDrainRate + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);

            stats.HungerTreshHold = stats.HungerTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            stats.ThirstTreshHold = stats.ThirstTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);
            stats.EnergyTreshHold = stats.EnergyTreshHold + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-0.02f, 0.8f) : 0);

            stats.Speed = stats.Speed + (GameManager.Instance.RollDice(25) ? UnityEngine.Random.Range(-0.02f, 10f) : 0);

            stats.MaxAliveTime = stats.MaxAliveTime + (GameManager.Instance.RollDice(10) ? UnityEngine.Random.Range(-5f, 10f) : 0);
        }

        public virtual void Reproduce()
        {
            bool chanceToMate = false;
            if(Stats.Atractivess > 50)
            {
                chanceToMate = GameManager.Instance.RollDice(10);
            }
        }
    }
}
