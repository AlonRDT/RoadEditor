using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.API.Map;

namespace Scene.Roads.API
{
    public class JunctionLogic : MonoBehaviour
    {
        private int[] m_SquareIndex;
        public int[] SquareIndex => m_SquareIndex;

        /// <summary>
        /// Sets junction data and transform after instantiate
        /// <param name="index"> index of square on the terrain </param>
        /// </summary>
        public void Initialize(int[] index)
        {
            m_SquareIndex = index;
            transform.position = MapLocation.GetWorldSquareCenterFromSquareIndex(index);
        }
    }
}
