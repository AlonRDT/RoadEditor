using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities.API.Data
{
    public static class DataSaver
    {
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
