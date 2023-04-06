using System.Collections;
using DG.Tweening;
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

        private float _timeoutWaitingDelta = 0f;
        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _recovering = null;
        private Coroutine _waiting = null;

        public bool Mine()
        {
            if (!active) return false;

            if (_timeoutWaitingDelta > 0) return false;

            if (_timeoutRecoveryDelta > 0) return false;

            if (!CanBeMinedCheckRequiredResources()) return false;
            
            amountMining--;
            
            if (amountMining == 0)
            {
                Recovering();
            }
            else
            {
                Waiting();
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

        private void Waiting()
        {
            if (_waiting != null) StopCoroutine(_waiting);
            _waiting = StartCoroutine(_Waiting());
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

        private IEnumerator _Waiting()
        {
            _timeoutWaitingDelta = 1f / profile.frequencyMining;
            
            while (_timeoutWaitingDelta > 0)
            {
                _timeoutWaitingDelta -= Time.deltaTime;
                yield return null;
            }
        }

        public void AnimationDrop(ResourceSourceEntity source)
        {
            for (int i = 0; i < source.profile.outputResourceAmount; i++)
            {
                var resource = Instantiate(source.profile.outputResourcePrefab);
                StartCoroutine(_AnimationDrop(resource));
            }
        }
        
        public float heightDrop = 2f;
        public float radiusDrop = 2f;
        
        private IEnumerator _AnimationDrop(ResourceEntity resource)
        {
            var startPoint = transform.position + Vector3.up * heightDrop;
            var dropPoint = transform.position +
                            radiusDrop * Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up).normalized;
            var targetScale = resource.transform.localScale;
            
            resource.collectable = false;
            resource.transform.position = startPoint;
            resource.transform.localScale = Vector3.zero;
            
            var duration = 0.5f;
            resource.transform.DOJump(dropPoint, 1, 3, duration);
            resource.transform.DOShakeScale(duration);
            resource.transform.DOScale(targetScale, duration);
            
            yield return new WaitForSeconds(duration);
            
            resource.collectable = true;
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
