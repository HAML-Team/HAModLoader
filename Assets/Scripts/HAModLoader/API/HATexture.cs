using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HAModLoaderAPI
{
    public class HATexture
    {
        public string SourcePath { get; private set; }
        private Texture2D _unityTex;
        public HATexture(string path) => SourcePath = path;

        public static HATexture FromMod(Assembly assembly, string fileName)
        {
            string path = Image.ResourceExtractor?.Invoke(assembly, fileName);
            return new HATexture(path);
        }
        internal void SetLoadedTexture(Texture2D tex) => _unityTex = tex;

        public Texture2D ToUnity()
        {
            if (_unityTex == null && !string.IsNullOrEmpty(SourcePath))
            {
                if (System.IO.File.Exists(SourcePath))
                {
                    byte[] data = System.IO.File.ReadAllBytes(SourcePath);
                    _unityTex = new Texture2D(2, 2);
                    ImageConversion.LoadImage(_unityTex, data);
                }
                else
                {
                    Debug.LogError($"[HAModLoader] Texture file not found: {SourcePath}");
                }
            }
            return _unityTex;
        }

        public void Unload()
        {
            if (_unityTex != null)
            {
                UnityEngine.Object.Destroy(_unityTex);
                _unityTex = null;
            }
        }

        public static implicit operator Texture2D(HATexture ha) => ha?.ToUnity();
    }
}
