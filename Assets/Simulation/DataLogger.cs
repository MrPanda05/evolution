using Evolution.Creatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Evolution
{
    /// <summary>
    /// Singleton that periodically samples the creature population and writes
    /// the results to a CSV file for later analysis (Excel, Sheets, Python, etc).
    /// Drop this on an empty GameObject in your scene.
    /// </summary>
    public class DataLogger : MonoBehaviour
    {
        public static DataLogger Instance { get; private set; }

        [Header("Sampling")]
        [Tooltip("How often (in seconds) to take a population snapshot")]
        [SerializeField] private float _sampleIntervalSeconds = 5f;

        [Header("Export")]
        [SerializeField] private string _fileName = "evolution_log.csv";
        [Tooltip("If true, writes to disk every sample instead of only on demand/quit")]
        [SerializeField] private bool _writeIncrementally = true;

        private float _timer;
        private int _births;
        private int _deaths;
        private bool _headerWritten;
        private string _filePath;

        private static readonly string[] Header =
        {
            "Time", "Population", "Births", "Deaths",
            "AvgHealth", "AvgSpeed", "AvgAtractivess", "AvgInteligence",
            "AvgSightRange", "AvgHungerDrainRate", "AvgThirstDrainRate", "AvgEnergyDrainRate",
            "MaleCount", "FemaleCount"
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _filePath = Path.Combine(Application.persistentDataPath, _fileName);
        }

        private void OnEnable()
        {
            CreatureRegistry.CreatureBorn += OnCreatureBorn;
            CreatureRegistry.CreatureDied += OnCreatureDied;
        }

        private void OnDisable()
        {
            CreatureRegistry.CreatureBorn -= OnCreatureBorn;
            CreatureRegistry.CreatureDied -= OnCreatureDied;
        }

        private void OnCreatureBorn(BaseCreature c) => _births++;
        private void OnCreatureDied(BaseCreature c) => _deaths++;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _sampleIntervalSeconds) return;

            _timer = 0f;
            Sample();
        }

        private void Sample()
        {
            var creatures = CreatureRegistry.All;
            int population = creatures.Count;

            float avgHealth = 0, avgSpeed = 0, avgAtractivess = 0, avgInteligence = 0;
            float avgSightRange = 0, avgHungerDrain = 0, avgThirstDrain = 0, avgEnergyDrain = 0;
            int males = 0, females = 0;

            if (population > 0)
            {
                foreach (var c in creatures)
                {
                    var s = c.Stats;
                    avgHealth += s.MaxHealth;
                    avgSpeed += s.Speed;
                    avgAtractivess += s.Atractivess;
                    avgInteligence += s.Inteligence;
                    avgSightRange += s.SightRange;
                    avgHungerDrain += s.HungerDrainRate;
                    avgThirstDrain += s.ThirstDrainRate;
                    avgEnergyDrain += s.EnergyDrainRate;
                    if (s.Sex) females++; else males++;
                }
                avgHealth /= population;
                avgSpeed /= population;
                avgAtractivess /= population;
                avgInteligence /= population;
                avgSightRange /= population;
                avgHungerDrain /= population;
                avgThirstDrain /= population;
                avgEnergyDrain /= population;
            }

            var row = new[]
            {
                Time.time.ToString("F1"),
                population.ToString(),
                _births.ToString(),
                _deaths.ToString(),
                avgHealth.ToString("F2"),
                avgSpeed.ToString("F2"),
                avgAtractivess.ToString("F2"),
                avgInteligence.ToString("F2"),
                avgSightRange.ToString("F2"),
                avgHungerDrain.ToString("F3"),
                avgThirstDrain.ToString("F3"),
                avgEnergyDrain.ToString("F3"),
                males.ToString(),
                females.ToString()
            };

            if (_writeIncrementally)
            {
                AppendRow(row);
            }
        }

        private void AppendRow(string[] row)
        {
            try
            {
                using (var writer = new StreamWriter(_filePath, append: true))
                {
                    if (!_headerWritten && new FileInfo(_filePath).Length == 0)
                    {
                        writer.WriteLine(string.Join(",", Header));
                    }
                    _headerWritten = true;
                    writer.WriteLine(string.Join(",", row));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLogger] Failed to write CSV: {e.Message}");
            }
        }

        /// <summary>
        /// Call this manually (e.g. from a debug button) to force a snapshot right now.
        /// </summary>
        [ContextMenu("Force Sample Now")]
        public void ForceSample() => Sample();

        private void OnApplicationQuit()
        {
            Debug.Log($"[DataLogger] Log written to: {_filePath}");
        }
    }

    /// <summary>
    /// Static registry of all living creatures. Lets systems (like DataLogger)
    /// observe population changes without polling FindObjectsOfType every frame.
    /// Call CreatureRegistry.Register(this) in BaseCreature.Awake(),
    /// and CreatureRegistry.Unregister(this) in HandleDeath().
    /// </summary>
    public static class CreatureRegistry
    {
        public static readonly List<Evolution.Creatures.BaseCreature> All = new List<Evolution.Creatures.BaseCreature>();
        public static event Action<Evolution.Creatures.BaseCreature> CreatureBorn;
        public static event Action<Evolution.Creatures.BaseCreature> CreatureDied;

        public static void Register(Evolution.Creatures.BaseCreature c)
        {
            All.Add(c);
            CreatureBorn?.Invoke(c);
        }

        public static void Unregister(Evolution.Creatures.BaseCreature c)
        {
            All.Remove(c);
            CreatureDied?.Invoke(c);
        }
    }
}