using UnityEngine;
using System.Collections.Generic;

/* A degree 3 b-spline.
  https://en.wikipedia.org/wiki/De_Boor%27s_algorithm

  Contributors:
  https://bitbucket.org/tjgoolsby/
*/
public class Spline : MonoBehaviour {
  [HideInInspector]
  public Transform[] points; // these are transforms to make editing control points easy.
  public bool looped;
  public bool drawSpline = true;
  public int drawPoints = 20;
  public bool debugAnimate;

  private float[] knots;
  private int k = 3;

  private bool storedLooped; // mirrors/shadows the value of `looped`

  public void Awake() {
    EnsurePoints();
     
  }

  public void OnDrawGizmos() {
    if (!drawSpline) {
      return;
    }

    EnsurePoints();

    int N;

    // draw staright lines between control points.
    Gizmos.color = Color.gray;
    N = points.Length;
    for (int i = 0; i < N; i++) {
      if (points[i] == null) {
        return;
      }
      Vector3 a = points[i].position;
      Vector3 b = points[Mathf.Min((i + 1), N - 1)].position;
      Gizmos.DrawLine(a, b);
    }

    // draw the spline line segments
    Gizmos.color = Color.white;
    N = drawPoints;
    for (int i = 0; i < N; i++) {
      float t = Lerp(i, 0, N, 0, 1);
      Vector3 a = Calc(t);
      t = Lerp(Mathf.Min((i + 1), N), 0, N, 0, 1);
      Vector3 b = Calc(t);
      Gizmos.DrawLine(a, b);
    }

    // draw a red dot that travels down the spline
    // this might help to visualize stretching.
    if (debugAnimate) {
      float t = (Time.realtimeSinceStartup / 1f) % 1;
      Vector3 p = Calc(t);
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(p, 0.05f);
    }
  }

  public Vector3 Calc(float t) {
    //ensure 0 <= t <= 1
    if (t > 1) {
      // allow t = 1
      t = t % 1;
    }
    //if (!(0 <= t && t <= 1)) {
    //  Debug.LogError("Ensure 0 <= t <= 1. t: " + t);
    //}

    // ensure n >= k
    if (!(points.Length >= k)) {
      //Debug.LogError("Ensure n >= k");
      return transform.position;
    }

    int n = points.Length;

    // transform t to u^bar
    float uBar = 0;
    try {
      uBar = Lerp(t, 0, 1, knots[k - 2], knots[n - 1]);
    } catch {
      print("IndexOutOfRange");
      RecreatePoints();
    }
    

    // find index I such that u_I <= u^bar <= u_(I + 1)
    int I = FindIndex(uBar);

    // d[generation][point of that generation]
    Vector3[,] d = new Vector3[k, n];

    // init first generation
    for (int i = 0; i < n; i++) {
      d[0, i] = points[i].position;
    }

    //print("I: " + I);
    //print("uBar: " + uBar);

    for (int j = 1; j <= (k - 1); j++) {
      for (int i = (I - (k - 2)); i <= I - j + 1; i++) {
        // the i'th point of generation j
        float x = Lerp(uBar, knots[i + j - 1], knots[i + k - 1],
            d[j - 1, i].x, d[j - 1, i + 1].x);
        float y = Lerp(uBar, knots[i + j - 1], knots[i + k - 1],
            d[j - 1, i].y, d[j - 1, i + 1].y);
        float z = Lerp(uBar, knots[i + j - 1], knots[i + k - 1],
            d[j - 1, i].z, d[j - 1, i + 1].z);
        d[j, i] = new Vector3(x, y, z);
      }
    }

    return d[k - 1, I - (k - 2)];
  }

  private int FindIndex(float u) {
    int i = 0;
    while (u > knots[i]) {
      i++;
    }

    /*
		 * The following is so that u is in [u_I, u_I+1)
		 */
    if (u == knots[k - 2]) {
      return i;
    } else {
      return i - 1;
    }
  }

  public void EnsurePoints() {
    // Call this in case you delete or create one of control points.
    //print(transform.childCount);
    if (points == null || knots == null || points.Length != transform.childCount
      || looped != storedLooped) {
      RecreatePoints();
      storedLooped = looped;
    }
  }

  public void RecreatePoints() {
    Transform[] parentAndChildren = GetComponentsInChildren<Transform>();

    if (!looped) {
      // reject first child because it is actually the parent.
      points = new Transform[parentAndChildren.Length - 1];
      for (int i = 0; i < parentAndChildren.Length - 1; i++) {
        points[i] = parentAndChildren[i + 1];
      }
    } else {
      // since this a degree 3 curve, we need 2 extra points.
      points = new Transform[parentAndChildren.Length + 1]; // guaranteed length at least 2
      for (int i = 0; i < parentAndChildren.Length - 1; i++) {
        try {
          points[i] = parentAndChildren[i + 1];
        } catch {
          print("IndexOutOfRange");
          points[i] = parentAndChildren[0]; // always contains the parent
        }
      }
      points[points.Length - 2] = points[0];
      points[points.Length - 1] = points[1];
    }
    

    // update knots to reflect this change. (knots are always lin-spaced)
    MakeLinSpaceKnots();
  }

  public void MakeLinSpaceKnots() {
    // linearly space knots on [0, 1]
    int n = points.Length;
    knots = new float[n + k - 2];
    for (int i = 0; i < n + k - 2; i++) {
      knots[i] = (1.0f * i / (n + k - 3));
    }
  }

  public static float Lerp(float t, float inLeft, float inRight, float outLeft,
      float outRight) {
    float dif = inRight - inLeft;
    return (inRight - t) / dif * outLeft + (t - inLeft) / dif * outRight;
  }
}
