using Assets.ImStriker.Scripts.Entities;    
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ImStriker.Scripts.Triggers
{
    public class GoalTrigger : MonoBehaviour
    {
        public Action OnCollidedWithBall;


        [SerializeField]
        GameObject OutNet;
        [SerializeField]
        GameObject Block;

        private void OnTriggerEnter(Collider other)
        {
            // If tag is ball
            if (other.CompareTag("Ball"))
            {
                // Invoke the action
                Action temp = OnCollidedWithBall;
                temp?.Invoke();

                // Immediately toggle MeshColliders
                ToggleMeshCollider(OutNet, false);
                ToggleMeshCollider(Block, true);

                // Schedule the reset after 2 seconds
                Invoke(nameof(ResetColliders), 2f);

                // Deactivate the GoalTrigger
                gameObject.SetActive(false);
            }
        }

        private void ToggleMeshCollider(GameObject obj, bool state)
        {
            if (obj.TryGetComponent<MeshCollider>(out MeshCollider meshCollider))
            {
                meshCollider.enabled = state;
            }
            else
            {
                Debug.LogWarning($"MeshCollider not found on {obj.name}");
            }
        }

        private void ResetColliders()
        {
            // Revert the MeshCollider states
            ToggleMeshCollider(OutNet, true);
            ToggleMeshCollider(Block, false);
        }
    }
}
