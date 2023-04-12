using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Tools
{
    /// <summary>
    /// Data inventory loader/saver
    /// </summary>
    public class DataInventoryStorage : MonoBehaviour
    {
        public string inventoryName = "player_1";

        [Space]
        public bool autoSave = true;
        public float autoSaveTime = 10f;
        
        [Space]
        public InventoryController inventory = null;
        public ResourceDatabase database = null;
        
        private InventoryData _lastSavedData = default;
        private Coroutine _autoSaving = null;
        
        public string sceneName => SceneManager.GetActiveScene().name;
        public string dataName => "inventory";
        public string fileName => $"{inventoryName}-{sceneName}-{dataName}.txt";
        public string filePath => Path.Combine(Application.persistentDataPath, fileName);

        public async Task Load()
        {
            Debug.Log("Loading Inventory...");
            
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                try
                {
                    var fileContent = await fileInfo.OpenText().ReadToEndAsync();
                    var data = JsonUtility.FromJson<InventoryData>(fileContent);
                
                    foreach (var stack in data.stacks)
                    {
                        // search resource profile by its guid
                        if (Guid.TryParse(stack.guid, out var guid) && database.TryGetProfile(guid, out var profile))
                        {
                            // create and add required amount of resources 
                            for (var i = 0; i < stack.amount; i++)
                            {
                                var res = profile.GetResource();
                                res.gameObject.SetActive(false);
                                inventory.Put(res);
                            }
                        }
                    }
                    
                    Debug.Log("Inventory Loaded!");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Data Inventory was not Loaded!\n{e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Data Inventory was not Found!");
            }
        }

        public async Task Save()
        {
            Debug.Log("Saving Inventory...");

            try
            {
                // keep data and create json 
                _lastSavedData = InventoryData.GetData(inventory);
                var json = JsonUtility.ToJson(_lastSavedData);
                
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    var sw = new StreamWriter(filePath, false, Encoding.Default);
                    await sw.WriteAsync(json);
                    sw.Close();
                }
                else
                {
                    var sw = fileInfo.CreateText();
                    await sw.WriteAsync(json);
                    sw.Close();
                }
            
                Debug.Log("Inventory Saved!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Inventory was not Saved!\n{e.Message}");
            }
        }

        private IEnumerator _AutoSaving()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveTime);
                
                if (autoSave)
                {
                    Debug.Log("Autosaving...");
                    
                    // TODO add check current inventory data with last saved data
                    // if current inventory data is not same as last saved data - save!

                    var saveTask = Save();

                    yield return new WaitUntil(() => saveTask.IsCompleted);
                }
            }
        }
        
        private async void OnEnable()
        {
            if (autoSave)
            {
                if (_autoSaving != null) StopCoroutine(_autoSaving);
                _autoSaving = StartCoroutine(_AutoSaving());
            }
            
            await Load();
        }

        private async void OnDisable()
        {
            if (_autoSaving != null) StopCoroutine(_autoSaving);
            
            await Save();
        }
    }

    /// <summary>
    /// Serializable data of inventory
    /// </summary>
    [Serializable]
    public struct InventoryData
    {
        public ResourceStackData[] stacks;

        public static InventoryData GetData(InventoryController inventory)
        {
            var inventoryData = new InventoryData();
            
            inventoryData.stacks = new ResourceStackData[inventory.items.Count];

            var index = 0;
            foreach (var item in inventory.items)
            {
                var stackData = new ResourceStackData()
                {
                    guid = item.Key.Guid.ToString(),
                    amount = item.Value.Count,
                };

                inventoryData.stacks[index] = stackData;
                index++;
            }

            return inventoryData;
        }
    }

    [Serializable]
    public struct ResourceStackData
    {
        public string guid;
        public int amount;
    }
}