using UnityEngine;
using TMPro;
using System;
namespace Evolution.Creatures
{
    public class CreaturesStatsUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _healthUI, _energyUI, _hungerUI, _thirstUI, _sexUI, _speedUI, _stateUI;

        [SerializeField]
        private BaseCreature _myCreature;

        private void Start()
        {
            _sexUI.text = "Sex: " + (_myCreature.IsFemale ? "F" : "M");
            _speedUI.text = $"{_myCreature.Stats.Speed}";
            _stateUI.text = _myCreature.Fsm.CurrentState.StateName;
        }

        private void OnEnable()
        {
            _myCreature.HealthComponent.OnHealthChanged += ChangeHealthUI;
            _myCreature.HungerComponent.OnHungerChanged += ChangeHungerUI;
            _myCreature.ThirstComponent.OnThirstChanged += ChangeThirstUI;
            _myCreature.EnergyComponent.OnEnergyChanged += ChangeEnergyUI;
            _myCreature.Fsm.OnStateChanged += ChangeStateUI;
        }

        private void ChangeStateUI()
        {
            _stateUI.text = _myCreature.Fsm.CurrentState.StateName;
        }

        private void OnDisable()
        {
            _myCreature.HealthComponent.OnHealthChanged -= ChangeHealthUI;
            _myCreature.HungerComponent.OnHungerChanged -= ChangeHungerUI;
            _myCreature.ThirstComponent.OnThirstChanged -= ChangeThirstUI;
            _myCreature.EnergyComponent.OnEnergyChanged -= ChangeEnergyUI;
            _myCreature.Fsm.OnStateChanged -= ChangeStateUI;
        }
        private void ChangeHealthUI()
        {
            _healthUI.text = $"Health:{Mathf.FloorToInt(_myCreature.HealthComponent.Health)}/{Mathf.FloorToInt(_myCreature.HealthComponent.MaxHealth)}";
        }
        private void ChangeHungerUI()
        {
            _hungerUI.text = $"Hunger:{Mathf.FloorToInt(_myCreature.HungerComponent.Hunger)}/{Mathf.FloorToInt(_myCreature.HungerComponent.MaxHunger)}";
        }
        private void ChangeThirstUI()
        {
            _thirstUI.text = $"Thirst:{Mathf.FloorToInt(_myCreature.ThirstComponent.Thirst)}/{Mathf.FloorToInt(_myCreature.ThirstComponent.MaxThirst)}";
        }
        private void ChangeEnergyUI()
        {
            _energyUI.text = $"Energy:{Mathf.FloorToInt(_myCreature.EnergyComponent.Energy)}/{Mathf.FloorToInt(_myCreature.EnergyComponent.MaxEnergy)}";
        }
    }
}
