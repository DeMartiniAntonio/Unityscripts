using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState;
using System;
using UnityEngine;

namespace Assets.ImStriker.Scripts.Others.EntitiesAnimationEvents
{
    public class GoalKeeperAnimationEvents : MonoBehaviour
    {
        public GoalKeeper Owner;

        public void GoToIdleState()
        {
            Owner.FSM.ChangeState<IdleMainState>();
        }

        public void GoToSleepState()
        {
            throw new NotImplementedException();
        }

        public void OnAnimatorIK(int layerIndex)
        {
            Owner.FSM.OnAnimatorIK(layerIndex);
        }

        private void OnAnimatorMove()
        {
            Owner.FSM.OnAnimatorMove();
        }
    }
}
