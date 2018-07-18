using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
public class ReplaceFont : EditorWindow  
{
    private static ReplaceFont window = null;
 	private static List<string> prefafbPathList = new List<string>();

	private static Font targetFont;
	private static Font curFont;
	private static float fontSizeRatio = 1f;
	private static bool isUGUI = true;

	[MenuItem("Tools/替换字体")]
	public static void CSVCode()
	{
		if (window == null)
		window = EditorWindow.GetWindow(typeof(ReplaceFont)) as ReplaceFont;
		GetFiles(new DirectoryInfo(Application.dataPath), "*.prefab", ref prefafbPathList);
		window.titleContent = new GUIContent("ReplaceFont");
		window.Show();
	}
	public static void GetFiles(DirectoryInfo directory, string pattern, ref List<string> fileList)
	{
		if (directory != null && directory.Exists && !string.IsNullOrEmpty(pattern)) 
		{
			try 
			{
				foreach (FileInfo info in directory.GetFiles(pattern)) 
				{
					string path = info.FullName.ToString();
					fileList.Add(path.Substring (path.IndexOf ("Assets")));
				}
			} 
			catch (System.Exception) 
			{
				throw;
			}
			foreach (DirectoryInfo info in directory.GetDirectories()) 
			{
				GetFiles(info, pattern, ref fileList);
			}
		}
	}
	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		isUGUI = EditorGUILayout.Toggle("UGUI",isUGUI);
		isUGUI = EditorGUILayout.Toggle("NGUI", !isUGUI);
		EditorGUILayout.EndHorizontal();
		curFont = (Font) EditorGUILayout.ObjectField("被替换字体", curFont, typeof(Font), true);
		targetFont = (Font) EditorGUILayout.ObjectField("目标字体", targetFont, typeof(Font), true);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("字号比例：");
		fontSizeRatio = EditorGUILayout.FloatField(fontSizeRatio);
		EditorGUILayout.EndHorizontal();
		if(GUILayout.Button("一键替换"))
		{
			for (int i = 0; i<prefafbPathList.Count; i++)
			{
				GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefafbPathList[i]);
				Change(gameObj);
			}
		AssetDatabase.SaveAssets();
		}
	}

	public static void Change(GameObject prefab)
	{
		if(null != prefab)
		{
            Component[] labels = null;
			if(isUGUI)
			{
                labels = prefab.GetComponentsInChildren<Text>(true);
			}
			else
			{
                //labels = prefab.GetComponentsInChildren<UILabel>(true);
			}
			if(null != labels)
			foreach (Object item in labels)
			{
				if(isUGUI)
				{
                    Text text = (Text)item;
                    int newFontSize = (int)(text.fontSize * fontSizeRatio);
                    if (text.font.name == curFont.name)
                    {
                        text.font = targetFont;
                        text.fontSize = newFontSize;
                       
                    }
				}
				else
				{
                    // UILabel label = (UILabel)item;
					// int newFontSize = (int)(label.fontSize * fontSizeRatio);
                    // if (label.trueTypeFont.name == curFont.name)
                    // {
                    //  	label.trueTypeFont = targetFont;
                   	//  	label.fontSize = newFontSize;
                    // }
				}
                EditorUtility.SetDirty(item);
			}
		}
	}
}
