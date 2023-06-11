//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxQuadMesh : SxMesh
    {
        public SxQuadMesh()
        {
            Build(Color.white);
        }

        public SxQuadMesh(Color color)
        {
            Build(color);
        }
        
        void Build(Color color)
        {
         
            var vertices = new SxMeshVertex[]
            {
                new(new Vector3(-1, -1, 0), color, new Vector2(0, 0)),
                new(new Vector3(1, -1, 0), color, new Vector2(1, 0)),
                new(new Vector3(1, 1, 0), color, new Vector2(1, 1)),
                new(new Vector3(-1, 1, 0), color, new Vector2(0, 1))
            };

            CreateSection(0, GL.QUADS, vertices);   
        }
    }

}