using Assets.ImStriker.Scripts.Entities;
using Assets.ImStriker.Scripts.States.GoalKeeperStates.Idle.MainState;
using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ImStriker.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        float _ballKickForce = 15;

        [SerializeField]
        Ball _ball;

        [SerializeField]
        Goal _goal;

        [SerializeField]
        GoalKeeper _goalKeeper;

        [SerializeField]
        Text _scoreText;

        /// <summary>
        /// dodano
        /// </summary>
        [SerializeField]
        GameObject _raycastReceiver;

        [SerializeField]
        GameObject _goalMouth;

        [SerializeField]
        GameObject _player;
        bool _run = true;
        int _score;
        Vector3 _ballInitPos;
        Quaternion _ballInitRot;

        protected Transform Cam;
        protected Vector3 CamForward;             // The current forward direction of the camera

        public delegate void BallLaunch(float power, Vector3 target, Vector3 curveDirection);   //delegate to launch a ball
        public BallLaunch OnBallLaunch;                                 //on ball launch

        private float _chargeStartTime; // Tracks when the mouse button was pressed

        [SerializeField]
        private float _maxChargeDuration = 2f; // Maximum time to charge power

        [SerializeField]
        private float _maxKickForce = 30f; // Maximum force when fully charged

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;
        private List<GameObject> _spawnedPlayers = new List<GameObject>();
        private float randomX;
        private float randomZ;

        public override void Awake()
        {
            // register the game manager to some events
            //_ball.OnBallLaunched += SoundManager.Instance.PlayBallKickedSound;
            //_goalKeeper.OnPunchBall += SoundManager.Instance.PlayBallKickedSound;
            //_goal.GoalTrigger.OnCollidedWithBall += SoundManager.Instance.PlayGoalScoredSound;

            //register entities to entitiy delegates
            _ball.OnBallLaunched += _goalKeeper.Instance_OnBallLaunched;

            //register entities to entitiy delegates
            _goal.GoalTrigger.OnCollidedWithBall += Instance_OnBallCollidedWithGoal;
            //register entities to local delegates
            OnBallLaunch += _ball.Instance_OnBallLaunch;

            //cache the initial data
            _ballInitPos = _ball.Position;
            _ballInitRot = _ball.Rotation;

            // get the transform of the main camera
            if (Camera.main != null)
                Cam = Camera.main.transform;
            else
                Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        private void Instance_OnBallCollidedWithGoal()
        {
            ++_score;
            _scoreText.text = string.Format("Score:{0}", _score);
            
        }


        private void Update()
        {
            if (!_run)
                return;

            #region TriggerShooting

            if (Input.GetMouseButtonDown(0))
            {
                _dragStartPos = Input.mousePosition; // Record the start position
                _chargeStartTime = Time.time; // Record the time the button was pressed
            }

            //get the mouse
            if (Input.GetMouseButtonUp(0))
            {
                _dragEndPos = Input.mousePosition; // Record the end position

                Vector3 dragVector = _dragEndPos - _dragStartPos;
                if (dragVector.x > 300f)
                    dragVector.x = 300f;
                if (dragVector.x < -300f)
                    dragVector.x = -300f;
                Vector3 curveDirection = new(0f, -dragVector.x / 100f, 0f); // Calculate curve direction and intensity >>To stavi od 200 do 100

                float chargeDuration = Time.time - _chargeStartTime; // Calculate the charge duration
                chargeDuration = Mathf.Clamp(chargeDuration, 0, _maxChargeDuration);// Clamp the charge duration to the maximum allowed
                float scaledKickForce = Mathf.Lerp(_ballKickForce, _maxKickForce, chargeDuration / _maxChargeDuration);

                // Create a ray from the mouse position
                Ray ray = Camera.main.ScreenPointToRay(_dragStartPos);

                // Perform raycast to find the target point
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _run = false; // Prevent further actions until reset
                    float circleRadius = 0.5f; // Adjust this radius to match your target circle size
                    Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * circleRadius;
                    Vector3 target = hit.point + new Vector3(randomOffset.x, randomOffset.y, 0f);
                    // Launch the ball to the random target

                    OnBallLaunch?.Invoke(scaledKickForce, target, curveDirection);

                    StartCoroutine(Reset());
                }
            }
            #endregion
        }

        private Vector3 GenerateRandomBallPosition()
        {
            float randomX = UnityEngine.Random.Range(-25f, 25f);
            float randomZ = UnityEngine.Random.Range(48f, 16f);

            // Log the coordinates to the console
            return new Vector3(randomX, _ballInitPos.y, randomZ);
        }



        private void PositionCamera(Vector3 ballPosition)
        {
            if (Cam != null)
            {
                Cam.position = new Vector3(ballPosition.x, ballPosition.y + 1.8f, ballPosition.z);
                _raycastReceiver.transform.position = new Vector3(0f, 0f, 15f + ballPosition.z);

                Vector3 targetPointCam = new Vector3(0f, 0f, 55f);
                Vector3 directionCam = targetPointCam - Cam.position;
                Cam.rotation = Quaternion.LookRotation(directionCam);

                Quaternion fullRotation = Quaternion.LookRotation(directionCam);
                Vector3 eulerAngles = fullRotation.eulerAngles;

                _raycastReceiver.transform.rotation = Quaternion.Euler(
                    _raycastReceiver.transform.rotation.eulerAngles.x, // Preserve x
                    eulerAngles.y,                                    // Update y
                    _raycastReceiver.transform.rotation.eulerAngles.z // Preserve z
                );
                _goalKeeper.transform.rotation = Quaternion.Euler(
                    0f, // Preserve x
                    eulerAngles.y + 180,                                    // Update y
                    0f // Preserve z
                );

                // Destroy previously spawned players
                foreach (GameObject player in _spawnedPlayers)
                {
                    Destroy(player);
                }
                _spawnedPlayers.Clear();

                // Instantiate new players
                int players = (int)Math.Round(_goal.transform.position.z - _ball.transform.position.z) / 5;
               
                
                if (_ball.transform.position.z < 49)
                {
                    for (int i = 0; i < players; i++)
                    {
                        bool isPositionValid;
                        Vector3 newPosition;

                        do
                        {
                            isPositionValid = true;
                            if (_ball.transform.position.x > 0f)
                                randomX = UnityEngine.Random.Range(_ball.transform.position.x, _ball.transform.position.x - 16f);
                            else
                                randomX = UnityEngine.Random.Range(_ball.transform.position.x, _ball.transform.position.x + 16f);
                            randomZ = UnityEngine.Random.Range(_ball.transform.position.z + 7, 49f);

                            newPosition = new Vector3(randomX, 0f, randomZ);
                            Debug.Log(randomZ);
                            // Check against all existing players
                            foreach (GameObject player in _spawnedPlayers)
                            {
                                float distance = Vector3.Distance(newPosition, player.transform.position);

                                // Set a minimum distance to ensure no overlap or too close positions
                                if (distance < 1.0f) // Adjust the threshold as needed
                                {
                                    isPositionValid = false;
                                    break;
                                }
                            }

                        } while (!isPositionValid);

                        // Instantiate the new player once a valid position is found
                        GameObject newPlayer = Instantiate(_player, newPosition, _goalKeeper.transform.rotation);
                        _spawnedPlayers.Add(newPlayer);
                    }
                }
                
            }
        }

        private IEnumerator Reset()
        {
            yield return new WaitForSeconds(5f);

            _ball.gameObject.SetActive(false);
            _ball.Stop();
            Vector3 newBallPosition = GenerateRandomBallPosition();
            _ball.Position = newBallPosition; // Set random position
            _ball.Rotation = _ballInitRot;

            PositionCamera(newBallPosition); // Update camera position and rotation
            _goalKeeper.FSM.ChangeState<IdleMainState>();

            yield return new WaitForSeconds(1f);

            _run = true;
            _ball.gameObject.SetActive(true);
            _goal.GoalTrigger.gameObject.SetActive(true);
        }

    }
}