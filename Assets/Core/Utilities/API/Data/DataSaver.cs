using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataSaver
    {
        /// <summary>
        /// writes text into file, if does not exist creates it
        /// </summary>
        /// <param name="fullPath">directory where file is to be saved</param>
        /// <param name="fileName">name with extension of file</param>
        /// <param name="text">text to save into file</param>
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
