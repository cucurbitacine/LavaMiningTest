using System.Collections;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Profiles;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class SourceBehaviour : DropperBehaviour
    {
        public SourceProfile profile = null;

        [Header("Settings")]
        public bool active = true;
        [Min(0)]
        public float timeoutResetMining = 1f;
        
        [Header("Information")]
        public bool mining = false;
        public bool recovering = false;
        [Min(0)]
        public float durationCurrentMining = 0f;
        [Min(0)]
        public int amountResources = 0;

        private float _timeoutResetMiningDelta = 0f;
        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _mining = null;
        private Coroutine _recovering = null;
        
        public bool Mine()
        {
            if (!active) return false;

            if (amountResources <= 0) return false;
            
            if (_timeoutRecoveryDelta > 0) return false;

            Mining();
            
            return true;
        }

        private void Mining()
        {
            _timeoutResetMiningDelta = timeoutResetMining;

            if (mining) return;
            
            if (_mining != null) StopCoroutine(_mining);
            _mining = StartCoroutine(_Mining());
        }
        
        private void Dropping(int amount)
        {
            var dropped = new List<ResourceBehaviour>();
            
            for (var i = 0; i < amount; i++)
            {
                dropped.Add(profile.dropResourceProfile.GetResource());
            }
            
            Drop(dropped);
        }
        
        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        private IEnumerator _Mining()
        {
            durationCurrentMining = 0f;
            var durationMaxMining = 1f / profile.frequencyMining;
            mining = true;
            
            while (0 < _timeoutResetMiningDelta)
            {
                if (durationMaxMining <= durationCurrentMining)
                {
                    var dropAmount = amountResources >= profile.dropAmountResources
                        ? profile.dropAmountResources
                        : amountResources;
            
                    amountResources -= dropAmount;
                    
                    Dropping(dropAmount);

                    if (amountResources > 0)
                    {
                        durationCurrentMining = 0f;
                    }
                    else
                    {
                        Recovering();
                    }
                }

                yield return null;
                
                _timeoutResetMiningDelta -= Time.deltaTime;
                durationCurrentMining += Time.deltaTime;
            }
            
            mining = false;
            durationCurrentMining = 0f;
        }
        
        private IEnumerator _Recovering()
        {
            mining = false;
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

        public override DropperProfile GetProfile()
        {
            return profile;
        }
    }
}
