using System.Collections;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceSourceEntity : MonoBehaviour
    {
        public ResourceSourceProfile profile = null;

        [Space]
        public bool active = true;
        
        [Space]
        [Min(0)]
        public int amountMining = 0;

        [Space]
        public UnityEvent<ResourceSourceEntity> onWasMined = new UnityEvent<ResourceSourceEntity>();

        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _recovering = null;

        public bool Mine()
        {
            if (!active) return false;

            if (_timeoutRecoveryDelta > 0) return false;

            if (!CanBeMinedCheckRequiredResources()) return false;
            
            amountMining--;
            
            if (amountMining == 0)
            {
                Recovering();
            }

            onWasMined.Invoke(this);
            
            return true;
        }

        protected virtual bool CanBeMinedCheckRequiredResources()
        {
            return true;
        }
        
        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        private IEnumerator _Recovering()
        {
            _timeoutRecoveryDelta = profile.timeoutMining;
            
            while (_timeoutRecoveryDelta > 0)
            {
                _timeoutRecoveryDelta -= Time.deltaTime;
                yield return null;
            }

            amountMining = profile.amountMaxMining;
        }

        private void OnEnable()
        {
            amountMining = profile.amountMaxMining;
        }

        private void OnDisable()
        {
            amountMining = 0;
        }
    }
}
