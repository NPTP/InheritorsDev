using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace ShadedTechnology.GrassPhysics
{
    public class UrpApplyWindow : EditorWindow
    {
        static ListRequest request;
        static readonly string templateUrpFilePath = "Assets/GrassPhysics/URP/Shaders/Template/WavingGrassPasses.hlsl";

        private void CopyUrpTemplatFile(string destPath)
        {
            if (File.Exists(templateUrpFilePath) && File.Exists(destPath))
            {
                File.Copy(templateUrpFilePath, destPath, true);
            }
        }

        private void HandleRegisryURP(UnityEditor.PackageManager.PackageInfo package)
        {
            if (GUILayout.Button("Apply Grass Shaders to URP"))
            {
                string targetPath = "Packages/" + Path.GetFileName(package.resolvedPath);
                AssetsManager.CopyDirectory(package.resolvedPath, targetPath);
                string destFilePath = targetPath + "/Shaders/Terrain/WavingGrassPasses.hlsl";
                CopyUrpTemplatFile(destFilePath);
                AssetDatabase.Refresh();
            }
        }

        private void HandleEmbeddedURP(UnityEditor.PackageManager.PackageInfo package)
        {
            EditorGUILayout.LabelField("URP Grass is applied.");
            if (GUILayout.Button("Reapply Grass Shaders to URP"))
            {
                string destFilePath = package.resolvedPath + "/Shaders/Terrain/WavingGrassPasses.hlsl";
                CopyUrpTemplatFile(destFilePath);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Reset URP"))
            {
                string targetPath = "Library/PackageCache/" + Path.GetFileName(package.resolvedPath);
                AssetsManager.CopyDirectory(package.resolvedPath, targetPath);
                Directory.Delete(package.resolvedPath, true);
                AssetDatabase.Refresh();
            }
        }

        private void HandleURP()
        {
            if (request == null)
            {
                request = Client.List();
                while (!request.IsCompleted) ;
            }
            foreach (UnityEditor.PackageManager.PackageInfo package in request.Result)
            {
                if (package.name.Contains("render-pipelines.universal"))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("URP Grass", EditorStyles.boldLabel);
                    if (package.source == PackageSource.Registry)
                    {
                        HandleRegisryURP(package);
                    }
                    if (package.source == PackageSource.Embedded)
                    {
                        HandleEmbeddedURP(package);
                    }
                }
            }
        }

        [MenuItem("Tools/Grass Physics/Grass URP Settings")]
        public static void ShowWindow()
        {
            UrpApplyWindow window = GetWindow<UrpApplyWindow>("Grass URP Settings");
            string path = AssetsManager.GetGrassAssetPath()
                + (EditorGUIUtility.isProSkin ? "/Icons/GrassLightIco.png" : "/Icons/GrassDarkIco.png");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            window.titleContent.image = texture;
            window.titleContent.text = "Grass URP Settings";
        }

        private void OnGUI()
        {
            HandleURP();
        }
    }
}