using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using FYP.Editor;
using System;

public class ProjectSynchronizerWindow : EditorWindow
{
    [SerializeField]
    [Tooltip("The package to be imported so that the folder can be synchronized")]
    private string packageFilePath = null;

    [SerializeField]
    private FolderReference rootFolder = null;

    [SerializeField]
    [Tooltip("The path of the output package")]
    private string targetFilePath = null;

    private SerializedObject so;
    private SerializedProperty packageFilePathProperty = null;
    private SerializedProperty rootFolderPathProperty = null;
    private SerializedProperty targetFilePathProperty = null;

    private const string SAVE_DATA_FILENAME = "SyncSaveData";

    [MenuItem("FYP/ProjectSynchronizer/Setup Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ProjectSynchronizerWindow));
    }

    [MenuItem("FYP/ProjectSynchronizer/Export Shared Folder")]
    public static void Export()
    {
        if (ValidateExportProperties())
        {
            var saveData = GetSavedData();
            ExportPackage(saveData.rootFileGuid, saveData.targetFilePath);
        }
    }



    [MenuItem("FYP/ProjectSynchronizer/Import Package")]
    public static void Import() 
    {
        var saveData = GetSavedData();
        if (ValidateImportProperties()) 
        {
            ImportPackage(saveData.packageFilePath);
        }
    }
    [MenuItem("FYP/ProjectSynchronizer/Import Package and Export")]
    public static void ImportAndExport() 
    {
        Import();
        Export();
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        packageFilePathProperty = so.FindProperty(nameof(packageFilePath));
        rootFolderPathProperty = so.FindProperty(nameof(rootFolder));
        targetFilePathProperty = so.FindProperty(nameof(targetFilePath));
        ReadSavedData();
    }
    private void ReadSavedData()
    {
        SyncSaveData res = GetSavedData();
        if (res != null)
        {
            packageFilePath = res.packageFilePath;
            rootFolder.guid = res.rootFileGuid;
            targetFilePath = res.targetFilePath;
        }
        Repaint();
    }

    private static SyncSaveData GetSavedData()
    {
        return EditorPropertySaver.GetProperty<SyncSaveData>(SAVE_DATA_FILENAME);
    }

    private void OnGUI()
    {

        so.Update();
        using(var layout = new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) 
        {
            DrawImportOptions();
        }
        GUILayout.Space(20f);
        using (var layout = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            DrawExportOptions();
        }
        if (so.hasModifiedProperties) 
        {
            so.ApplyModifiedProperties();
            SaveData();
        }
    }

    private void DrawImportOptions()
    {
        GUILayout.Label("Import Options", EditorStyles.boldLabel);
        GUILayout.Space(15f);
        using (var layout = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(packageFilePathProperty);
            if (GUILayout.Button("...", GUILayout.Width(25f)))
            {
                var path = EditorUtility.OpenFilePanel("Open Unity package", Application.dataPath, "unitypackage");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path != packageFilePath)
                    {
                        Undo.RecordObject(this, "Change Import Package");
                        packageFilePathProperty.stringValue = path;
                    }
                }
            }
        }
        GUILayout.Space(5f);
        if (!string.IsNullOrEmpty(packageFilePath))
        {
            if (GUILayout.Button("Synchronize"))
            {
                ImportPackage(packageFilePath);
            }
        }
    }
    private void DrawExportOptions()
    {
        GUILayout.Label("Export Options", EditorStyles.boldLabel);

        GUILayout.Space(20f);

        using (var layout = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.PropertyField(rootFolderPathProperty);
            GUILayout.Space(5f);
            EditorGUILayout.HelpBox("The folder which will be exported as a package which can be used for synchronization", MessageType.Info);
        }
        GUILayout.Space(10f);
        if (rootFolder != null && !string.IsNullOrEmpty(rootFolder.guid))
        {
            using (var layout = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(targetFilePathProperty);
                if (GUILayout.Button("...",GUILayout.Width(25f)))
                {
                    string folderPath, fileName;
                    if (File.Exists(targetFilePath))
                    {
                        folderPath = Path.GetDirectoryName(targetFilePath);
                        fileName = Path.GetFileName(targetFilePath);
                    }
                    else
                    {
                        folderPath = Application.dataPath;
                        fileName = null;
                    }
                    var path = EditorUtility.SaveFilePanel("Set Package Location", folderPath, fileName, "unitypackage");
                    if (!string.IsNullOrEmpty(path))
                    {
                        targetFilePathProperty.stringValue = path;
                    }
                }
            }
            if (!string.IsNullOrEmpty(targetFilePath))
            {
                if (GUILayout.Button("Export Package"))
                {
                    ExportPackage(rootFolder.guid,targetFilePath);
                }
            }
        }

    }

    private static void ExportPackage(string rootFolderGuid,string targetFilePath)
    {
        targetFilePath = Path.ChangeExtension(targetFilePath, "unitypackage");
        AssetDatabase.ExportPackage(AssetDatabase.GUIDToAssetPath(rootFolderGuid), targetFilePath, ExportPackageOptions.Recurse);
        Debug.Log("Exported the package successfully");
    }
    private static void ImportPackage(string packageFilePath)
    {
        AssetDatabase.ImportPackage(packageFilePath, false);
    }
    private static bool ValidateExportProperties()
    {
        var saveData = GetSavedData();
        return !string.IsNullOrWhiteSpace(saveData.rootFileGuid) && !string.IsNullOrWhiteSpace(saveData.targetFilePath);
    }
    private static bool ValidateImportProperties()
    {
        var saveData = GetSavedData();
        return !string.IsNullOrWhiteSpace(saveData.packageFilePath);
    }
    private void SaveData()
    {
        var saveData = new SyncSaveData() { packageFilePath = packageFilePath , rootFileGuid = rootFolder.guid,targetFilePath = targetFilePath};
        EditorPropertySaver.SetProperty(SAVE_DATA_FILENAME, saveData);
    }

    private class SyncSaveData 
    {
        public string packageFilePath = "";
        public string rootFileGuid = "";
        public string targetFilePath = "";
    }
}
