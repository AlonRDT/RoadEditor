using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataLoader
    {
        /// <summary>
        /// try to load asset from resources and print error if not found
        /// </summary>
        /// <param name="address">address where asset should be, with asset name inside</param>
        /// <returns>target asset if exists, otherwise null</returns>
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
        /// read text written in file from given address
        /// </summary>
        /// <param name="address">fulll name of the file</param>
        /// <returns>text inside target file, null if does not exist</returns>
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
