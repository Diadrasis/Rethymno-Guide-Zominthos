using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spinaloga
{
    public static class TextureExtensions
    {
        public static Sprite ToSprite(this Texture2D tex)
        {
            if (tex == null) return null;
            //Debug.Log(tex.width + "x" + tex.height);    
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
}
