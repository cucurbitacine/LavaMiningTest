using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public class PlayerInventoryStorage : MonoBehaviour
    {
        public InventoryController inventory = null;

        public List<ResourceStack> data = new List<ResourceStack>();

        public async Task Load()
        {
            Debug.Log("Loading");
            
            await Task.Delay(1000);

            foreach (var stack in data)
            {
                for (var i = 0; i < stack.amount; i++)
                {
                    var resource = stack.profile.GetResource();
                    resource.gameObject.SetActive(false);
                    inventory.Put(resource);
                }
            }
            
            Debug.Log("Loaded");
        }
        
        public async Task Save()
        {
            Debug.Log("Saving");
            
            await Task.Delay(1000);
            
            data.Clear();
            
            foreach (var pair in inventory.items)
            {
                if (pair.Value.Count > 0)
                {
                    data.Add(new ResourceStack() { amount = pair.Value.Count, profile = pair.Key });
                }
            }
            
            Debug.Log("Saved");
        }
        
        private async void OnEnable()
        {
            await Load();
        }
        
        private async void OnDisable()
        {
            await Save();
        }
    }
}
