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

        public InventoryData data = default;
        public ResourceDatabase database = null;
        
        public async Task Load()
        {
            Debug.Log("Loading");

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                var fileContent = await fileInfo.OpenText().ReadToEndAsync();

                Debug.Log(fileContent);

                data = JsonUtility.FromJson<InventoryData>(fileContent);
                
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
            }

            Debug.Log("Loaded");
        }

        public async Task Save()
        {
            Debug.Log("Saving");

            data = InventoryData.GetData(inventory);
            var json = JsonUtility.ToJson(data);
            Debug.Log(json);
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
            
            Debug.Log("Saved");
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