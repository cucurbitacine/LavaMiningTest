using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceSpotEntity : ResourceSourceEntity
    {
        [Space]
        public ResourceInventory inventory = null;

        public void FillRequired(ref List<RequiredResource> outputRequired)
        {
            outputRequired.Clear();

            if (inventory == null) return;
            if (profile.inputResourceProfile == null) return;
            
            foreach (var requiredResource in profile.inputResourceProfile.requiredResources)
            {
                var amountResourceInInventory = inventory.CountResource(requiredResource.profile);

                if (amountResourceInInventory < requiredResource.amount)
                {
                    var output = new RequiredResource()
                    {
                        amount = requiredResource.amount - amountResourceInInventory,
                        profile = requiredResource.profile,
                    };
                    
                    outputRequired.Add(output);
                }
            }
        }
        
        protected override bool CanBeMinedInternalCheck()
        {
            if (inventory == null) return false;
            if (profile.inputResourceProfile == null) return false;

            var resources = new List<ResourceEntity>();
            if (inventory.Contains(profile.inputResourceProfile.requiredResources))
            {
                if (inventory.Pick(profile.inputResourceProfile.requiredResources, ref resources) > 0)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}