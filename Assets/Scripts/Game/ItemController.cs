using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public bool isAnimated = false;
    public bool isRotating = false;
    public bool isScaling = false;
    private bool scalingUp = true;

    public Vector3 rotationAngle;
    public Vector3 startScale;
    public Vector3 endScale;

    public float rotationSpeed;
    public float scaleSpeed;
    public float scaleRate;
    private float scaleTimer;

    [HideInInspector]
    public int Point;

    // Use this for initialization
    void Start()
    {
        Point = Random.Range(10, 51);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimated)
        {
            if (isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime;

                if (scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
                }
                else if (!scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
                }

                if (scaleTimer >= scaleRate)
                {
                    if (scalingUp)
                    {
                        scalingUp = false;
                    }
                    else if (!scalingUp)
                    {
                        scalingUp = true;
                    }

                    scaleTimer = 0;
                }
            }
        }
    }
}
