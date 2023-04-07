using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class PlayerSendsResourceToSpotEffect : MonoBehaviour
    {
        public float durationEffect = 1;
        
        [Space]
        public ResourceDistributor distributor = null;
        
        public void AnimationSend(ResourceEntity resource, ResourceSpotEntity spot)
        {
            if (resource == null || spot == null) return;
            
            StartCoroutine(_Animation(resource, spot));
        }

        private IEnumerator _Animation(ResourceEntity resource, ResourceSpotEntity spot)
        {
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
            if (distributor != null) distributor.onDistributed.AddListener(AnimationSend);
        }
        
        private void OnDisable()
        {
            if (distributor != null) distributor.onDistributed.RemoveListener(AnimationSend);
        }
    }
}