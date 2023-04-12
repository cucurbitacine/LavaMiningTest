using System.Collections;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Profiles;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    /// <summary>
    /// Source behaviour. Main source of free resources
    /// </summary>
    public class SourceBehaviour : DropperBehaviour
    {
        public SourceProfile profile = null;

        [Header("Settings")]
        public bool active = true;
        
        /// <summary>
        /// Time to reset mining, if during which no mining
        /// </summary>
        [Min(0)]
        public float timeoutResetMining = 1f;
        
        [Header("Information")]
        public bool mining = false;
        public bool recovering = false;
        
        /// <summary>
        /// Time of current mining process 
        /// </summary>
        [Min(0)]
        public float durationCurrentMining = 0f;
        
        /// <summary>
        /// Amount available resources
        /// </summary>
        [Min(0)]
        public int amountResources = 0;

        private float _timeoutResetMiningDelta = 0f;
        private float _timeoutRecoveryDelta = 0f;
        
        private Coroutine _mining = null;
        private Coroutine _recovering = null;
        
        public override DropperProfile GetProfile()
        {
            return profile;
        }
        
        /// <summary>
        /// Main method for mining
        /// </summary>
        /// <returns></returns>
        public bool Mine()
        {
            // check active state
            if (!active) return false;

            // check amount available resources 
            if (amountResources <= 0) return false;
            
            // check recovery process
            if (recovering) return false;

            // reset timeout to continue mining process
            _timeoutResetMiningDelta = timeoutResetMining;

            // skip if already are mining
            if (mining) return true;
            
            // start mining process
            if (_mining != null) StopCoroutine(_mining);
            _mining = StartCoroutine(_Mining());

            return true;
        }

        /// <summary>
        /// Drop some amount resources 
        /// </summary>
        /// <param name="amount"></param>
        private void Dropping(int amount)
        {
            if (amount <= 0) return;
            
            var dropped = new List<ResourceBehaviour>();
            
            for (var i = 0; i < amount; i++)
            {
                dropped.Add(profile.dropResourceProfile.GetResource());
            }
            
            Drop(dropped);
        }
        
        /// <summary>
        /// Start recovering process
        /// </summary>
        private void Recovering()
        {
            if (_recovering != null) StopCoroutine(_recovering);
            _recovering = StartCoroutine(_Recovering());
        }

        /// <summary>
        /// Main mining process
        /// </summary>
        /// <returns></returns>
        private IEnumerator _Mining()
        {
            // reset time current mining process and calculate time max mining
            durationCurrentMining = 0f;
            var durationMaxMining = 1f / profile.frequencyMining;
            mining = true;
            
            // mining process is working if it not reset
            while (0 < _timeoutResetMiningDelta)
            {
                // check time current mining process
                if (durationMaxMining <= durationCurrentMining)
                {
                    // calculate available amount resources for dropping 
                    var dropAmount = amountResources >= profile.dropAmountResources
                        ? profile.dropAmountResources
                        : amountResources;
                    
                    amountResources -= dropAmount;
                    
                    Dropping(dropAmount);

                    // reset time current mining process
                    durationCurrentMining = 0f;
                    
                    // if source is empty - start recovering
                    if (amountResources <= 0)
                    {
                        Recovering();
                    }
                }

                yield return null;
                
                // update time to reset mining and time mining
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
            
            _timeoutRecoveryDelta = profile.timeoutRecovery;

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
