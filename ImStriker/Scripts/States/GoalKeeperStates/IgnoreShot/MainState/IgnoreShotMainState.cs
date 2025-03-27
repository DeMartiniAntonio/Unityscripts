using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.FSMs;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState;
using RobustFSM.Base;

namespace Assets.ImStriker.Scripts.States.GoalKeeperStates.IgnoreShot.MainState
{
    public class IgnoreShotMainState : BState
    {
        public override void Enter()
        {
            base.Enter();

            //run the IdleMainState enter function
            Machine.GetState<IdleMainState>().Enter();
        }

        public override void Exit()
        {
            base.Exit();

            //run the IdleMainState exit function
            Machine.GetState<IdleMainState>().Exit();
        }

        GoalKeeper Owner
        {
            get
            {
                return ((GoalKeeperFSM)SuperMachine).Owner;
            }
        }
    }
}
