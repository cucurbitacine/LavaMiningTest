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

        [Space] [Min(0)]
        public bool waiting = false;
        public bool recovering = false;
        public int amountResources = 0;

        private float _timeoutWaitingDelta = 0f;
        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _waiting = null;
        private Coroutine _recovering = null;

        protected override ResourceBehaviour GetResource()
        {
            return profile.dropResourcePrefab;
        }
        
        public bool Mine()
        {
            if (!active) return false;

            if (amountResources <= 0) return false;
            
            if (_timeoutWaitingDelta > 0) return false;
            
            if (_timeoutRecoveryDelta > 0) return false;

            var dropAmount = amountResources >= profile.dropAmountResources
                ? profile.dropAmountResources
                : amountResources;
            
            amountResources -= dropAmount;
            Dropping(dropAmount);

            if (amountResources > 0)
            {
                Waiting();
            }
            else
            {
                Recovering();
            }
            
            return true;
        }

        private void Dropping(int amount)
        {
            StartCoroutine(_Dropping(amount));
        }
        
        private void Waiting()
        {
            if (_waiting != null) StopCoroutine(_waiting);
            _waiting = StartCoroutine(_Waiting());
        } 
        
        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        private IEnumerator _Dropping(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                Drop();

                yield return new WaitForSeconds(profile.timeoutDropping);
            }
        }
        
        private IEnumerator _Waiting()
        {
            waiting = true;
            
            _timeoutWaitingDelta = 1f / profile.frequencyMining;
            
            while (_timeoutWaitingDelta > 0)
            {
                _timeoutWaitingDelta -= Time.deltaTime;
                yield return null;
            }
            
            waiting = false;
        }

        private IEnumerator _Recovering()
        {
            recovering = true;
            
            _timeoutRecoveryDelta = profile.timeoutMining;

            while (_timeoutRecoveryDelta > 0)
            {
                _timeoutRecoveryDelta -= Time.deltaTime;
                yield return null;
            }

            amountResources = profile.maxAmountResources;
            
            recovering = false;
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
