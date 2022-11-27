using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask grappleLayers;
    public Camera camera;
    public float maxDistance;
    public float delayTime;

    public float grapplingCooldown;
    
    private Vector3 _grapplePoint;
    private float _grappleTimer;

    private bool _grappling;


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            StartGrapple();
        }

        if (_grappleTimer > 0)
        {
            _grappleTimer -= Time.deltaTime;
        }
    }

    private void StartGrapple()
    {
        if (grapplingCooldown > 0)
            return;

        _grappling = true;

        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, grappleLayers))
        {
            _grapplePoint = hit.point;
            Invoke(nameof(Grapple), delayTime);
        }
        else
        {
            _grapplePoint = camera.transform.position + camera.transform.forward * maxDistance;
            Invoke(nameof(StopGrapple), delayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, _grapplePoint);
    }

    private void LateUpdate()
    {
        if (_grappling)
        {
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void Grapple()
    {
        
    }

    private void StopGrapple()
    {
        _grappling = false;
        _grappleTimer = delayTime;
        lineRenderer.enabled = false;
    }
}
