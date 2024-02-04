using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.API.Map
{
    public static class MapLocation
    {
        private static int s_SquaresPerDimension = 200; // amount of squares in terrain
        private static int s_TerrainSize = 1000; // world size of terrain
        private static float s_SquareSize => (float)s_TerrainSize / s_SquaresPerDimension;

        public static Vector3 GetSquareCenterFromPosition(Vector3 position)
        {
            return GetWorldSquareCenterFromSquareIndex(GetSquareIndexFromPosition(position));
        }

        public static int[] GetSquareIndexFromPosition(Vector3 position)
        {
            int[] output = new int[2];

            output[0] = Mathf.CeilToInt((position.x - s_SquareSize / 2) / s_SquareSize);
            output[1] = Mathf.CeilToInt((position.z - s_SquareSize / 2) / s_SquareSize);

            return output;
        }

        public static Vector3 GetWorldSquareCenterFromSquareIndex(int[] squareIndex)
        {
            Vector3 output = new Vector3(squareIndex[0] * s_SquareSize, 0, squareIndex[1] * s_SquareSize);

            if (Terrain.activeTerrain != null)
            {
                float height = Terrain.activeTerrain.SampleHeight(output);
                output.y = height;
            }

            return output;
        }

        public static int[] MoveMapIndexTowardsTargetMapIndex(int[] mapIndex, int[] targetMapIndex)
        {
            int[] output = new int[2];

            if(Math.Abs(mapIndex[0] - targetMapIndex[0]) > Math.Abs(mapIndex[1] - targetMapIndex[1]))
            {
                output[0] = mapIndex[0] + Math.Sign(targetMapIndex[0] - mapIndex[0]);
                output[1] = mapIndex[1];
            }
            else
            {
                output[1] = mapIndex[1] + Math.Sign(targetMapIndex[1] - mapIndex[1]);
                output[0] = mapIndex[0];
            }

            return output;
        }
    }
}
