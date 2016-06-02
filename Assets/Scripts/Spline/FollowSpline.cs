using UnityEngine;
using UnityEditor;
using System.Collections;

/* Makes the gameobject ride along a spline path.

  Contributors:
  https://bitbucket.org/tjgoolsby/
*/
public class FollowSpline : MonoBehaviour {
  public float playTime = 1;
  public float offset = 0;
  public Mode mode = Mode.Periodic;
  public bool ignoreX;
  public bool ignoreY;
  public bool ignoreZ;
  public Spline spline;
  public Transform lookTarget;
  public bool lookTangentToSpline;
  public bool reset;
  public bool controlSceneCamera; // this feature makes use of the undocumented SceneView, therefore it is experimental.

  protected float startTime;

  public enum Mode {
    PlayOnce,
    Periodic,
    Oscillate
  }

  // Use this for initialization
  void Start() {
    Reset();
  }

  // Update is called once per frame
  void Update() {
    if (reset) {
      reset = false;
      Reset();
    }
    if (spline == null) {
      return;
    }

    // get t on domain [0, 1]
    float t = (Time.realtimeSinceStartup - startTime) / playTime + offset;

    switch (mode) {
      case Mode.PlayOnce:
        // stop t at 1
        if (t > 1) {
          t = 1;
        }
        break;
      case Mode.Periodic:
        // allow t to keep incrementing. Spline.Calc(t) by default is periodic using modulo.
        break;
      case Mode.Oscillate:
        // oscillate t between 0 and 1 with period of 2.
        print(t);
        t = Mathf.Abs(((t + 1) % 2) - 1);
        break;
      default:
        break;
    }

    Vector3 splinePoint = spline.Calc(t);
    if (ignoreX) {
      splinePoint.x = transform.position.x;
    }
    if (ignoreY) {
      splinePoint.y = transform.position.y;
    }
    if (ignoreZ) {
      splinePoint.z = transform.position.z;
    }

    // Determine the look rotation.
    Quaternion rotation = transform.rotation;
    if (lookTarget != null) {
      rotation = Quaternion.LookRotation(lookTarget.position - splinePoint);
    } else if (lookTangentToSpline) {
      // look direction is tangent to spline.
      float previousTime = Mathf.Max(0, t - Time.deltaTime);
      Vector3 prevSplinePoint = spline.Calc(previousTime);
      Vector3 nextSplinePoint = spline.Calc(t + Time.deltaTime);
      Vector3 forward = nextSplinePoint - prevSplinePoint;
      rotation = Quaternion.LookRotation(forward);
    }

    // Apply Transform Changes
    transform.position = splinePoint;
    transform.rotation = rotation;

    if (controlSceneCamera && SceneView.sceneViews.Count >= 1) {
      // control the scene camera, too.

      SceneView sceneView = ((SceneView) SceneView.sceneViews[0]);

      // determine where the scene camera's pivot should be.
      Vector3 pivotPosition;
      if (lookTarget != null) {
        pivotPosition = lookTarget.position;
      } else {
        pivotPosition = splinePoint + rotation * Vector3.forward;
      }

      //sceneView.LookAt(splinePoint, rotation);
      sceneView.pivot = pivotPosition;
      if (lookTarget != null || lookTangentToSpline) {
        sceneView.rotation = rotation;
      }
      sceneView.size = Vector3.Magnitude(pivotPosition - splinePoint);
    }
  }

  public void Reset() {
    startTime = Time.realtimeSinceStartup;
  }
}
