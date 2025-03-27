using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.FSMs;
using RobustFSM.Base;

namespace Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.SubStates
{
    public class IdleWithNoBall : BState
    {
        public override void Enter()
        {
            base.Enter();

            //register to goalkeeper events
            Owner.OnHasBall += Instance_OnHasBall;

            //set the animator
            Owner.Animator.SetBool("HasBall", false);
        }

        public override void Exit()
        {
            base.Exit();

            //deregister to goalkeeper events
            Owner.OnHasBall -= Instance_OnHasBall;
        }

        private void Instance_OnHasBall()
        {
            Machine.ChangeState<IdleWithBall>();
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
