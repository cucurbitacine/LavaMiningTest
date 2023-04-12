using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    /// <summary>
    /// Spot behaviour. Factory which transform some resources to another 
    /// </summary>
    public class SpotBehaviour : DropperBehaviour
    {
        public SpotProfile profile = null;
        
        [Space]
        public bool active = true;
        public bool producing = false;
        [Range(0f, 1f)]
        public float productionProgress = 0f;
        
        [Space]
        public UnityEvent<bool> onProductionChanged = new UnityEvent<bool>();

        [Space]
        public InventoryController inventory = null;
        
        private Coroutine _producing = null;

        private List<ResourceBehaviour> _cacheResources = new List<ResourceBehaviour>();

        public override DropperProfile GetProfile()
        {
            return profile;
        }
        
        /// <summary>
        /// Main method for producing resources
        /// </summary>
        /// <returns></returns>
        public bool Produce()
        {
            // check active state, availability of inventory and producing state
            if (!active) return false;
            if (inventory == null) return false;
            if (producing) return false;
            
            // check spot's inventory
            if (inventory.Contains(profile.inputRequiredProfile.requiredResources))
            {
                // if inventory contains required resources - start producing
                if (_producing != null) StopCoroutine(_producing);
                _producing = StartCoroutine(_Producing());
                
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Fill the list with information about the required resources
        /// </summary>
        /// <param name="outputRequired"></param>
        public void FillRequired(ref List<ResourceStack> outputRequired)
        {
            outputRequired.Clear();

            if (inventory == null) return;
            if (profile.inputRequiredProfile == null) return;
            
            // looking list required resources in the profile
            foreach (var requiredResource in profile.inputRequiredProfile.requiredResources)
            {
                // get amount resources in the spot's inventory
                var amountResourceInInventory = inventory.CountResource(requiredResource.profile);
                
                if (amountResourceInInventory < requiredResource.amount)
                {
                    // create and add info about resource requirements
                    var output = new ResourceStack()
                    {
                        amount = requiredResource.amount - amountResourceInInventory,
                        profile = requiredResource.profile,
                    };
                    
                    outputRequired.Add(output);
                }
            }
        }

        private IEnumerator _Producing()
        {
            productionProgress = 0f;
            producing = true;
            onProductionChanged.Invoke(producing);

            // waiting producing 
            var time = 0f;
            while (time < profile.durationProduction)
            {
                productionProgress = time / profile.durationProduction;
                time += Time.deltaTime;
                yield return null;
            }
            productionProgress = 1f;

            // get outputs resources and dropping them 
            var dropped = new List<ResourceBehaviour>();
            for (var i = 0; i < profile.dropAmountResources; i++)
            {
                dropped.Add(profile.dropResourceProfile.GetResource());
            }
            Drop(dropped);
            
            yield return new WaitForSeconds(profile.timeoutRecovering);
                 
            // pick resources from spot's inventory
            inventory.Pick(profile.inputRequiredProfile.requiredResources, ref _cacheResources);
            
            // dispose resources
            DisposeOfResources(_cacheResources);

            productionProgress = 0f;
            producing = false;
            onProductionChanged.Invoke(producing);
        }

        private void DisposeOfResources(List<ResourceBehaviour> resources)
        {
            foreach (var resource in resources)
            {
                resource.Free();
            }
        }

        private void InventoryChanged(ResourceBehaviour resource)
        {
            Produce();
        }
        
        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.onPutted.AddListener(InventoryChanged);
            }
        }

        private void OnDisable()
        {
            if (_producing != null) StopCoroutine(_producing);
            
            if (inventory != null)
            {
                inventory.onPutted.RemoveListener(InventoryChanged);
            }
        }
    }
}