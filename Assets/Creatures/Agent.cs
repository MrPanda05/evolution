using Evolution.Neural;
using UnityEngine;

namespace Evolution.Creatures
{
    public class Agent : MonoBehaviour
    {
        public Brain Brain { get; private set; }
        private int[] _networkStructure = { 3, 4, 4, 2 };

        public float Fitness { get; protected set; } = 0f;
        private void Awake()
        {
            Brain = new Brain(_networkStructure);
        }
        public void SetBrain(Brain inheritedBrain)
        {
            Brain = new Brain(inheritedBrain);
        }
        public virtual void Move(Vector2 direction)
        {
            throw new System.NotImplementedException();
        }
    }
}
