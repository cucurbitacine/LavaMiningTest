using System.Collections;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class SourceBehaviour : DropperBehaviour
    {
        public SourceProfile profile = null;

        [Space]
        public bool active = true;
        [Min(0f)]
        public float timeoutDropping = 0.1f;
        
        [Space]
        [Min(0)]
        public int amountResources = 0;

        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _recovering = null;

        protected override ResourceBehaviour GetResource()
        {
            return profile.dropResourcePrefab;
        }
        
        public bool Mine()
        {
            if (!active) return false;

            if (_timeoutRecoveryDelta > 0) return false;
            
            if (amountResources <= 0) return false;

            amountResources--;
            
            Dropping();
            
            if (amountResources == 0)
            {
                Recovering();
            }
            
            return true;
        }

        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        private void Dropping()
        {
            StartCoroutine(_Dropping());
        }
        
        private IEnumerator _Recovering()
        {
            _timeoutRecoveryDelta = profile.timeoutMining;
            
            while (_timeoutRecoveryDelta > 0)
            {
                _timeoutRecoveryDelta -= Time.deltaTime;
                yield return null;
            }

            amountResources = profile.maxAmountResources;
        }

        private IEnumerator _Dropping()
        {
            for (var i = 0; i < profile.dropAmountResources; i++)
            {
                Drop();

                yield return new WaitForSeconds(timeoutDropping);
            }
        }
        
        private void OnEnable()
        {
            amountResources = profile.maxAmountResources;
        }

        private void OnDisable()
        {
            amountResources = 0;
        }
    }
}
