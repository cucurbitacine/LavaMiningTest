using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Player;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Tools
{
    /// <summary>
    /// Tutorial manager which determine target sources or spots by goal resource 
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public bool active = true;
        
        [Space]
        public ResourceProfile goal = null;
        public List<DropperBehaviour> droppers = new List<DropperBehaviour>();

        [Space]
        public LinesController lines = null;
        public PlayerController player = null;
        
        private const int NumberMaxRecursion = 128;
        
        private List<DropperBehaviour> _targetDroppers = new List<DropperBehaviour>();

        #region Static

        /// <summary>
        /// Get droppers which drops goal resource
        /// </summary>
        /// <param name="droppers"></param>
        /// <param name="goal"></param>
        /// <param name="result"></param>
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
        
        /// <summary>
        /// Get droppers which can be used right now
        /// </summary>
        /// <param name="droppers"></param>
        /// <param name="inventory"></param>
        /// <param name="result"></param>
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

        /// <summary>
        /// Get list of required resources for droppers
        /// </summary>
        /// <param name="droppers"></param>
        /// <param name="result"></param>
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

        /// <summary>
        /// Get Target Dropper by goal resource
        /// </summary>
        /// <param name="droppers"></param>
        /// <param name="goal"></param>
        /// <param name="inventory"></param>
        /// <param name="result"></param>
        /// <param name="indexRecursion"></param>
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

        #endregion

        public void SwitchMode(bool value)
        {
            active = value;

            lines.SwitchMode(value);
        }
        
        private void Update()
        {
            if (active)
            {
                // get targets
                _targetDroppers.Clear();
                GetTargetDropper(droppers, goal, player.inventory, ref _targetDroppers);

                var targets = _targetDroppers
                    .Select(d => d.transform.position)
                    .Select(v => Vector3.ProjectOnPlane(v, Vector3.up))
                    .ToArray();

                var origin = Vector3.ProjectOnPlane(player.transform.position, Vector3.up);
             
                // draw lines
                lines.Point(origin, targets);
            }
        }
    }
}
