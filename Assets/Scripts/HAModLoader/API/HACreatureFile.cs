using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAModLoaderAPI
{
    public class HACreatureFile
    {
        public string FileName { get; private set; }

        public HACreatureFile(string fileName)
        {
            FileName = fileName;
        }

        public static implicit operator HACreatureFile(string fileName) => new HACreatureFile(fileName);
    }
}