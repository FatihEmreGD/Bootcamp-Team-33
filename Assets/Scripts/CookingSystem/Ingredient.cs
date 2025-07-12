using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FolderIconSetter
{
    static FolderIconSetter()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    static void OnProjectWindowItemGUI(string guid, Rect rect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);

        // Sadece klas�rler i�in �al��s�n
        if (AssetDatabase.IsValidFolder(path))
        {
            // Klas�r yolu buraya e�itse simgeyi g�ster
            if (path == "Assets/CookingSystem/ScriptableObjects/Ingredients")
            {
                // PNG simge yolu (�rnek olarak 'carrot.png' kullan�ld�)
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ingredient_png/carrot.png");

                if (icon != null)
                {
                    GUI.DrawTexture(new Rect(rect.x, rect.y, 16, 16), icon);
                }
            }
        }
    }
}

