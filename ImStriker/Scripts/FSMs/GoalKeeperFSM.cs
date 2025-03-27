using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Dive.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.IgnoreShot.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.InterceptShot.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.TendGoal.MainState;
using RobustFSM.Base;

namespace Assets.ImStriker.Scripts.FSMs
{
    public class GoalKeeperFSM : BFSM
    {
        public GoalKeeper Owner { get; set; }

        public override void AddStates()
        {
            AddState<PunchBallMainState>();
            AddState<IdleMainState>();
            AddState<IgnoreShotMainState>();
            AddState<InterceptShotMainState>();
            AddState<TendGoalMainState>();

            SetInitialState<IdleMainState>();
        }

        private void Awake()
        {
            Owner = GetComponent<GoalKeeper>();
        }
    }
}
