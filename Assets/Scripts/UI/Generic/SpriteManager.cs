using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [System.Serializable]
    public struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }
    // These fields are functionally equivalent, but they logically group icons
    public NamedSprite[] namedImages1; // icons and misc
    public NamedSprite[] playerImages;
    public NamedSprite[] wifeImages;
    public NamedSprite[] sonImages;
    public NamedSprite[] otherFaces;
    public NamedSprite[] seasonImages;

    private Dictionary<string, Sprite> stringToSpriteMap = new Dictionary<string, Sprite>();
    private static SpriteManager mSpriteManagerSingleton;

    void Awake()
    {
        if (stringToSpriteMap.Count == 0)
        {
            Setup();
        }
    }

    private void Setup()
    {
        foreach (NamedSprite[] namedSpriteArray in new NamedSprite[][] {
            namedImages1,
            playerImages,
            wifeImages,
            sonImages,
            otherFaces,
            seasonImages
        })
        {
            foreach (NamedSprite namedSprite in namedSpriteArray)
            {
                if (stringToSpriteMap.ContainsKey(namedSprite.name))
                {
                    Debug.LogWarning("Multiple sprites with name: " + namedSprite.name);
                }
                stringToSpriteMap.Add(namedSprite.name, namedSprite.sprite);
            }
        }
    }

    public static Sprite GetSprite(string imageName)
    {
        if (mSpriteManagerSingleton == null)
        {
            mSpriteManagerSingleton = GameObject.FindObjectsOfType<SpriteManager>()[0];
        }
        return mSpriteManagerSingleton.GetSpriteInternal(imageName);
    }

    // To be called by the static GetSprite
    public Sprite GetSpriteInternal(string imageName)
    {
        if (imageName == "" || imageName == "None")
        {
            return null;
        }
        // Call Setup here in case awake hasnt yet happened
        if (stringToSpriteMap.Count == 0)
        {
            Setup();
        }
        if (stringToSpriteMap.ContainsKey(imageName))
        {
            return stringToSpriteMap[imageName];
        }
        Debug.LogWarning("WARNING(SpriteManager): No sprite found for name: " + imageName);
        return null;
    }
}