using Management.API;
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

        /// <summary>
        /// returns wether the input index is the index of the junction
        /// <param name="index"> index in question </param>
        /// </summary>
        public bool IsJuctionIndex(int[] index)
        {
            return m_SquareIndex[0] == index[0] && m_SquareIndex[1] == index[1];
        }
    }
}
