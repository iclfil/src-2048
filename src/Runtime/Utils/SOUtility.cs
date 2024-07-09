#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class SOUtility
{
#if UNITY_EDITOR
    public static void SaveAsset(ScriptableObject so, string path)
    {
        // Проверка, что путь не пустой и объект не null
        if (so == null || string.IsNullOrEmpty(path))
        {
            Debug.LogError("ScriptableObject or path is null or empty");
            return;
        }

        // Создание ассета
        AssetDatabase.CreateAsset(so, "Assets/" + path + ".asset");

        // Сохранение ассетов и обновление базы данных ассетов
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}