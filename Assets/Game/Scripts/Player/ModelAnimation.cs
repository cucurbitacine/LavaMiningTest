using Game.Scripts.ResourceSystem.Controllers;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class ModelAnimation : MonoBehaviour
    {
        public MinerController miner = null;
        public Animator animator = null;
        
        private static readonly int Mining = Animator.StringToHash("Mining");

        private void Update()
        {
            animator.SetBool(Mining, miner.mining);
        }
    }
}