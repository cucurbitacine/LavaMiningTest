using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    public abstract class DropperBehaviour : MonoBehaviour
    {
        public UnityEvent<List<ResourceBehaviour>> onResourceDropped = new UnityEvent<List<ResourceBehaviour>>();

        public abstract DropperProfile GetProfile();

        public TProfile GetProfile<TProfile>() where TProfile : DropperProfile
        {
            return GetProfile() as TProfile;
        }
        
        public void Drop(List<ResourceBehaviour> droppedResources)
        {
            onResourceDropped.Invoke(droppedResources);
        }
    }
}