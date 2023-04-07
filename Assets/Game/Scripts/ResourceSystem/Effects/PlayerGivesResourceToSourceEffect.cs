using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Effects
{
    public class PlayerGivesResourceToSourceEffect : MonoBehaviour
    {
        public float durationEffect = 1;
        
        [Space]
        public ResourceDistributor distributor = null;
        
        public void AnimationDistribute(ResourceEntity resource, ResourceSpotEntity spot)
        {
            if (resource == null || spot == null) return;
            
            StartCoroutine(_AnimationDistribute(resource, spot));
        }

        private IEnumerator _AnimationDistribute(ResourceEntity resource, ResourceSpotEntity spot)
        {
            spot.inventory.Put(resource);
            
            var trg = resource.transform;

            trg.position = transform.position + Vector3.up * 2f;
            trg.gameObject.SetActive(true);
            
            trg.DOJump(spot.transform.position, 1, 2, durationEffect);
            trg.DOShakeScale(durationEffect);

            yield return new WaitForSeconds(durationEffect);

            trg.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (distributor != null) distributor.onDistributed.AddListener(AnimationDistribute);
        }
        
        private void OnDisable()
        {
            if (distributor != null) distributor.onDistributed.RemoveListener(AnimationDistribute);
        }
    }
}