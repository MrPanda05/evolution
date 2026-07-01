using Evolution.Neural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolution.Neural
{
    public class Layer
    {
        public Layer(int width)
        {
            for (int i = 0; i < width; i++)
            {
                Nodes.Add(new Neuron((float input) =>
                {
                    // Mathematical sigmoid function
                    return ((10 / (1 + MathF.Pow(MathF.E, -1 * input))) - 5);
                }));
            }
        }

        public List<Neuron> Nodes { get; set; } = new List<Neuron>();

        public void CalcLayer()
        {
            foreach (var node in Nodes)
            {
                node.CalcNeuron();
            }
        }

        public void LinkLayer(Layer lastLayer)
        {
            foreach (var lastNode in lastLayer.Nodes)
            {
                foreach (var node in Nodes)
                {
                    lastNode.AddInputNeurons(node);
                }
            }
        }
    }
}
