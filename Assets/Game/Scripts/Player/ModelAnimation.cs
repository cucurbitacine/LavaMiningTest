using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player
{
    /// <summary>
    /// Animation controller
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ModelAnimation : MonoBehaviour
    {
        public PlayerController player = null;
        
        private Animator _animator = null;
        private Coroutine _animation = null;
        
        private static readonly int Mining = Animator.StringToHash("Mining");

        private IEnumerator _Animation()
        {
            // waiting player and miner controller  
            while (player == null || player.miner == null)
            {
                yield return null;
            }

            while (true)
            {
                // update mining parameter
                _animator.SetBool(Mining, player.miner.mining);

                yield return null;
            }
        }

        private void Awake()
        {
            if (_animator == null) _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (player == null) player = GetComponentInParent<PlayerController>();
            
            if (_animation != null) StopCoroutine(_animation);
            _animation = StartCoroutine(_Animation());
        }

        private void OnDisable()
        {
            if (_animation != null) StopCoroutine(_animation);

            player = null;
        }
    }
}