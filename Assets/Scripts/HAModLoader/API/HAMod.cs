using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace HAModLoaderAPI
{
    public interface HAMod
    {
        public string ModName { get; }
        public string ModAuthor { get; }
        public string ModVersion { get; }
        public Sprite ModLogo { get; }
        public string ModDescriptionShort => null;
        public void OnModLoad() { }
        public void OnEnterScene(Scene scene) { }
        public void OnCreate(GameObject obj) { }
        public void Update() { }
    }
}