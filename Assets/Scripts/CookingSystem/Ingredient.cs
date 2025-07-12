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

        // Sadece klasörler için çalýþsýn
        if (AssetDatabase.IsValidFolder(path))
        {
            // Klasör yolu buraya eþitse simgeyi göster
            if (path == "Assets/CookingSystem/ScriptableObjects/Ingredients")
            {
                // PNG simge yolu (örnek olarak 'carrot.png' kullanýldý)
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/ingredient_png/carrot.png");

                if (icon != null)
                {
                    GUI.DrawTexture(new Rect(rect.x, rect.y, 16, 16), icon);
                }
            }
        }
    }
}

