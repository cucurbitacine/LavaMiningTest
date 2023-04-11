using System.Collections;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class ModelAnimation : MonoBehaviour
    {
        public PlayerController player = null;
        
        private Animator _animator = null;
        private Coroutine _animation = null;
        
        private static readonly int Mining = Animator.StringToHash("Mining");

        private IEnumerator _Animation()
        {
            while (player == null || player.miner == null)
            {
                yield return null;
            }

            while (true)
            {
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