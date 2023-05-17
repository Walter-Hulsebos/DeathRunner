// Created by Jake Carter - 2023/03/15. Using Unity 2021.3.20f1
// Modification is allowed. Crediting is required.
// Purpose: Searches materials by shader.

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SearchMaterialByShader : EditorWindow
{
    private const Int32 materialsPerPage = 10;
    private const Int32 buttonsPerRow = 10;
    private readonly Int32 heightFindMaterialsBtnInital = 100;
    private readonly Int32 heightFindMaterialsBtnResults = 25;
    private readonly Int32 heightSelectedShaderPickerInitial = 50;
    private readonly Int32 heightSelectedShaderPickerResults = 25;
    private GUIStyle bgColor;
    private Int32 currentPage;
    private String currentPageTxt;
    private Int32 endIndex;
    private readonly List<Material> foundMaterials = new();
    private Int32 heightFindMaterialsBtn = 100;
    private Int32 heightSelectedShaderPicker = 50;
    private List<Editor> materialPreviews = new();
    private Int32 pageCount;
    private Vector2 scrollPos;
    private Shader selectedShader;
    private Boolean showResults;
    private Int32 startIndex;

    private void OnGUI()
    {
        //Main Buttons
        GUILayout.Space(25f);
        GUILayout.Label("Target Shader:");
        selectedShader = EditorGUILayout.ObjectField(selectedShader, typeof(Shader), false,
            GUILayout.Height(heightSelectedShaderPicker)) as Shader;
        if (GUILayout.Button("Find Materials", GUILayout.Height(heightFindMaterialsBtn))) FindMaterials();

        //Results Panel
        if (showResults)
        {
            //Selection Buttons
            if (GUILayout.Button("Select All - On Page")) SelectAllMaterialsOnPage();
            if (GUILayout.Button("Select All - All Pages")) SelectAllMaterials();

            //Page Navigation
            GUILayout.Space(10f);
            GUILayout.Label(currentPageTxt, GUILayout.Height(20));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            for (Int32 i = 0; i < pageCount; i++)
            {
                if (i % buttonsPerRow == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                }

                GUI.enabled = i != currentPage;
                if (GUILayout.Button((i + 1).ToString(), GUILayout.ExpandWidth(true), GUILayout.Height(20)))
                {
                    currentPage = i;
                    MenuChangeCache();
                }

                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Found Materials List
            GUILayout.Label($"Found Materials: {foundMaterials.Count}");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width),
                GUILayout.ExpandHeight(true));
            for (Int32 i = startIndex; i < endIndex; i++)
            {
                Material mat = foundMaterials[i];
                if (mat == null)
                {
                    MenuChangeCache();
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                materialPreviews[i].OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), bgColor);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(mat), GUILayout.Height(20));
                if (GUILayout.Button(mat.name, GUILayout.Height(80))) SelectMaterial(mat);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }

            EditorGUILayout.EndScrollView();
        }

        //Credits 
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Created by Jake Carter", EditorStyles.linkLabel, GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false))) Application.OpenURL("https://jcfolio.weebly.com/");
        GUILayout.EndHorizontal();
    }

    private void CreateGUI()
    {
        bgColor = new GUIStyle();
        selectedShader = Shader.Find("Standard");
        MenuChangeCache();
    }

    [MenuItem("Tools/Search Material By Shader", false, 22)]
    public static void ShowWindow()
    {
        SearchMaterialByShader window = GetWindow<SearchMaterialByShader>(false, "Search Material By Shader");
        Vector2 windowSize = new(700, 580);
        Rect windowRect = new(Screen.width * 0.5f, Screen.height * 0.5f, windowSize.x, windowSize.y);
        window.position = windowRect;
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    private void MenuChangeCache()
    {
        if (showResults)
        {
            //Page Navigation
            startIndex = currentPage * materialsPerPage;
            endIndex = Mathf.Min(startIndex + materialsPerPage, foundMaterials.Count);
            pageCount = Mathf.CeilToInt((Single)foundMaterials.Count / materialsPerPage);
            currentPageTxt = $"Current Page: {currentPage + 1}/{pageCount}";

            //Material Previews
            materialPreviews.Clear();
            materialPreviews = new Editor[foundMaterials.Count].ToList();
            for (Int32 i = startIndex; i < endIndex; i++)
            {
                Material mat = foundMaterials[i];
                Editor materialPreviewObjectEditor = Editor.CreateEditor(mat);
                materialPreviews[i] = materialPreviewObjectEditor;
            }

            //GUI Formatting
            heightSelectedShaderPicker = heightSelectedShaderPickerResults;
            heightFindMaterialsBtn = heightFindMaterialsBtnResults;
        }
        else
        {
            //GUI Formatting
            heightSelectedShaderPicker = heightSelectedShaderPickerInitial;
            heightFindMaterialsBtn = heightFindMaterialsBtnInital;
        }
    }

    private void FindMaterials()
    {
        FindMaterialsWithShader(selectedShader.name);
        showResults = true;
        currentPage = 0;
        MenuChangeCache();
    }

    private static void SelectMaterial(Material mat)
    {
        Selection.activeObject = mat;
        EditorGUIUtility.PingObject(mat);
    }

    private void SelectAllMaterials()
    {
        Selection.objects = foundMaterials.ToArray();
    }

    private void SelectAllMaterialsOnPage()
    {
        List<Material> materialsOnPage = foundMaterials.GetRange(startIndex, endIndex - startIndex);
        Selection.objects = materialsOnPage.ToArray();
    }

    private void FindMaterialsWithShader(String shaderName)
    {
        Int32 count = 0;
        foundMaterials.Clear();
        String[] allMaterialPaths = AssetDatabase.FindAssets("t:Material");
        foreach (String path in allMaterialPaths)
        {
            String assetPath = AssetDatabase.GUIDToAssetPath(path);
            if (assetPath.StartsWith("Assets/"))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (mat != null && mat.shader != null && !String.IsNullOrEmpty(mat.shader.name))
                    if (mat.shader.name.Equals(shaderName, StringComparison.OrdinalIgnoreCase))
                    {
                        foundMaterials.Add(mat);
                        count++;
                    }
            }
        }

        foundMaterials.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
    }
}

#endif