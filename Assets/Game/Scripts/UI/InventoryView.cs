using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.UI
{
    public abstract class InventoryView : MonoBehaviour
    {
        public InventoryController inventory { get; private set; }

        protected abstract void Put(ResourceBehaviour resource);
        protected abstract void Pick(ResourceBehaviour resource);

        public void Subscribe(InventoryController resourceInventory)
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