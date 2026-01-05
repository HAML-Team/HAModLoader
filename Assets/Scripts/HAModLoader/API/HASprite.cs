using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HAModLoaderAPI
{
    public class HASprite
    {
        public HATexture TextureWrapper { get; private set; }
        private Sprite _unitySprite;

        public HASprite(string path) => TextureWrapper = new HATexture(path);
        public HASprite(HATexture tex) => TextureWrapper = tex;

        public static HASprite FromMod(Assembly assembly, string fileName) 
            => new HASprite(HATexture.FromMod(assembly, fileName));

        public Sprite ToUnity()
        {
            if (_unitySprite == null) {
                Texture2D tex = TextureWrapper?.ToUnity();
                if (tex != null) _unitySprite = Image.ToSprite(tex);
            }
            return _unitySprite;
        }

        public void Unload()
        {
            if (_unitySprite != null) UnityEngine.Object.Destroy(_unitySprite);
            TextureWrapper?.Unload();
            _unitySprite = null;
        }

        public static implicit operator Sprite(HASprite ha) => ha.ToUnity();
    }
}