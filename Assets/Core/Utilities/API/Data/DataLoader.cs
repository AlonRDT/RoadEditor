using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataLoader
    {
        /// <summary>
        /// Shows text on the left side of the mouse
        /// </summary>
        /// <param name="text">The point the ray cast hit the object is saved here</param>
        /// <returns>returns null when off terrain, if terrain square has an object on it returns the object otherwise returns the terrain</returns>
        public static T LoadFromResources<T>(string address) where T : Object
        {
            T target = Resources.Load<T>(address);

            if (target == null)
            {
                string[] splitAddress = address.Split('/');
                string targetName = splitAddress[splitAddress.Length - 1];
                Debug.LogError("Couldnt find data of type: " + typeof(T).Name + ", name: " + targetName);
            }

            return target;
        }

        /// <summary>
        /// Shows text on the left side of the mouse
        /// </summary>
        /// <param name="text">The point the ray cast hit the object is saved here</param>
        /// <returns>returns null when off terrain, if terrain square has an object on it returns the object otherwise returns the terrain</returns>
        public static string ReadTextualFile(string address)
        {
            string output = null;

            if (File.Exists(address) == true)
            {
                output = File.ReadAllText(address);
            }
            else
            {
                Debug.LogError("tryed to read file: " + address + ", but file does not exist");
            }

            return output;
        }
    }
}
