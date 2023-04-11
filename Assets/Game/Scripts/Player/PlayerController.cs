using System;
using Game.Scripts.ResourceSystem.Controllers;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool moving;
        
        [Space]
        public NavMeshAgent agent = null;

        [Space]
        public CollectorController collector = null;
        public InventoryController inventory = null;
        public MinerController miner = null;
        public DistributorController distributor = null;
        
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

        private void Awake()
        {
            if (miner == null) miner = GetComponentInChildren<MinerController>();
            if (collector == null) collector = GetComponentInChildren<CollectorController>();
            if (distributor == null) distributor = GetComponentInChildren<DistributorController>();
            if (inventory == null) inventory = GetComponentInChildren<InventoryController>();
        }
    }
}
