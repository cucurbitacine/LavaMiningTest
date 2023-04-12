using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class JoystickInputView : MonoBehaviour
    {
        [Space]
        [Range(0f, 1f)]
        public float alphaActive = 0.6f;
        [Range(0f, 1f)]
        public float alphaInactive = 0.0f;
        
        [Space]
        public PlayerInput input;
        public Animator animator;
        public CanvasGroup group;

        private RectTransform _rect = null;

        private Vector3 _touchBegan;
        
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");

        private void Show(Vector3 screenPoint)
        {
            if (group != null) group.alpha = alphaActive;

            _rect.anchoredPosition = screenPoint;

            _touchBegan = screenPoint;
        }

        private void Hide(Vector3 screenPoint)
        {
            if (group != null) group.alpha = alphaInactive;
        }

        private void UpdateAnimator(Vector3 screenPoint)
        {
            if (animator == null) return;

            var dir = (screenPoint - _touchBegan).normalized;
            
            animator.SetFloat(Horizontal, dir.x);
            animator.SetFloat(Vertical, dir.y);
        }
        
        private void OnEnable()
        {
            if (input != null)
            {
                input.onTouchBegan.AddListener(Show);
                input.onTouchMoved.AddListener(UpdateAnimator);
                input.onTouchEndOrCancel.AddListener(Hide);
            }

            if (_rect == null) _rect = GetComponent<RectTransform>();

            Hide(Vector3.zero);
        }

        private void OnDisable()
        {
            if (input != null)
            {
                input.onTouchBegan.RemoveListener(Show);
                input.onTouchMoved.RemoveListener(UpdateAnimator);
                input.onTouchEndOrCancel.RemoveListener(Hide);
            }
        }
    }
}
