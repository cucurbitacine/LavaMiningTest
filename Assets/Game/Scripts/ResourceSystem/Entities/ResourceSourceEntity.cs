using System.Collections;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

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

        private float _timeoutDelta = 0f;
        private Coroutine _recovering = null;
        
        public void Mine()
        {
            if (!active) return;
            
            amountMining--;

            if (amountMining == 0)
            {
                Recovering();
            }
            else
            {
                DropResources();
            }
        }

        private void DropResources()
        {
            // profile.outputResource.resourcePrefab
            
            Debug.Log("Drop Resource");
        }
        
        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        private IEnumerator _Recovering()
        {
            active = false;
            
            _timeoutDelta = profile.timeoutMining;
            
            while (_timeoutDelta > 0)
            {
                _timeoutDelta -= Time.deltaTime;
                yield return null;
            }

            amountMining = profile.amountMaxMining;
            
            active = true;
        }

        private void OnEnable()
        {
            amountMining = profile.amountMaxMining;
        }
    }
}
