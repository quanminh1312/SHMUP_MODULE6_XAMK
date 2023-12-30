using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FontMaker : EditorWindow
{
    public string letters = "";
    public Object fontObject = null;
    public Object textureObject = null;

    [MenuItem("SHMUP/FontCreator")]
    public static void ShowWindow()
    {
        GetWindow<FontMaker>("Font Creator");
    }

    private void OnGUI()
    {
        textureObject = EditorGUILayout.ObjectField("Sprite Source",textureObject,typeof(Texture2D),false);
        GUILayout.Space(10);
        fontObject = EditorGUILayout.ObjectField("Font Destination", fontObject, typeof(Font), false);
        GUILayout.Space(10);
        letters = EditorGUILayout.TextField("Characters", letters);
        GUILayout.Space(10);
        if (GUILayout.Button("Create") && textureObject != null && fontObject != null)
        {
            Font font = fontObject as Font;
            Texture2D texture = textureObject as Texture2D;
            if (font != null && texture != null) 
            {
                Debug.Log("creating Font");
                Sprite[] sprites = Resources.LoadAll<Sprite>(texture.name);
                CharacterInfo[] characterInfo = new CharacterInfo[sprites.Length];
                float width = texture.width;
                float height = texture.height;
                int i = 0;
                foreach(Sprite sprite in sprites)
                {
                    float charWidth = sprite.rect.width;
                    float charHeight = sprite.rect.height;
                    float charX = sprite.rect.x;
                    float charY = sprite.rect.y;
                    int charAdvance = (int)charWidth + 1;

                    CharacterInfo charInfo = new CharacterInfo();

                    charInfo.index = letters[i];
                    charInfo.uvBottomLeft = new Vector2(charX / width,charY / height);
                    charInfo.uvBottomRight = new Vector2((charX + charWidth) / width, charY / height);
                    charInfo.uvTopLeft = new Vector2(charX / width, (charY + charHeight) / height);
                    charInfo.uvTopRight = new Vector2((charX + charWidth) / width, (charY + charHeight) / height);

                    charInfo.minX = 0;
                    charInfo.maxX = (int)charWidth;
                    charInfo.minY = -(int)charHeight;
                    charInfo.maxY = 0;
                    charInfo.advance = charAdvance;

                    characterInfo[i] = charInfo;
                    i++;
                }
                font.characterInfo = characterInfo;
            }
        }


    }
}
