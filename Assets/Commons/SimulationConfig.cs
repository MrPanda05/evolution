using Evolution.Simulation;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evolution.Commons
{
    public class SimulationConfig : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private TMP_InputField _population, _resource, _radius;
        [SerializeField]
        private SimulationStarter _simulationStarter;
        [SerializeField]
        private GameObject _canvas, _counter, _reset;


        public void StartSimulation()
        {
            int population = 0;
            int ressource = 0;
            int radius = 0;
            if(Int32.TryParse(_population.text, out population) && Int32.TryParse(_resource.text, out ressource) && Int32.TryParse(_radius.text, out radius))
            {
                print("Population " + population + " resource " + ressource + " radius " + radius);
                _simulationStarter.StartSimulation(population, ressource, radius);
                _canvas.SetActive(false);
                _counter.SetActive(true);
                _reset.SetActive(true);
            }
        }
    }
}
