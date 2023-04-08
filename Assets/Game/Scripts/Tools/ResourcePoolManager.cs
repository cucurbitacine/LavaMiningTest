using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public static class ResourcePoolManager
    {
        private static readonly Dictionary<ResourceProfile, Dictionary<ResourceBehaviour, bool>> Pool =
            new Dictionary<ResourceProfile, Dictionary<ResourceBehaviour, bool>>();
        
        public static ResourceBehaviour GetResourceOfProfile(ResourceProfile profile)
        {
            if (!Pool.TryGetValue(profile, out var resources))
            {
                resources = new Dictionary<ResourceBehaviour, bool>();
                Pool.Add(profile, resources);
            }

            var freeResource = resources.FirstOrDefault(r => r.Value).Key;

            if (freeResource == null)
            {
                freeResource = Object.Instantiate(profile.prefab);
                freeResource.profile = profile;
                resources.Add(freeResource, false);
            }
            else
            {
                resources[freeResource] = false;
                freeResource.gameObject.SetActive(true);
            }
            
            return freeResource;
        }

        public static void FreeResourceOfProfile(ResourceProfile profile, ResourceBehaviour resource)
        {
            if (!Pool.TryGetValue(profile, out var resources))
            {
                resources = new Dictionary<ResourceBehaviour, bool>();
                Pool.Add(profile, resources);
            }

            if (resources.TryGetValue(resource, out _))
            {
                resources[resource] = true;
            }
            else
            {
                resources.Add(resource, true);
            }
            
            resource.gameObject.SetActive(false);
        }
        
        public static ResourceBehaviour GetResource(this ResourceProfile profile)
        {
            return GetResourceOfProfile(profile);
        }

        public static void FreeResource(this ResourceProfile profile, ResourceBehaviour resource)
        {
            FreeResourceOfProfile(profile, resource);
        }

        public static void Free(this ResourceBehaviour resource)
        {
            FreeResourceOfProfile(resource.profile, resource);
        }
    }
}