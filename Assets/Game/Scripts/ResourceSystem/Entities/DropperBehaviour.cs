using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    /// <summary>
    /// Dropper entity. This is what can drop resources
    /// </summary>
    public abstract class DropperBehaviour : MonoBehaviour
    {
        public UnityEvent<List<ResourceBehaviour>> onResourceDropped = new UnityEvent<List<ResourceBehaviour>>();

        public abstract DropperProfile GetProfile();

        /// <summary>
        /// Dropping resources
        /// </summary>
        /// <param name="droppedResources"></param>
        public void Drop(List<ResourceBehaviour> droppedResources)
        {
            onResourceDropped.Invoke(droppedResources);
        }
    }
}