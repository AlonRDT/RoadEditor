using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataSaver
    {
        /// <summary>
        /// Shows text on the left side of the mouse
        /// </summary>
        /// <param name="text">The point the ray cast hit the object is saved here</param>
        /// <returns>returns null when off terrain, if terrain square has an object on it returns the object otherwise returns the terrain</returns>
        public static void SaveTextToFile(string fullPath, string fileName, string text)
        {
            if(File.Exists(fullPath + fileName) == false)
            {
                Directory.CreateDirectory(fullPath);
                File.Create(fullPath + fileName);
            }

            File.WriteAllText(fullPath + fileName, text);
        }
    }
}
