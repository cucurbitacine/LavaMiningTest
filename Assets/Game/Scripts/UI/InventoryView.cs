using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.UI
{
    public abstract class InventoryView : MonoBehaviour
    {
        public ResourceInventory inventory { get; private set; }

        protected abstract void Put(ResourceEntity resource);
        protected abstract void Pick(ResourceEntity resource);

        public void Subscribe(ResourceInventory resourceInventory)
        {
            inventory = resourceInventory;

            if (inventory != null)
            {
                inventory.onPutted.AddListener(Put);
                inventory.onPicked.AddListener(Pick);
            }
        }
        
        public void Unsubscribe()
        {
            if (inventory != null)
            {
                inventory.onPutted.RemoveListener(Put);
                inventory.onPicked.RemoveListener(Pick);
            }

            inventory = null;
        }
    }
}