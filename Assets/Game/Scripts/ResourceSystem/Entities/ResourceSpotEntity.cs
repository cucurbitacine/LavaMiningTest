using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Controllers;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceSpotEntity : ResourceSourceEntity
    {
        public ResourceInventory inventory = null;
        
        protected override bool CanBeMinedCheckRequiredResources()
        {
            if (inventory == null) return false;
            if (!profile.inputRequired) return true;
            if (profile.inputResourceProfile == null) return false;

            var resources = new List<ResourceEntity>();
            if (inventory.TryPick(profile.inputResourceProfile.requiredResources, resources))
            {
                foreach (var resource in resources)
                {
                    resource.gameObject.SetActive(false);
                }
                
                return true;
            }

            return false;
        }
    }
}