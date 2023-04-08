using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Entities
{
    public abstract class DropperBehaviour : MonoBehaviour
    {
        public UnityEvent<ResourceBehaviour> onResourceDropped = new UnityEvent<ResourceBehaviour>();

        public void Drop()
        {
            onResourceDropped.Invoke(GetResource());
        }
        
        protected abstract ResourceBehaviour GetResource();
    }
}