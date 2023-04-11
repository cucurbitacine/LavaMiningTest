using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = "Create ResourceEffectProfile", fileName = "ResourceEffectProfile", order = 0)]
    public class ResourceEffectProfile : ScriptableObject
    {
        [Header("Drop from Source / Spot")]
        [Min(0f)]
        public float dropRadius = 0.5f;
        [Min(0f)]
        public float dropHeight = 3f;
        [Min(0)]
        public float dropDuration = 0.5f;
        public Vector3 dropShakeScalePower = Vector3.one;
        [Min(0f)]
        public float dropFlyDuration = 0.5f;
        [Min(0f)]
        public float dropCollectableTimeout = 1f;
        
        [Header("Collecting by Player")]
        [Min(0)]
        public float collectDuration = 0.25f;

        public Vector3 GetDropStart(Vector3 origin)
        {
            var seed = Random.insideUnitCircle;
            return origin + new Vector3(seed.x, 0f, seed.y) * dropRadius;
        }

        public Vector3 GetDropTarget(Vector3 origin)
        {
            return origin + Vector3.up * dropHeight;
        }
    }
}