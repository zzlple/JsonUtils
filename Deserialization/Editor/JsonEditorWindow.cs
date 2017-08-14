using UnityEngine;
using System.Collections;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Xamasoft.JsonClassGenerator;
using System.Collections.Generic;
using Xamasoft.JsonClassGenerator.CodeWriters;

[InitializeOnLoad]
public class JsonEditorWindow : AbstractEditorWindow,OnInputEventsListener
{



	static  JsonEditorWindow ()
	{

		Application.RegisterLogCallback (delegate(string condition, string stackTrace, LogType type) {


			Debug.Log("该编辑器依赖于.NET2.0");

		});  

		Debug.Log ("该编辑器依赖于.NET2.0");
		UnityEditor.PlayerSettings.apiCompatibilityLevel = UnityEditor.ApiCompatibilityLevel.NET_2_0;
	}


	public static class Jconfigure
	{


		public static string nameSpace;
		public static string mainClassName;
		public static string jsonStr;
		public static string outputFolder;
		public static bool isFields=true;
		public static bool isPublic = true;
		public static bool isGenerateSingleFile = true;


	}




	public enum JSONFROM
	{
		FILE = 0,
		STRING = 1,
		URI = 2
	
	}

	public static JSONFROM jsonFrom = JSONFROM.STRING;
	public string inputFile = "", inputURI = "";


	public string opMsg;
	public Vector2 positionScrollText = new Vector2 (0f, 0f);

	public void Cut ()
	{
		Debug.Log ("CUT");
	}

	public void Copy ()
	{
		Debug.Log ("Copy");
	}

	public void Paste ()
	{
		Debug.Log ("Paste");
	}

	public void SelectAll ()
	{
		Debug.Log ("SelectAll");


	}

	[MenuItem ("JsonUtils/Manager")]
	public static void AddWindow ()
	{     
		getInstance ("Json解析器", new Rect (0, 0, 640, 720), typeof(JsonEditorWindow));
	}

	void OnGUI ()
	{



		DoGUI ();

		DoEvents (this);
	}

	GUIStyle tempStyle;

	void DoGUI ()
	{


	

		GUILayout.BeginHorizontal ();

		
		GUILayout.Label ("类型", new GUILayoutOption[]{ GUILayout.Width (50) });
		jsonFrom = (JSONFROM)EditorGUILayout.EnumPopup (jsonFrom, new GUILayoutOption[]{ GUILayout.Width (200) });

		GUILayout.EndHorizontal ();


		switch (jsonFrom) {
		case JSONFROM.FILE:
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("文件", new GUILayoutOption[] {
				GUILayout.Width (50),
				GUILayout.Height (50)
			});
			inputFile = EditorGUILayout.TextField (inputFile, new GUILayoutOption[] {
				GUILayout.Width (400),
				GUILayout.Height (20)
			});


			GUILayout.EndHorizontal ();

			break;

		case JSONFROM.STRING:




			tempStyle =	new GUIStyle ();
			
			tempStyle.wordWrap = true;

			tempStyle.alignment = TextAnchor.UpperLeft;

			tempStyle.fontSize = 15;
			GUIStyleState gss =	new GUIStyleState ();

			gss.textColor = Color.white;
			tempStyle.onNormal = gss;

			GUILayout.BeginHorizontal ();

			GUILayout.Label ("字符串", new GUILayoutOption[] {
				GUILayout.Width (50),
				GUILayout.Height (50)
			});



			positionScrollText =	EditorGUILayout.BeginScrollView (positionScrollText, true, true, new GUILayoutOption[] {
				GUILayout.Width (550),
				GUILayout.Height (400)
			});


	

			Jconfigure.jsonStr = EditorGUILayout.TextField (Jconfigure.jsonStr, tempStyle, new GUILayoutOption[] {
				GUILayout.Width (550),
				GUILayout.Height (400)
				
			});

		



			EditorGUILayout.EndScrollView ();

			GUILayout.EndHorizontal ();

			break;

		case JSONFROM.URI:
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("网址", new GUILayoutOption[] {
				GUILayout.Width (50),
				GUILayout.Height (50)
			});
			inputURI = EditorGUILayout.TextField (inputURI, new GUILayoutOption[] {
				GUILayout.Width (400),
				GUILayout.Height (20)
			});
			GUILayout.EndHorizontal ();
			break;
		}





		GUILayout.Space (10);

	
		GUILayout.BeginVertical ();
		GUILayout.BeginHorizontal ();



		GUILayout.Label ("命名空间：", new GUILayoutOption[]{ GUILayout.Width (50) });
	

		Jconfigure.nameSpace = EditorGUILayout.TextField (Jconfigure.nameSpace , new GUILayoutOption[] {
			GUILayout.Width (350),
			GUILayout.Height (20)
		});


		GUILayout.EndHorizontal ();


		GUILayout.BeginHorizontal ();



		GUILayout.Label ("主类名：", new GUILayoutOption[]{ GUILayout.Width (50) });


		Jconfigure.mainClassName = EditorGUILayout.TextField (Jconfigure.mainClassName , new GUILayoutOption[] {
			GUILayout.Width (350),
			GUILayout.Height (20)
		});


		GUILayout.EndHorizontal ();


		GUILayout.BeginHorizontal ();



		GUILayout.Label ("成员变量：", new GUILayoutOption[]{ GUILayout.Width (50) });


		Jconfigure.isFields = EditorGUILayout.Toggle (Jconfigure.isFields);


		GUILayout.EndHorizontal ();




		GUILayout.BeginHorizontal ();



		GUILayout.Label ("公开方法：", new GUILayoutOption[]{ GUILayout.Width (50) });


		Jconfigure.isPublic = EditorGUILayout.Toggle (Jconfigure.isPublic);


		GUILayout.EndHorizontal ();




		GUILayout.BeginHorizontal ();



		GUILayout.Label ("单一文件：", new GUILayoutOption[]{ GUILayout.Width (50) });


		Jconfigure.isGenerateSingleFile = EditorGUILayout.Toggle (Jconfigure.isGenerateSingleFile);


		GUILayout.EndHorizontal ();




		GUILayout.EndVertical ();







		GUILayout.BeginHorizontal ();



		GUILayout.Label ("输出", new GUILayoutOption[]{ GUILayout.Width (50) });


		Jconfigure.outputFolder = EditorGUILayout.TextField (	Jconfigure.outputFolder , new GUILayoutOption[] {
			GUILayout.Width (350),
			GUILayout.Height (20)
		});



		if (GUILayout.Button ("选择路径", new GUILayoutOption[]{ GUILayout.Height (19), GUILayout.Width (90) })) {
		
		
			Jconfigure.outputFolder = getLocalFolderPath ();
		}

		if (GUILayout.Button ("生成", new GUILayoutOption[]{ GUILayout.Height (19), GUILayout.Width (90) })) {


	
			if (string.IsNullOrEmpty (	Jconfigure.outputFolder)) {
				opMsg = "输出路径非法";
			
			} else {


				if (!System.IO.Directory.Exists (	Jconfigure.outputFolder)) {
				
					System.IO.Directory.CreateDirectory (	Jconfigure.outputFolder);
				}


				Prepare (Jconfigure.jsonStr).GenerateClasses ();


				opMsg = "";
			}


		}


		GUILayout.EndHorizontal ();


		GUILayout.Space (5);


		GUILayout.Label (opMsg, new GUILayoutOption[] {
			GUILayout.Width (350),
			GUILayout.Height (20)
		});




	}


	private JsonClassGenerator Prepare (string json)
	{



		var gen = new JsonClassGenerator ();
		gen.Example = json;
		gen.InternalVisibility = !Jconfigure.isPublic;

		gen.CodeWriter = new CSharpCodeWriter ();
		gen.ExplicitDeserialization = false;



		gen.Namespace = Jconfigure.nameSpace;


	
		gen.NoHelperClass = true;
	//	gen.SecondaryNamespace =  Jconfigure.nameSpace;

		gen.TargetFolder = 	Jconfigure.outputFolder ;
		gen.UseProperties = !Jconfigure.isFields;
		gen.MainClass = Jconfigure.mainClassName;
		gen.UsePascalCase = false;
		gen.UseNestedClasses = false;
		gen.ApplyObfuscationAttributes = false;
		gen.SingleFile = Jconfigure.isGenerateSingleFile;
		gen.ExamplesInDocumentation = false;
		return gen;
	}



}
