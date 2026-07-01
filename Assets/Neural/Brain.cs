using System;
using UnityEngine;

namespace Evolution.Neural
{
    /// <summary>
    /// A simple feedforward neural network, built from scratch (no ML libraries).
    /// Used as a creature's "brain": takes sensory inputs and produces action outputs.
    ///
    /// Architecture: fully connected layers, configurable sizes, tanh activation.
    /// Since this brain is evolved (mutation/crossover) rather than trained via
    /// backpropagation, there's no need for gradients, loss functions, etc.
    /// We only need: Forward() to think, and ways to copy/mutate/crossover weights.
    /// </summary>
    [Serializable]
    public class Brain
    {
        // Number of neurons in each layer, e.g. {6, 8, 4} = 6 inputs, 1 hidden layer of 8, 4 outputs.
        public int[] layerSizes;

        // weights[i] is the weight matrix connecting layer i to layer i+1.
        // weights[i][neuronInNextLayer][neuronInThisLayer]
        public float[][][] weights;

        // biases[i] is the bias vector for layer i+1 (the layer the weights feed into).
        // biases[i][neuronInNextLayer]
        public float[][] biases;

        /// <summary>
        /// Creates a brain with random weights/biases for the given layer structure.
        /// Example: new Brain(new int[] {6, 8, 4}) -> 6 inputs, 8 hidden, 4 outputs.
        /// </summary>
        public Brain(int[] layerSizes)
        {
            this.layerSizes = layerSizes;
            int layerCount = layerSizes.Length - 1; // number of weight/bias layers

            weights = new float[layerCount][][];
            biases = new float[layerCount][];

            for (int i = 0; i < layerCount; i++)
            {
                int inSize = layerSizes[i];
                int outSize = layerSizes[i + 1];

                weights[i] = new float[outSize][];
                biases[i] = new float[outSize];

                for (int n = 0; n < outSize; n++)
                {
                    weights[i][n] = new float[inSize];
                    for (int w = 0; w < inSize; w++)
                    {
                        weights[i][n][w] = RandomWeight();
                    }
                    biases[i][n] = RandomWeight();
                }
            }
        }

        /// <summary>
        /// Creates a brain by deep-copying another brain's weights (used for reproduction).
        /// </summary>
        public Brain(Brain other)
        {
            layerSizes = (int[])other.layerSizes.Clone();
            int layerCount = other.weights.Length;

            weights = new float[layerCount][][];
            biases = new float[layerCount][];

            for (int i = 0; i < layerCount; i++)
            {
                int outSize = other.weights[i].Length;
                weights[i] = new float[outSize][];
                biases[i] = new float[outSize];

                for (int n = 0; n < outSize; n++)
                {
                    weights[i][n] = (float[])other.weights[i][n].Clone();
                    biases[i][n] = other.biases[i][n];
                }
            }
        }

        private static float RandomWeight()
        {
            // Random in [-1, 1]. Good enough starting range for a small evolved network.
            return UnityEngine.Random.Range(-1f, 1f);
        }

        /// <summary>
        /// Runs the network forward: inputs in, outputs out.
        /// inputs.Length must match layerSizes[0].
        /// </summary>
        public float[] Forward(float[] inputs)
        {
            if (inputs.Length != layerSizes[0])
            {
                Debug.LogError($"Brain expected {layerSizes[0]} inputs but got {inputs.Length}");
                return new float[layerSizes[layerSizes.Length - 1]];
            }

            float[] activations = inputs;

            for (int i = 0; i < weights.Length; i++)
            {
                int outSize = weights[i].Length;
                float[] next = new float[outSize];

                for (int n = 0; n < outSize; n++)
                {
                    float sum = biases[i][n];
                    float[] w = weights[i][n];
                    for (int k = 0; k < w.Length; k++)
                    {
                        sum += w[k] * activations[k];
                    }
                    next[n] = Tanh(sum);
                }

                activations = next;
            }

            return activations;
        }

        private static float Tanh(float x)
        {
            return (float)Math.Tanh(x);
        }

        /// <summary>
        /// Mutates this brain's weights/biases in place.
        /// mutationRate: probability that any individual weight gets nudged.
        /// mutationStrength: max magnitude of the random nudge applied.
        /// </summary>
        public void Mutate(float mutationRate, float mutationStrength)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int n = 0; n < weights[i].Length; n++)
                {
                    for (int w = 0; w < weights[i][n].Length; w++)
                    {
                        if (UnityEngine.Random.value < mutationRate)
                        {
                            weights[i][n][w] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
                        }
                    }

                    if (UnityEngine.Random.value < mutationRate)
                    {
                        biases[i][n] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new brain by mixing weights from two parents (sexual reproduction).
        /// For each weight, randomly picks the value from parentA or parentB (uniform crossover).
        /// Assumes both parents have identical layerSizes.
        /// </summary>
        public static Brain Crossover(Brain parentA, Brain parentB)
        {
            Brain child = new Brain(parentA.layerSizes);

            for (int i = 0; i < child.weights.Length; i++)
            {
                for (int n = 0; n < child.weights[i].Length; n++)
                {
                    for (int w = 0; w < child.weights[i][n].Length; w++)
                    {
                        child.weights[i][n][w] = UnityEngine.Random.value < 0.5f
                            ? parentA.weights[i][n][w]
                            : parentB.weights[i][n][w];
                    }

                    child.biases[i][n] = UnityEngine.Random.value < 0.5f
                        ? parentA.biases[i][n]
                        : parentB.biases[i][n];
                }
            }

            return child;
        }
    }

}
