using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.FSMs;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.IgnoreShot.MainState;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.InterceptShot.MainState;
using RobustFSM.Base;
using UnityEngine;

namespace Assets.ImStriker.Scripts.States.GoalKeeperStates.TendGoal.MainState
{
    public class TendGoalMainState : BState
    {
        float _timeSinceLastUpdate;
        Vector3 _steeringTarget;
        Vector3 _prevBallPosition;

        public override void Enter()
        {
            base.Enter();

            //set some data
            _prevBallPosition = 1000 * Vector3.one;
            _timeSinceLastUpdate = 0f;

            //set the rpg movement
            Owner.RPGMovement.SetSteeringOn();
            Owner.RPGMovement.Speed = Owner.TendGoalSpeed;

            //register to some events
            Owner.OnBallLaunched += Instance_OnBallLaunched;

            //set the animator
            Owner.Animator.SetTrigger("TendGoal");
        }

        public override void Execute()
        {
            base.Execute();

            //if the ball is not within threatening distance then idle
            if (!Owner.IsBallWithThreateningDistance())
                Machine.ChangeState<IdleMainState>();

            //get the entity positions
            Vector3 ballPosition = new(Owner.Ball.Position.x, 0f, Owner.Ball.Position.z);

            //set the look target
            Owner.RPGMovement.SetRotateFacePosition(ballPosition);

            //if I have exhausted my time then update the tend point
            if (_timeSinceLastUpdate <= 0f)
            {
                //do not continue if the ball didnt move
                if (_prevBallPosition != ballPosition)
                {
                    //cache the ball position
                    _prevBallPosition = ballPosition;

                    //run the logic for protecting the goal, find the position
                    Vector3 ballRelativePosToGoal = Owner.Goal.transform.InverseTransformPoint(ballPosition);
                    ballRelativePosToGoal.z /= 8f;
                    ballRelativePosToGoal.x /= (2f + ballRelativePosToGoal.z);
                    if (ballRelativePosToGoal.x < -3.5)
                        ballRelativePosToGoal.x = -3.5f;
                    if (ballRelativePosToGoal.x > 3.5)
                        ballRelativePosToGoal.x = 3.5f;

                    _steeringTarget = Owner.Goal.transform.TransformPoint(ballRelativePosToGoal);

                    float limit = 1f - Owner.GoalKeeping;
                    _steeringTarget.x += Random.Range(-limit, limit);
                    _steeringTarget.z += Random.Range(-limit, limit);
                }

                //reset the time 
                _timeSinceLastUpdate = 2f * (1f - Owner.GoalKeeping);
                if (_timeSinceLastUpdate == 0f)
                    _timeSinceLastUpdate = 2f * 0.1f;
            }

            //decrement the time
            _timeSinceLastUpdate -= Time.deltaTime;

            //set the ability to steer here
            Owner.RPGMovement.Steer = Vector3.Distance(Owner.Position, _steeringTarget) >= 1f;        
            Owner.RPGMovement.SetMoveTarget(_steeringTarget);
            

            //get my relative velocity
            Vector3 relativeVelocity = Owner.transform.InverseTransformDirection(Owner.RPGMovement.Velocity);
            float clampedForward = Mathf.Clamp(relativeVelocity.z, -1f, 0.5f);
            float clampedSide = Mathf.Clamp(relativeVelocity.x, -1f, 1f);

            //update the animator
            Owner.Animator.SetFloat("Forward", clampedForward, 0.1f, 0.1f);
            Owner.Animator.SetFloat("Turn", clampedSide, 0.1f, 0.1f);
        }

        public override void Exit()
        {
            base.Exit();

            //deregister to some events
            Owner.OnBallLaunched -= Instance_OnBallLaunched;

            //set the animator
            Owner.Animator.ResetTrigger("TendGoal");
        }

        private void Instance_OnBallLaunched(float flightTime, float velocity, Vector3 initial, Vector3 target)
        {
            //set some variables
            Owner.BallInitialPosition = initial;
            Owner.BallHitTarget = target;
            Owner.BallFlightTime = flightTime;
            Owner.BallVelocity = velocity;

            //change state
            if (Owner.IsShotOnTarget())
                Machine.ChangeState<InterceptShotMainState>();
            else
                Machine.ChangeState<IgnoreShotMainState>();
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
