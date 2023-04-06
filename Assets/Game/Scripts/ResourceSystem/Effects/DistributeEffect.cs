using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Effects
{
    public class DistributeEffect : MonoBehaviour
    {
        public ResourceDistributor distributor = null;
        
        public void AnimationDistribute(ResourceEntity resource, ResourceSpotEntity spot)
        {
            StartCoroutine(_AnimationDistribute(resource, spot));
        }

        private IEnumerator _AnimationDistribute(ResourceEntity resource, ResourceSpotEntity spot)
        {
            var trg =resource.transform;

            trg.position = transform.position + Vector3.up * 2f;
            trg.gameObject.SetActive(true);
            
            var duration = 1f;
            trg.DOJump(spot.transform.position, 1, 2, duration);
            trg.DOShakeScale(duration);

            yield return new WaitForSeconds(duration);
            
            spot.inventory.Put(resource);
            
            trg.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            distributor.onDistributed.AddListener(AnimationDistribute);
        }
        
        private void OnDisable()
        {
            distributor.onDistributed.RemoveListener(AnimationDistribute);
        }
    }
}