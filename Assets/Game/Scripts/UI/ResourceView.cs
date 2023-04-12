using Game.Scripts.ResourceSystem.Profiles;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class ResourceView : MonoBehaviour
    {
        [Min(0)]
        public int amount = 0;
        public ResourceProfile profile = null;

        [Space]
        public UnityEvent<int> onAmountChanged = new UnityEvent<int>();

        [Space]
        public Image iconImage = null;
        public TextMeshProUGUI amountText = null;

        public LayoutElement layoutElement { get; private set; }
        public CanvasGroup canvasGroup { get; private set; }
        
        public void Initiate(ResourceProfile initProfile, int initAmount = 0)
        {
            profile = initProfile;
            
            if (iconImage != null && profile.icon != null) iconImage.sprite = initProfile.icon;

            SetAmount(initAmount);
        }

        public int SetAmount(int value)
        {
            value = Mathf.Max(value, 0);

            if (value != amount)
            {
                amount = value;
                
                onAmountChanged.Invoke(amount);
            }

            return amount;
        }
        
        public int Increase(int value = 1)
        {
            return SetAmount(amount + value);
        }
        
        public int Decrease(int value = 1)
        {
            return SetAmount(amount - value);
        }

        public void UpdateView(int value)
        {
            if (amountText != null) amountText.text = value.ToString("D");
        }

        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            onAmountChanged.AddListener(UpdateView);
        }
        
        private void OnDisable()
        {
            onAmountChanged.RemoveListener(UpdateView);
        }

        private void OnValidate()
        {
            UpdateView(amount);
        }
    }
}