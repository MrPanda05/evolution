using Evolution.Commons.StateMachine;
using UnityEngine;

namespace Evolution.Commons.StateMachine
{
    /// <summary>
    /// Basic state
    /// </summary>
    public class State : MonoBehaviour
    {
        public string StateName => GetType().Name;
        public FiniteStateMachine StateMachine { get; private set; }
        public bool IsActive => StateMachine.CurrentState == this;

        public void SetStateMachine(FiniteStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        /// <summary>
        /// Same as awake
        /// </summary>
        public virtual void Ready() { }
        /// <summary>
        /// Same as start
        /// </summary>
        public virtual void OnStart() { }
        /// <summary>
        /// Called when entering the state
        /// </summary>
        public virtual void Enter()
        {
            print("Entering: " + StateName);
        }
        /// <summary>
        /// Called when exiting the state
        /// </summary>
        public virtual void Exit()
        {
            print("Exiting: " + StateName);
        }
        /// <summary>
        /// Called every frame, same as update
        /// </summary>
        public virtual void Process() { }
        /// <summary>
        /// Called every fixed update, same as fixed update
        /// </summary>
        public virtual void FixedProcess() { }
    }
}
