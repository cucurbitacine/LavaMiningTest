using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Player;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public class TutorialManager : MonoBehaviour
    {
        public ArrowController arrow = null;
        public PlayerController player = null;

        public ResourceProfile goal = null;
        public List<DropperBehaviour> droppers = new List<DropperBehaviour>();

        private const int NumberMaxRecursion = 128;
        
        private static void GetDroppersByResource(List<DropperBehaviour> droppers, ResourceProfile goal, ref List<DropperBehaviour> result)
        {
            result.Clear();

            foreach (var dropper in droppers)
            {
                if (dropper.GetProfile().dropResourceProfile == goal)
                {
                    result.Add(dropper);
                }
            }
        }
        
        private static void GetReadyDroppers(List<DropperBehaviour> droppers, InventoryController inventory, ref List<DropperBehaviour> result)
        {
            result.Clear();

            foreach (var dropper in droppers)
            {
                if (dropper is SourceBehaviour)
                {
                    result.Add(dropper);
                }
                else if (dropper is SpotBehaviour spot)
                {
                    var req = new List<ResourceStack>();
                    spot.FillRequired(ref req);

                    if (inventory.Contains(req))
                    {
                        result.Add(dropper);
                    }
                }
            }
        }

        private static void GetRequiredForDroppers(List<DropperBehaviour> droppers, ref List<ResourceStack> result)
        {
            result.Clear();
            
            foreach (var spot in droppers.OfType<SpotBehaviour>())
            {
                var req = new List<ResourceStack>();
                spot.FillRequired(ref req);
                result.AddRange(req);
            }
        }

        private static void GetTargetDropper(List<DropperBehaviour> droppers, ResourceProfile goal, InventoryController inventory, ref List<DropperBehaviour> result, int indexRecursion = 0)
        {
            if (indexRecursion > NumberMaxRecursion) return;

            var goalDroppers = new List<DropperBehaviour>();
            GetDroppersByResource(droppers, goal, ref goalDroppers);
            
            var readyDroppers = new List<DropperBehaviour>();
            GetReadyDroppers(goalDroppers, inventory, ref readyDroppers);

            if (readyDroppers.Count > 0)
            {
                result.AddRange(readyDroppers);
                return;
            }

            var required = new List<ResourceStack>();
            GetRequiredForDroppers(goalDroppers, ref required);
            required.RemoveAll(r => inventory.CountResource(r.profile) >= r.amount);

            foreach (var req in required)
            {
                GetTargetDropper(droppers, req.profile, inventory, ref result, indexRecursion + 1);
            }
        }

        public List<DropperBehaviour> drops = new List<DropperBehaviour>();
        
        private void Update()
        {
            drops.Clear();
             GetTargetDropper(droppers, goal, player.inventory, ref drops);

             var targets = drops
                 .Select(d => d.transform.position)
                 .Select(v => Vector3.ProjectOnPlane(v, Vector3.up))
                 .ToArray();

             var origin = Vector3.ProjectOnPlane(player.transform.position, Vector3.up);
             
            arrow.Point(origin, targets);
        }
    }
}
