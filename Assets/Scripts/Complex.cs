using UnityEngine;
using System.Collections;

public class Complex {
  public float a;
  public float b;

  public Complex() {
    a = 0;
    b = 0;
  }

  public Complex(float a, float b) {
    this.a = a;
    this.b = b;
  }

  public float magnitude {
    get {
      return Mathf.Sqrt(a * a + b * b);
    }
  }

  public float sqrMagnitude {
    get {
      return a * a + b * b;
    }
  }

  public override string ToString() {
    return a + " + " + b + "i";
  }

  public static Complex operator +(Complex c1, Complex c2) {
    return new Complex(c1.a + c2.a, c1.b + c2.b);
  }

  public static Complex operator -(Complex c1, Complex c2) {
    return new Complex(c1.a - c2.a, c1.b - c2.b);
  }

  public static Complex operator *(Complex c1, Complex c2) {
    return new Complex(c1.a*c2.a - c1.b*c2.b, c1.a*c2.b + c2.a*c1.b);
  }

  public static Complex operator *(Complex c, float f) {
    return new Complex(c.a * f, c.b * f);
  }

  public static Complex operator /(Complex c, float f) {
    return new Complex(c.a / f, c.b / f);
  }

  public static Complex InvertUnity(Complex c) {
    Complex r = new Complex();
    r.a = c.a;
    r.b = -c.b;
    return r;
  }

  public static Complex NthRootUnity(int n) {
    Complex r = new Complex();
    float t = Mathf.Tan(2 * Mathf.PI / n);
    r.a = 1 / (1 + t*t);
    r.b = Mathf.Sqrt(1 - r.a);
    r.a = Mathf.Sqrt(r.a);
    return r;
  }

  public static Complex[] Parse(float[] f) {
    Complex[] c = new Complex[f.Length];
    for (int i = 0; i < f.Length; i++) {
      c[i]  = new Complex(f[i], 0);
    }
    return c;
  }
}
