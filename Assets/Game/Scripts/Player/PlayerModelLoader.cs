using UnityEngine;

namespace Game.Scripts.Player
{
    /*
     * PlayerModelLoader can be used later by some scene loader to load the model you need
     */
    
    [RequireComponent(typeof(PlayerController))]
    public class PlayerModelLoader : MonoBehaviour
    {
        public GameObject currentModel = null;

        [Space]
        public bool loadOnEnable = true;
        public int numberModelDefault = 0;
        public GameObject[] modelPrefabs = null;

        private PlayerController _player = null;

        public void LoadModel(int number)
        {
            if (number < 0 || modelPrefabs.Length <= number) number = numberModelDefault;
            
            if (currentModel != null) Destroy(currentModel);

            currentModel = Instantiate(modelPrefabs[number], _player.transform, false);
        }
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (loadOnEnable) LoadModel(numberModelDefault);
        }

        private void OnValidate()
        {
            numberModelDefault = Mathf.Clamp(numberModelDefault, 0, modelPrefabs.Length - 1);
        }
    }
}