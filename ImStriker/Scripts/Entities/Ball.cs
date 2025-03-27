using UnityEngine;

namespace Assets.ImStriker.Scripts.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Ball : MonoBehaviour
    {
        [Tooltip("The gravity acting on the ball")]
        public float gravity = 9f;

        [Tooltip("Magnus coefficient for the ball's spin")]
        public float magnusCoefficient = 0.1f; // Adjust for the desired curve effect

        public delegate void BallLaunched(float flightTime, float velocity, Vector3 initial, Vector3 target);
        public BallLaunched OnBallLaunched;

        public Rigidbody Rigidbody { get; set; }
        public SphereCollider SphereCollider { get; set; }

        private void Awake()
        {
            // Get the components
            Rigidbody = GetComponent<Rigidbody>();
            SphereCollider = GetComponent<SphereCollider>();

            // Set the gravity of the ball
            Physics.gravity = new Vector3(0f, -gravity, 0f);
        }

        private void FixedUpdate()
        {
            // Apply Magnus effect for curving the ball
            ApplyMagnusForce();
        }

        public void Stop()
        {
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.linearVelocity = Vector3.zero;
        }

        public Vector3 FuturePosition(float time)
        {
            // Get the velocities
            Vector3 velocity = Rigidbody.linearVelocity;
            Vector3 spin = Rigidbody.angularVelocity; // The current spin
            Vector3 velocityXZ = velocity;
            velocityXZ.y = 0f;

            // Magnus force calculation
            Vector3 magnusForce = magnusCoefficient * Vector3.Cross(spin, velocity);

            // Integrate the Magnus force over time for the xz plane
            Vector3 accelerationDueToMagnus = magnusForce / Rigidbody.mass;
            //Vector3 futureVelocityXZ = velocityXZ + accelerationDueToMagnus * time;

            // Find the future position on the y-axis
            float futurePositionY = Position.y + (velocity.y * time + 0.5f * -gravity * Mathf.Pow(time, 2));

            // Find the future position in the xz-plane
            Vector3 displacementXZ = velocityXZ * time + 0.5f * Mathf.Pow(time, 2) * accelerationDueToMagnus;
            Vector3 futurePositionXZ = Position + displacementXZ;

            // Bundle the future positions together
            Vector3 futurePosition = futurePositionXZ;
            futurePosition.y = futurePositionY;

            // Return the future position
            return futurePosition;
        }



        public void Launch(float power, Vector3 final, Vector3 spin)
        {
            // Set the initial position
            Vector3 initial = Position;

            // Find the direction vectors
            Vector3 toTarget = final - initial;
            Vector3 toTargetXZ = toTarget;
            toTargetXZ.y = 0;

            // Find the time to target
            float time = toTargetXZ.magnitude / power;

            // Set the y-velocity
            Vector3 velocity = toTargetXZ.normalized * toTargetXZ.magnitude / time;
            velocity.y = toTarget.y / time + (0.5f * gravity * time);

            // Apply velocity to Rigidbody
            Rigidbody.linearVelocity = velocity;

            // Apply initial spin to the ball
            Rigidbody.angularVelocity = spin;

            // Invoke the ball launched event
            BallLaunched temp = OnBallLaunched;
            temp?.Invoke(time, power, initial, final);
        }

        private void ApplyMagnusForce()
        {
            // Get the ball's velocity and spin
            Vector3 velocity = Rigidbody.linearVelocity;
            Vector3 angularVelocity = Rigidbody.angularVelocity;

            // Calculate Magnus force: perpendicular to both velocity and spin
            Vector3 magnusForce = magnusCoefficient * Vector3.Cross(angularVelocity, velocity);

            // Apply the force to the Rigidbody
            Rigidbody.AddForce(magnusForce, ForceMode.Force);
        }

        public void Instance_OnBallLaunch(float power, Vector3 target, Vector3 spin)
        {
            // Launch the ball with the specified power, target, and spin
            Launch(power, target, spin);
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
    }
}
