using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public class PlayerInventoryStorage : MonoBehaviour
    {
        public InventoryController inventory = null;


        public string fileName = "Save.txt";
        public string filePath => Path.Combine(Application.persistentDataPath, fileName);

        public ResourceDatabase database = null;
        
        public async Task Load()
        {
            Debug.Log("Loading Player Inventory...");

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                try
                {
                    var fileContent = await fileInfo.OpenText().ReadToEndAsync();
                    var data = JsonUtility.FromJson<InventoryData>(fileContent);
                
                    foreach (var stack in data.stacks)
                    {
                        if (Guid.TryParse(stack.guid, out var guid))
                        {
                            var profile = database.profiles.FirstOrDefault(p => p.Guid == guid);

                            if (profile != null)
                            {
                                for (var i = 0; i < stack.amount; i++)
                                {
                                    var res = profile.GetResource();
                                    res.gameObject.SetActive(false);
                                    inventory.Put(res);
                                }
                            }
                        }
                    }
                    
                    Debug.Log("Player Inventory Loaded!");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Data Player Inventory was not Loaded! {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Data Player Inventory was not Found!");
            }
        }

        public async Task Save()
        {
            Debug.Log("Saving Player Inventory...");

            try
            {
                var data = InventoryData.GetData(inventory);
                var json = JsonUtility.ToJson(data);
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
            
                Debug.Log("Player Inventory Saved!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Player Inventory was not Saved! {e.Message}");
            }
        }

        private async void OnEnable()
        {
            await Load();
        }

        private async void OnDisable()
        {
            await Save();
        }
    }

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