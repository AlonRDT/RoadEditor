using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataLoader
    {
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
