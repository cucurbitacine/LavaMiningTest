using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile of effects information for resource
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceEffectProfile : ScriptableObject
    {
        [Header("Drop from Source / Spot")]
        
        [Tooltip("Radius scatter of appearing resource")]
        [Min(0f)]
        public float dropAppearRadius = 0.5f;
        
        [Tooltip("Height to which the resource will rise")]
        [Min(0f)]
        public float dropAppearHeight = 3f;
        
        [Tooltip("Time duration while the resource will be rising")]
        [Min(0)]
        public float dropAppearDuration = 0.5f;
        
        [Tooltip("Shake power of resource while dropping")]
        public Vector3 dropShakeScalePower = Vector3.one;
        
        [Tooltip("Time duration of resource flying to final drop point")]
        [Min(0f)]
        public float dropFlyDuration = 0.5f;
        
        [Tooltip("Time duration after that resource can be collected")]
        [Min(0f)]
        public float dropCollectableTimeout = 1f;
        
        [Header("Collecting by Player")]
        
        [Tooltip("Time duration of resource collecting by player")]
        [Min(0)]
        public float collectDuration = 0.25f;

        private const string ProfileName = "Effects";
        
        public Vector3 GetDropAppearStartPoint(Vector3 origin)
        {
            var seed = Random.insideUnitCircle;
            return origin + new Vector3(seed.x, 0f, seed.y) * dropAppearRadius;
        }

        public Vector3 GetDropAppearRisenPoint(Vector3 origin)
        {
            return origin + Vector3.up * dropAppearHeight;
        }
    }
}