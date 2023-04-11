using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public class ArrowController : MonoBehaviour
    {
        public Vector3 offset = Vector3.up;

        public LineRenderer linePrefab = null;
        
        private readonly List<LineRenderer> _lines = new List<LineRenderer>();

        public void Point(Vector3 origin, params Vector3[] targets)
        {
            AddLine(targets.Length - _lines.Count);

            for (var i = 0; i < _lines.Count; i++)
            {
                var line = _lines[i];
                line.enabled = i < targets.Length;
                
                if (line.enabled)
                {
                    line.SetPosition(0, origin + offset);
                    line.SetPosition(1, targets[i] + offset);
                }
            }
        }

        private void AddLine(int count = 1)
        {
            if (count <= 0) return;

            var line = Instantiate(linePrefab, transform, false);
            _lines.Add(line);

            line.useWorldSpace = true;
        }
    }
}