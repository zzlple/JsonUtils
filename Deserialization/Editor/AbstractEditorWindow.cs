using UnityEngine;
using System.Collections;
using UnityEditor;
public interface OnInputEventsListener
{
	void Cut();
	void Copy();
	void Paste();
	void SelectAll();
}
public abstract  class AbstractEditorWindow : EditorWindow
{
	public OnInputEventsListener IME;

	public  static EditorWindow window;

	public static AbstractEditorWindow getInstance (string name, Rect rect, System.Type t)
	{
		
		if (null == window) {
			Caching.CleanCache ();
			window = AbstractEditorWindow.GetWindowWithRect (t, rect, true, name);	
			window.Show ();
			window.Focus ();

		}

		return (AbstractEditorWindow)window;
	}


	public void DoEvents (OnInputEventsListener ime)
	{
		this.IME = ime;

		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
			case "UndoRedoPerformed":
			case "Cut":
			case "Copy":
			case "Paste":
			case "SelectAll":
				Event.current.Use ();
				break;
			}
		}

		if (Event.current.type == EventType.ExecuteCommand) {			
			switch (Event.current.commandName) {


			case "Cut":
				ime.Cut ();
				break;

			case "Copy":

				ime.Copy();

				break;

			case "Paste":
				ime.Paste ();

				break;

			case "SelectAll":
				ime.SelectAll ();
				break;

			}

			GUIUtility.ExitGUI ();
		}
	}




	public static string getLocalFilePath(string[] format){


		string tempPath = "";
		tempPath=EditorUtility.OpenFilePanel ("选择文件", "", "");
		if (string.IsNullOrEmpty (tempPath)) {

			return "";
		}
		int count = 0;
		for (int i=0; i<format.Length; i++) {


			if(tempPath.EndsWith(format[i])){

				count++;

			}

		}

		if (count == 0) {
			return "";

		}

		return tempPath;




	} 



	public static string getLocalFolderPath(){




		string tempPath = "";
		tempPath=EditorUtility.OpenFolderPanel ("选择文件夹", "", "");
		if (string.IsNullOrEmpty (tempPath)) {

			return "";
		}


		return tempPath+"/";




	} 


}
