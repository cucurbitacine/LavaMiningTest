using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool moving;
        
        [Space]
        public NavMeshAgent agent = null;

        public void Move(Vector3 offset)
        {
            agent.Move(offset);
        }

        public void View(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
