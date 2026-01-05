using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace HAModLoaderAPI
{
    public static class Image
    {
        public static Func<Assembly, string, string> ResourceExtractor;

        public static Sprite ToSprite(Texture2D texture)
        {
            if (texture == null) return null;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static IEnumerator LoadRemoteCoroutine(string pathOrUrl, Action<HATexture> callback)
        {
            string uri = pathOrUrl.Contains("://") ? pathOrUrl : "file://" + pathOrUrl;
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[API] Remote Load Fail: {uwr.error}");
                    callback?.Invoke(null);
                }
                else
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                    HATexture wrapper = new HATexture(pathOrUrl);
                    wrapper.SetLoadedTexture(tex);
                    callback?.Invoke(wrapper);
                }
            }
        }
    }
}