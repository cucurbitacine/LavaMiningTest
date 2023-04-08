using System.Collections;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class SpotBehaviour : DropperBehaviour
    {
        public SpotProfile profile = null;
        public bool producting = false;
        [Range(0f, 1f)]
        public float productionProgress = 0f;
        
        [Space]
        public bool active = true;
        
        [Space]
        public UnityEvent<bool> onProductionChanged = new UnityEvent<bool>();

        [Space]
        public InventoryController inventory = null;
        
        private Coroutine _production = null;

        private List<ResourceBehaviour> _cacheResources = new List<ResourceBehaviour>();
        
        protected override ResourceBehaviour GetResource()
        {
            return profile.dropResourcePrefab;
        }
        
        public void FillRequired(ref List<ResourceStack> outputRequired)
        {
            outputRequired.Clear();

            if (inventory == null) return;
            if (profile.inputRequiredProfile == null) return;
            
            foreach (var requiredResource in profile.inputRequiredProfile.requiredResources)
            {
                var amountResourceInInventory = inventory.CountResource(requiredResource.profile);

                if (amountResourceInInventory < requiredResource.amount)
                {
                    var output = new ResourceStack()
                    {
                        amount = requiredResource.amount - amountResourceInInventory,
                        profile = requiredResource.profile,
                    };
                    
                    outputRequired.Add(output);
                }
            }
        }
        
        private IEnumerator _Production()
        {
            while (true)
            {
                if (!active)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                if (inventory.Contains(profile.inputRequiredProfile.requiredResources))
                {
                    inventory.Pick(profile.inputRequiredProfile.requiredResources, ref _cacheResources);

                    DisposeOfResources(_cacheResources);
                    
                    var time = 0f;
                    productionProgress = 0f;

                    producting = true;
                    onProductionChanged.Invoke(producting);
                    
                    while (time < profile.durationProduction)
                    {
                        productionProgress = time / profile.durationProduction;
                        time += Time.deltaTime;
                        yield return null;
                    }
                    
                    productionProgress = 1f;
                    
                    producting = false;
                    onProductionChanged.Invoke(producting);

                    productionProgress = 0f;
                    
                    for (var i = 0; i < profile.dropAmountResources; i++)
                    {
                        Drop();
                        
                        yield return new WaitForSeconds(profile.timeoutDropping);
                    }
                    
                    yield return new WaitForSeconds(profile.timeoutProduction);
                }
                
                yield return new WaitForFixedUpdate();
            }
        }

        private void DisposeOfResources(List<ResourceBehaviour> resources)
        {
            //
        }
        
        private void OnEnable()
        {
            if (_production != null) StopCoroutine(_production);
            _production = StartCoroutine(_Production());
        }

        private void OnDisable()
        {
            if (_production != null) StopCoroutine(_production);
        }
    }
}