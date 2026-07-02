using Evolution.Commons.Components;
using Evolution.Creatures;
using System;
using UnityEngine;

namespace Evolution.Resources
{
    [RequireComponent(typeof(TimerComponent))]
    public class Fruit : MonoBehaviour, IConsumable
    {
        [SerializeField]
        private int _minRangeFruit, _maxRangeFruit;
        private int _maxFruits = 10;
        private int _currentFruits = 0;
        public event Action OnCosume;
        [SerializeField]
        private TimerComponent _regrowTimer;

        private int _frameCounter = 0;
        private void Awake()
        {
            _regrowTimer = GetComponent<TimerComponent>();
        }
        private void OnEnable()
        {
            _regrowTimer.OnTimeFinished += Regrow;
        }
        private void OnDisable()
        {
            _regrowTimer.OnTimeFinished -= Regrow;
        }
        private void Start()
        {
            _maxFruits = UnityEngine.Random.Range(_minRangeFruit, _maxRangeFruit);
            _currentFruits = _maxFruits;
            float radius = UnityEngine.Random.Range(4f, 10f);
            transform.localScale = new Vector3(radius, radius);
            _regrowTimer.StartTimer(UnityEngine.Random.Range(5f, 20f));
        }
        public void Consume()
        {
            throw new NotImplementedException();
        }

        public void Consume(BaseCreature creature)
        {
            if (_currentFruits <= 0) return;
            print("Ate fruit");
            creature.HungerComponent.IncreaseHunger(creature.HungerComponent.MaxHunger);
            _currentFruits--;

        }
        public void Regrow()
        {
            if(_currentFruits > _maxFruits)
            {
                _currentFruits = _maxFruits;
                return;
            }
            _currentFruits++;
        }
    }
}
