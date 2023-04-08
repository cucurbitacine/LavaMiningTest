using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    public abstract class DropperProfile : ScriptableObjectWithGuid
    {
        [Header("Drop Settings")]
        [Min(0)]
        public int dropAmountResources = 1;
        public ResourceBehaviour dropResourcePrefab = null;
    }
}