//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples
{
    public class PlaceableMarker : MonoBehaviour
    {
        public string markerName = "MyMarker";

        private void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

        private void OnDrawGizmos()
        {
            DrawGizmo(false);
        }

        private void DrawGizmo(bool selected)
        {
            // Draw the wireframe
            Gizmos.color = selected ? Color.red : Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}
