using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class InventoryController : MonoBehaviour
    {
        public UnityEvent<ResourceBehaviour> onPutted = new UnityEvent<ResourceBehaviour>();
        public UnityEvent<ResourceBehaviour> onPicked = new UnityEvent<ResourceBehaviour>();

        public readonly Dictionary<ResourceProfile, Queue<ResourceBehaviour>> items = new Dictionary<ResourceProfile, Queue<ResourceBehaviour>>();

        public bool Contains(List<ResourceStack> listRequired)
        {
            return listRequired.All(required => required.amount <= CountResource(required.profile));
        }
        
        public int CountResource(ResourceProfile profile)
        {
            return items.TryGetValue(profile, out var queue) ? queue.Count : 0;
        }
        
        public void Put(ResourceBehaviour resource)
        {
            if (!items.TryGetValue(resource.profile, out var queue))
            {
                queue = new Queue<ResourceBehaviour>();
                items.Add(resource.profile, queue);
            }

            if (queue.Contains(resource))
            {
                return;
            }
            
            queue.Enqueue(resource);
            
            onPutted.Invoke(resource);
        }

        public bool TryPick(ResourceProfile inputProfile, out ResourceBehaviour outputResource)
        {
            if (items.TryGetValue(inputProfile, out var queue))
            {
                if (queue.TryDequeue(out outputResource))
                {
                    onPicked.Invoke(outputResource);
                    return true;
                }
            }

            outputResource = null;
            return false;
        }

        public int Pick(List<ResourceStack> inputRequired, ref List<ResourceBehaviour> outputResources)
        {
            outputResources.Clear();

            foreach (var input in inputRequired)
            {
                for (var i = 0; i < input.amount; i++)
                {
                    if (TryPick(input.profile, out var output))
                    {
                        outputResources.Add(output);
                    }
                }
            }

            return outputResources.Count;
        }

#if UNITY_EDITOR
        [SerializeField] private List<InventoryItem> _displayItems = new List<InventoryItem>();

        private void Update()
        {
            _displayItems.Clear();
            _displayItems.AddRange(items.Keys.Select(k => new InventoryItem()
                { profileName = k.name, amount = items[k].Count }));
        }

        [Serializable]
        internal class InventoryItem
        {
            public string profileName = string.Empty;
            public int amount = 0;
        }
#endif
    }
}