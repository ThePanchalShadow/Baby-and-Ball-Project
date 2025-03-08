using System;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    // Inspector Variables
    [Header("Personalize the object speed")]
    public float maxObjectSpeed = 40f;
    [Space(10)]
    [Header("Personalize the flick speed")]
    public float flickSpeed = 4.0f;
    public float howClose = 9.5f;

    // Private Variables
    private bool thrown = false;
    private bool holding = false;
    private float startTime;
    private float endTime;
    private Vector3 startPosition;
    private Vector3 endPosition;

    //Other variables you may need
    private Rigidbody rb; //Make sure to assign the object a rigidbody
    public string respawnObjectName; //The name of the object you want to respawn
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        rb = GetComponent<Rigidbody>(); //Get the rigidbody component
        rb.useGravity = false; // Disable gravity at the start
    }

    private void Update()
    {
        // If holding, call OnTouch to update the object's position
        if (holding)
        {
            OnTouch();
        }
        // If thrown, do nothing
        else if (thrown)
        {
            return;
        }

        // Touch Input Handling
        if (Input.touchCount <= 0) return;
        Touch touch = Input.GetTouch(0);

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (touch.phase)
        {
            //When a touch begins
            case TouchPhase.Began:
            {
                Ray ray = _camera.ScreenPointToRay(touch.position);

                // Shoot the ray cast
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    //If the ray cast hits this object
                    if (hit.transform == this.transform)
                    {
                        startTime = Time.time;
                        startPosition = touch.position;
                        holding = true;
                    }
                }

                break;
            }
            //When a touch ends
            case TouchPhase.Ended when holding:
            {
                endTime = Time.time;
                endPosition = touch.position;

                //Calculations for time and distance
                float swipeDistance = (endPosition - startPosition).magnitude;
                float swipeTime = endTime - startTime;

                // Filtering the flick
                // The code for filtering the flick based on swipeTime would go here
                //This code is not included in the transcripts

                holding = false;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //Move the object while holding
    private void OnTouch()
    {
        Vector3 mousePos = Input.GetTouch(0).position;
        mousePos.z = Camera.main.nearClipPlane * howClose; //"howClose" is how close you want the ball to follow along the screen when you're dragging on

        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePos);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, 80 * Time.deltaTime);
    }
}