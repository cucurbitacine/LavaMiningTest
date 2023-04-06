using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceInventory : MonoBehaviour
    {
        public UnityEvent<ResourceEntity> onPutted = new UnityEvent<ResourceEntity>();
        public UnityEvent<ResourceEntity> onPicked = new UnityEvent<ResourceEntity>();

        public readonly Dictionary<ResourceProfile, Queue<ResourceEntity>> items = new Dictionary<ResourceProfile, Queue<ResourceEntity>>();

        public bool Contains(params RequiredResourceProfile.RequiredResource[] required)
        {
            return required.All(r => r.amount <= CountResource(r.profile));
        }
        
        public int CountResource(ResourceProfile profile)
        {
            if (items.TryGetValue(profile, out var queue))
            {
                return queue.Count;
            }

            return 0;
        }
        
        public void Put(ResourceEntity resource)
        {
            if (!items.TryGetValue(resource.profile, out var queue))
            {
                queue = new Queue<ResourceEntity>();
                items.Add(resource.profile, queue);
            }

            if (queue.Contains(resource))
            {
                return;
            }
            
            queue.Enqueue(resource);
            
            Debug.Log($"{resource.name} was putted to {name}");
            
            onPutted.Invoke(resource);
        }

        public bool TryPick(ResourceProfile profile, out ResourceEntity resource)
        {
            if (items.TryGetValue(profile, out var queue))
            {
                if (queue.TryDequeue(out resource))
                {
                    Debug.Log($"{resource.name} was picked from {name}");
                    
                    onPicked.Invoke(resource);
                    
                    return true;
                }
            }

            resource = null;
            
            return false;
        }

        public bool TryPick(RequiredResourceProfile.RequiredResource[] required, List<ResourceEntity> resources)
        {
            if (!Contains(required)) return false;
            
            resources.Clear();

            foreach (var r in required)
            {
                for (var i = 0; i < r.amount; i++)
                {
                    if (TryPick(r.profile, out var resource))
                    {
                        resources.Add(resource);
                    }
                }
            }

            return true;
        }

        public void AnimationPut(ResourceEntity resource)
        {
            StartCoroutine(_AnimationPut(resource));
        }
        
        private IEnumerator _AnimationPut(ResourceEntity resource)
        {
            resource.collectable = false;
            
            var duration = 1f;

            resource.transform.DOJump(transform.position, 1, 3, duration);
            resource.transform.DOShakeScale(duration);
            resource.transform.DOScale(Vector3.zero, duration);
            
            yield return new WaitForSeconds(duration);
            
            resource.gameObject.SetActive(false);
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