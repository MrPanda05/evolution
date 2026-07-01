using Evolution.Commons.Components;
using Evolution.Creatures.States;
using UnityEngine;

namespace Evolution.Creatures.States.Generics
{
    public class SleepingState : CreatureState
    {
        private TimerComponent _sleepTimer;
        public override void Ready()
        {
            base.Ready();
            _sleepTimer = GetComponent<TimerComponent>();
        }
        public override void Enter()
        {
            _creature.Sleep();
            print("Sleeping");
            _sleepTimer.StartTimer();
            _sleepTimer.OnTimeFinished += OnSleepTimeFinished;
        }
        private void OnSleepTimeFinished()
        {
            StateMachine.ChangeState("WanderState");
        }
        public override void Exit()
        {
            _creature.WakeUp();
            _sleepTimer.OnTimeFinished -= OnSleepTimeFinished;
        }
    }
}
