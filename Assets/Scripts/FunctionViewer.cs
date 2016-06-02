using UnityEngine;
using System.Collections;

public class FunctionViewer : MonoBehaviour {
  public int n;
  public float upperBound = 10;
  public int nDrawPoints = 100;

  // Use this for initialization
  void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnDrawGizmos() {
    for (int i = 0; i < nDrawPoints; i++) {
      float t1 = NormalizeIterator(i, nDrawPoints);

      Vector3 p1 = new Vector3(t1, 0, Calc(t1));
      
      float t2 = NormalizeIterator(i + 1, nDrawPoints);

      Vector3 p2 = new Vector3(t2, 0, Calc(t2));

      Gizmos.color = Color.white;

      Vector3 draw1 = transform.rotation * p1 + transform.position;
      Vector3 draw2 = transform.rotation * p2 + transform.position;
      Gizmos.DrawLine(draw1, draw2);
    }
  }

  private float Calc(float t) {
    t = t * upperBound;
    return CircularHarmonics.EvaluateBasis(n, t).a;
  }

  private float NormalizeIterator(int i, int maxIterations) {
    if (i < maxIterations) {
      return 1f * i / maxIterations;
    } else {
      return 1;
    }
  }
}
