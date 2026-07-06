using Evolution.Simulation;
using TMPro;
using UnityEngine;

namespace Evolution.Commons
{
    public class PopulationCounter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _total, _males, _females;
        [SerializeField]
        private GameObject _counter;
        [SerializeField]
        private DataLogger _dataLogger;

        private void OnEnable()
        {
            _dataLogger.ForceSample();
        }
        private void Update()
        {
            _total.text = $"{_dataLogger.population}";
            _males.text = $"male: {_dataLogger.maleCount}";
            _females.text = $"female: {_dataLogger.femaleCount}";
        }
    }
}
