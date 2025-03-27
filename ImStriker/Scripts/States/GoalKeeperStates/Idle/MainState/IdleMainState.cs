using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.FSMs;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.TendGoal.MainState;
using RobustFSM.Base;

namespace Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState
{
    public class IdleMainState : BState
    {

        public override void Enter()
        {
            base.Enter();

            //set the components
            Owner.Animator.SetTrigger("Idle");
            Owner.RPGMovement.SetSteeringOff();
        }

        public override void Execute()
        {
            base.Execute();
            //if the ball is within threatening distance then tend goal
            if (Owner.IsBallWithThreateningDistance())
                Machine.ChangeState<TendGoalMainState>();
        }

        public override void Exit()
        {
            base.Exit();

            //reset the animator
            Owner.Animator.ResetTrigger("Idle");
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
