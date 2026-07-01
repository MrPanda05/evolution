using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evolution.Neural
{
    public class Neuron
    {
        public Dictionary<Neuron, float> InputNeurons { get; private set; } = new();
        public float Result { get; set; }
        public Func<float, float> Sigmoid {  get; set; }

        public void AddInputNeurons(Neuron neuron)
        {
            InputNeurons.Add(neuron, 0.00001f);
        }
        public void CalcNeuron()
        {
            foreach (var valuePair in InputNeurons)
            {
                var node = valuePair.Key;
                var weight = valuePair.Value;
                Result += node.Sigmoid(weight * node.Result);

                if (double.IsNaN(Result) || double.IsInfinity(Result) || double.IsNegativeInfinity(Result))
                {
                    throw new Exception("Bad result!");
                }
            }

            Result = Result / Math.Max(1, InputNeurons.Count);
        }

        public Neuron(Func<float, float> sigmoid, float result = 0)
        {
            Sigmoid = sigmoid;
            Result = result;
        }
    }
}
