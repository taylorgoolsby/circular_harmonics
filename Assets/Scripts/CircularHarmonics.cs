using UnityEngine;
using System.Collections;

public class CircularHarmonics : MonoBehaviour {
  public float radius;
  public int n = 1;
  public int harmonics = 1;
  public float offset;
  public float speed;
  public int nDrawPoints;
  public bool drawWave;
  public bool drawRibbon;
  public bool drawCircle;
  public bool drawProb;
  public Color drawColor = Color.cyan;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnDrawGizmos() {
    Complex wave; 
    float prob1, prob2;
    Complex basis, timeOp; 
    
    // iterate over nDrawPoints around a circle.
		for (int i = 0; i < nDrawPoints; i++) {
      // note t here does not stand for time
      float t1 = NormalizeIterator(i, nDrawPoints); // map interator to domain [0, 1]

      // add up consecutive basis functions
      wave = new Complex(0, 0);
      for (int j = 0; j < harmonics; j++) {
        timeOp = EvaluateTimeEvolutionOperator(n + j + 1, offset);
        basis = EvaluateBasis(n + j + 1, t1 * 2 * Mathf.PI);
        //wave += new Complex(timeOp.a * basis.a, timeOp.b * basis.b);
        wave += timeOp * basis; // the wave function evaluated at t
      }
      wave /= harmonics; // normalize
      prob1 = wave.sqrMagnitude;

      // a circle whose radius is perturbed by wave.a
      // forms a spiral around a circular path
      float x1 = (wave.a + radius) * Mathf.Cos(t1 * 2 * Mathf.PI);
      float y1 = wave.b;
      float z1 = (wave.a + radius) * Mathf.Sin(t1 * 2 * Mathf.PI);
      
      // repeat the above steps for the next point in order to draw a line segment.
      float t2 = NormalizeIterator(i + 1, nDrawPoints); // map iterator to [0, 1]
      wave = new Complex(0, 0);
      for (int j = 0; j < harmonics; j++) {
        timeOp = EvaluateTimeEvolutionOperator(n + j + 1, offset);
        basis = EvaluateBasis(n + j + 1, t2 * 2 * Mathf.PI);
        //wave += new Complex(timeOp.a * basis.a, timeOp.b * basis.b);
        wave += timeOp * basis;
      }
      wave /= harmonics;
      prob2 = wave.sqrMagnitude;
      float x2 = (wave.a + radius) * Mathf.Cos(t2 * 2 * Mathf.PI);
      float y2 = wave.b;
      float z2 = (wave.a + radius) * Mathf.Sin(t2 * 2 * Mathf.PI);

      // final computation of wave's line segment points
      Vector3 p1 = new Vector3(x1, y1, z1);
      Vector3 p2 = new Vector3(x2, y2, z2);
      p1 = transform.rotation * (p1 + transform.position); // object to world
      p2 = transform.rotation * (p2 + transform.position); // object to world

      // draw the equilibrium circle
      x1 = radius * Mathf.Cos(t1 * 2 * Mathf.PI);
      z1 = radius * Mathf.Sin(t1 * 2 * Mathf.PI);
      x2 = radius * Mathf.Cos(t2 * 2 * Mathf.PI);
      z2 = radius * Mathf.Sin(t2 * 2 * Mathf.PI);
      Vector3 ec1 = new Vector3(x1, 0, z1);
      Vector3 ec2 = new Vector3(x2, 0, z2);
      ec1 = transform.rotation * (ec1 + transform.position);
      ec2 = transform.rotation * (ec2 + transform.position);
      Vector3 equilibriumPoint = (ec1 + ec2) / 2f;
      Gizmos.color = Color.white;
      if (drawCircle) {
        Gizmos.DrawLine(ec1, ec2);
      }

      // draw the wave
      Gizmos.color = drawColor;
      Vector3 wavePoint = (p1 + p2) / 2f;
      if (drawRibbon) {
        Vector3 diff = wavePoint - equilibriumPoint;
        Gizmos.DrawLine(equilibriumPoint, wavePoint); // uncomment this line to draw rays between the equilibrium circle and the wave.
      }
      if (drawWave) {
        Gizmos.DrawLine(p1, p2); // uncomment this line to draw the wave.
      }
      
      if (drawProb) {
        // draw the probability
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(equilibriumPoint, (prob1 + prob2) / 2f);
      }
    }
	}
	
	// Update is called once per frame
	void Update () {
    offset += Time.deltaTime * speed;
	}

  private float NormalizeIterator(int i, int maxIterations) {
    // maps an iterator to the domain [0, 1]
    if (i < maxIterations) {
      return 1f * i / maxIterations;
    } else {
      return 1;
    }
  }

  public static Complex EvaluateBasis(int n, float x) {
    // Evaluate a basis function for circular harmonics.
    // n on [1, inf)
    // x on [0, 2*pi]
    float a = Mathf.Cos(n * x) / Mathf.PI;
    float b = Mathf.Sin(n * x) / Mathf.PI;
    return new Complex(a, b);
  }

  private Complex EvaluateTimeEvolutionOperator(float E, float t) {
    // given energy and time
    float phase = -E * t;
    float a = Mathf.Cos(phase);
    float b = Mathf.Sin(phase);
    return new Complex(a, b);
  }
}
