using Evolution.Commons.Managers;
using Evolution.Creatures;
using UnityEngine;

namespace Evolution.Simulation
{
    //Starts the simulation, first generation off all creatures, spawning resoruces so on
    public class SimulationStarter : MonoBehaviour
    {
        [SerializeField]
        private BaseCreature[] _creatures;
        [SerializeField]
        private GameObject[] _resources;

        [SerializeField]
        private float _spawnRadius = 100;
        [SerializeField]
        private int _maxPopulation = 100;
        [SerializeField]
        private int _maxResourcesPopulation = 100;

        private void SpawnCreatures()
        {
            foreach (var item in _creatures)
            {
                int count = 0;
                for (count = 0; count < _maxPopulation; count++)
                {
                    var instance = Instantiate(item);
                    instance.RandomizeAllStats();
                    instance.gameObject.transform.position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius+1), Random.Range(-_spawnRadius, _spawnRadius + 1), 0);
                }
            }
        }
        private void SpawnResources()
        {
            foreach (var item in _resources)
            {
                int count = 0;
                for (count = 0; count < _maxResourcesPopulation; count++)
                {
                    var instance = Instantiate(item);
                    instance.gameObject.transform.position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius + 1), Random.Range(-_spawnRadius, _spawnRadius + 1), 0);
                }
            }
        }
        public void StartSimulation(int population, int resource, int radius)
        {
            foreach (var item in _resources)
            {
                int count = 0;
                for (count = 0; count < population; count++)
                {
                    var instance = Instantiate(item);
                    instance.gameObject.transform.position = new Vector3(Random.Range(-radius, radius + 1), Random.Range(-radius, radius + 1), 0);
                }
            }
            foreach (var item in _creatures)
            {
                int count = 0;
                for (count = 0; count < population; count++)
                {
                    var instance = Instantiate(item);
                    instance.RandomizeAllStats();
                    instance.gameObject.transform.position = new Vector3(Random.Range(-radius, radius + 1), Random.Range(-radius, radius + 1), 0);
                }
            }
            GameManager.Instance.IsSimulationRunning = true;
        }

    }
}
