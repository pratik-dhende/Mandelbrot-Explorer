using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotExplorer : MonoBehaviour
{
    public Material mat;

    public Vector2 position;

    public float scale = 4;
    public float moveSpeed = 2f;
    public float angle = 0f;
    public float smoothFactor = 0.03f;
    public float scrollSpeed = 0.1f;
    public float gradientRange = 20.0f;
    public int maxIterations = 255;

    public bool fixMandelbrotColor = false;
    public bool originalMandelbrotColor = false;
    public Color mandelbrotGradient = Color.white;
    public Color mandelbrotColor = Color.black;
    public Color outerColor = Color.white;


    Vector2 smoothPosition;
    float smoothScale;
    float smoothAngle;

    bool mousePressed = false;

    void UpdateMandelbrot()
    {
        smoothPosition = Vector2.Lerp(smoothPosition, position, smoothFactor);
        smoothScale = Mathf.Lerp(smoothScale, scale, smoothFactor);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, smoothFactor);

        float scaleX = smoothScale;
        float scaleY = smoothScale;

        float aspectRatio = (float)Screen.width / (float)Screen.height;

        if (aspectRatio >= 1)
        {
            scaleY /= aspectRatio;
        }
        else
        {
            scaleX *= aspectRatio;
        }
        
        mat.SetFloat("_MaxIterations", maxIterations);
        mat.SetVector("_Area", new Vector4((!mousePressed) ? smoothPosition.x : position.x, (!mousePressed) ? smoothPosition.y : position.y, scaleX, scaleY));
        mat.SetFloat("_Angle", smoothAngle);
        mat.SetFloat("_GradientRange", gradientRange);
        mat.SetColor("_Gradientcolor", mandelbrotGradient);

        if (originalMandelbrotColor)
        {
            mat.SetInt("_OriginalMandelbrot", 1);
            mat.SetColor("_OuterColor", outerColor);
        }
        else
        {
            mat.SetInt("_OriginalMandelbrot", 0);
        }
        if (fixMandelbrotColor)
        {
            mat.SetInt("_FixMandelbrotColor", 1);
            mat.SetColor("_MandelbrotColor", mandelbrotColor);
        }
        else
        {
            mat.SetInt("_FixMandelbrotColor", 0);
        }
    }

    void FixedUpdate()
    {
        HandleInputs();
        UpdateMandelbrot();   
    }

    void HandleInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 xDir = Vector2.right;
        Vector2 yDir = Vector2.up;

        float cosa = Mathf.Cos(angle);
        float sina = Mathf.Sin(angle);

        xDir = new Vector2(xDir.x * cosa - xDir.y * sina, xDir.x * sina + xDir.y * cosa);
        yDir = new Vector2(-xDir.y, xDir.x);

        if (!mousePressed)
        {
            position += xDir * (horizontal * moveSpeed * scale);
            position += yDir * (vertical * moveSpeed * scale);
        }

        //if (!mousePressed)
            //position += new Vector2(horizontal, vertical) * moveSpeed * scale;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");

            Debug.Log(mouseX + " " + mouseY);

            //position -= new Vector2(mouseX, mouseY) * moveSpeed * scale;
            position -= xDir * (mouseX * moveSpeed * scale);
            position -= yDir * (mouseY * moveSpeed * scale);

            smoothPosition = position;
            mousePressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        float mouseScroll = Input.mouseScrollDelta.y;

        if (mouseScroll > 0)
        {
            scale *= 0.99f - scrollSpeed;
        }
        else if (mouseScroll < 0)
        {
            scale *= 1.01f + scrollSpeed;
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            scale *= 0.99f;
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            scale *= 1.01f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            angle += 0.01f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            angle -= 0.01f;
        }
    }
}
