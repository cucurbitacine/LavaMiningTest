using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.UI
{
    public abstract class InventoryView : MonoBehaviour
    {
        public ResourceInventory inventory = null;

        protected abstract void Put(ResourceEntity resource);
        protected abstract void Pick(ResourceEntity resource);

        protected virtual void OnEnable()
        {
            if (inventory != null)
            {
                inventory.onPutted.AddListener(Put);
                inventory.onPicked.AddListener(Pick);
            }
        }

        protected virtual void OnDisable()
        {
            if (inventory != null)
            {
                inventory.onPutted.RemoveListener(Put);
                inventory.onPicked.RemoveListener(Pick);
            }
        }
    }
}