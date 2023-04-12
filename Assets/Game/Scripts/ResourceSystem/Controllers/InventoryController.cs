using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    /// <summary>
    /// Controller of inventory which keeps resources
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        public UnityEvent<ResourceBehaviour> onPutted = new UnityEvent<ResourceBehaviour>();
        public UnityEvent<ResourceBehaviour> onPicked = new UnityEvent<ResourceBehaviour>();

        /// <summary>
        /// All items.
        /// Queues of resources by their profiles
        /// </summary>
        public readonly Dictionary<ResourceProfile, Queue<ResourceBehaviour>> items = new Dictionary<ResourceProfile, Queue<ResourceBehaviour>>();

        /// <summary>
        /// Contains all resources in the inventory
        /// </summary>
        /// <param name="listRequired"></param>
        /// <returns></returns>
        public bool Contains(List<ResourceStack> listRequired)
        {
            return listRequired.All(required => required.amount <= CountResource(required.profile));
        }
        
        /// <summary>
        /// Get count of resource in the inventory by its profile 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public int CountResource(ResourceProfile profile)
        {
            return items.TryGetValue(profile, out var queue) ? queue.Count : 0;
        }
        
        public void Put(ResourceBehaviour resource)
        {
            // get or add resources queue for profile
            if (!items.TryGetValue(resource.profile, out var queue))
            {
                queue = new Queue<ResourceBehaviour>();
                items.Add(resource.profile, queue);
            }

            // skip if resource already contains 
            if (queue.Contains(resource))
            {
                return;
            }
            
            // adding
            queue.Enqueue(resource);
            
            onPutted.Invoke(resource);
        }

        /// <summary>
        /// Try pick resource from the inventory by its profile
        /// </summary>
        /// <param name="inputProfile"></param>
        /// <param name="outputResource"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Picks resources from the inventory by list of required resources
        /// </summary>
        /// <param name="inputRequired"></param>
        /// <param name="outputResources"></param>
        /// <returns></returns>
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
        
        /*
         * For display stored resources in inspector
         */
        
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