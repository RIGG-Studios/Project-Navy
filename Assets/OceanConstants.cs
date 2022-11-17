using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OceanConstants 
{
    public static class OceanTiles
    {
        public static int LOD0_MESH_RESOLUTION = 25;
        public static int LOD1_MESH_RESOLUTION = 20;
        public static int LOD2_MESH_RESOLUTION = 10;
        public static int LOD3_MESH_RESOLUTION = 8;

        public static float LOD0_MESH_SIZE = 6.0f;
        public static float LOD1_MESH_SIZE = 6.0f;
        public static float LOD2_MESH_SIZE = 6.0f;
        public static float LOD3_MESH_SIZE = 6.0f;

        public static int GetMeshLODMeshResolution(int i)
        {
            switch (i)
            {
                case 0:
                    return LOD0_MESH_RESOLUTION;
                
                case 1:
                    return LOD1_MESH_RESOLUTION;
                
                case 2:
                    return LOD2_MESH_RESOLUTION;
                
                case 3:
                    return LOD3_MESH_RESOLUTION;
            }

            return LOD0_MESH_RESOLUTION;
        }
        
        public static float GetMeshLODSize(int i)
        {
            switch (i)
            {
                case 0:
                    return LOD0_MESH_SIZE;
                
                case 1:
                    return LOD1_MESH_SIZE;
                
                case 2:
                    return LOD2_MESH_SIZE;
                
                case 3:
                    return LOD3_MESH_SIZE;
            }

            return LOD0_MESH_RESOLUTION;
        }
    }
}
