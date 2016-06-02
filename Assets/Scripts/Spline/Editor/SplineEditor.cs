using UnityEngine;
using UnityEditor;

/* Editor for Spline.cs

  Contributors:
  https://bitbucket.org/tjgoolsby/
*/
[System.Serializable]
[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor {
	[SerializeField]
	private bool editMode;
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

    Spline bsp = target as Spline;
    bsp.EnsurePoints();

    // Edit button hides the transform tools
    bool toggleOutput = GUILayout.Toggle(editMode, "Edit", GUI.skin.GetStyle("Button"));
    Tools.hidden = toggleOutput;
    bool dirty = false;
    if (toggleOutput != editMode) {
      dirty = true;
      
    }
    editMode = toggleOutput;

    // Button to add a control point
    bool addPoint = GUILayout.Button("Push Control Point");
    if (addPoint) {
      GameObject go = new GameObject("Control Point " + bsp.transform.childCount);
      Undo.RegisterCompleteObjectUndo(go, "Push Control Point");
      try { 
        go.transform.position = bsp.points[bsp.points.Length - 1].position;
      } catch {
        Debug.Log("IndexOutOfRange");
        go.transform.position = bsp.transform.position;
      }
      go.transform.parent = bsp.transform;
    }

    // Button to remove points
    bool removePoint = GUILayout.Button("Pop Control Point");
    if (removePoint) {
      Undo.DestroyObjectImmediate(bsp.points[bsp.points.Length - 1].gameObject);
      bsp.EnsurePoints();
    }
    
    // repaint scene at the end of inspectorgui
    if (dirty) {
      SceneView.RepaintAll();
    }
	}
	
	void OnSceneGUI() {
    Spline bsp = target as Spline;
    bsp.EnsurePoints();

    int N = bsp.points.Length;
    
    if (editMode) {
      // show transform position handles for each control point.
      for (int i = 0; i < N; i++) {
        bsp.points[i].position = Handles.PositionHandle(bsp.points[i].position, Quaternion.identity);
      }
    }
	}
}
