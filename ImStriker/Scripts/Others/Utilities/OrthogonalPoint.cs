using Assets.ImStriker.Scripts.Entities;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.ImStriker.Scripts.Others.Utilities
{
    public static class OrthogonalPoint
    {
        /// <summary>
        /// Given two points from and target, this function calculates the direction vector starting at point to destination target
        /// and returns a point on Vector from->target such that Vector point->p is orthogonal to Vector3 from->target.
        /// Also adjusts the point to account for the Magnus effect (spin-induced curve).
        /// </summary>
        /// <returns>The point.</returns>
        /// <param name="from">From.</param>
        /// <param name="target">Target.</param>
        /// <param name="point">Point.</param>
        /// <param name="spin">Spin of the ball.</param>
        /// <param name="magnusCoefficient">Magnus effect coefficient.</param>
        public static Vector3 OrthPoint(Vector3 from, Vector3 target, Vector3 point, Vector3 spin)
        {
            // Calculate the base orthogonal point
            Vector3 directionToTarget = (target - from).normalized;
            float distanceFromToPoint = Vector3.Distance(from, point);
            float angle = Mathf.Deg2Rad * Vector3.Angle(target - from, point - from);
            Vector3 orthogonalBase = from + (directionToTarget * (distanceFromToPoint * Mathf.Cos(angle)));

            // Calculate Magnus force adjustment
            Vector3 velocity = (point-from).normalized; // Approximation of velocity direction
            float magnus = spin.y / -300 + 0.05f;

            Vector3 magnusForce = magnus * Vector3.Cross(spin, velocity); // x<5+ point -spin je los, ostalo dobro

            // Adjust the orthogonal point for the Magnus force
            Vector3 curveAdjustment = magnusForce * distanceFromToPoint; // Scale by distance for smoother transition
            Vector3 curvedPoint = orthogonalBase + curveAdjustment;
            
            return curvedPoint;
        }
    }
}
