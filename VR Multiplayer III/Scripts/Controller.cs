﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Controller : NetworkBehaviour {

    public SteamVR_TrackedController _controller;
    public SteamVR_TrackedObject trackedObject;
    public SteamVR_Controller.Device device;
    public Vector2 touchSpot;
    public GameObject dino;
    public GameObject bullet;
    public Transform nose;
    public float moveSpeed;
    public float rotateSpeed;
    public float jumpSpeed;
    public float bulletSpeed = 500;
    public float frameCount;
    public float jumpAmount;
    public bool touching;
    public bool pulledTrigger;    
    
    
    public bool clicked;


    private void OnEnable()
    {

        _controller = GetComponent<SteamVR_TrackedController>();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        _controller.TriggerClicked += HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerUnclicked;
        _controller.PadTouched += LPadTouched;
        _controller.PadUntouched += LPadUntouched;
        _controller.PadClicked += RHandlePadClicked;
        _controller.PadUnclicked += RHandlePadClickUp;
        _controller.Gripped += HandleGripped;
        _controller.Ungripped += HandleUngripped;
        touching = false;
        pulledTrigger = false;
        clicked = false;

        //_controller.TriggerUnclicked += HandleTriggerUnclicked;
        //_controller.MenuButtonClicked += MenuButtonHandler;
        //following = false;
        //melee = false;
        //_controller.Ungripped += HandleUngripped;            
        }


    private void HandleUngripped(object sender, ClickedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void HandleGripped(object sender, ClickedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void RHandlePadClickUp(object sender, ClickedEventArgs e)
    {
        clicked = false;
        
    }

    private void RHandlePadClicked(object sender, ClickedEventArgs e)
    {
        
        clicked = true;
        Mover.CallMover(this);
        //StartCoroutine(Jump());
    }

    private void LPadUntouched(object sender, ClickedEventArgs e)
    {
        touching = false;
        
        
    }

    private void LPadTouched(object sender, ClickedEventArgs e)
    {
        
        touching = true;
        Mover.CallMover(this);

    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        pulledTrigger = false;

    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        pulledTrigger = true;
        Mover.CallMover(this);
        //StartCoroutine(ShootGun());
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        _controller.TriggerUnclicked -= HandleTriggerUnclicked;
        _controller.PadTouched -= LPadTouched;
        _controller.PadUntouched -= LPadUntouched;
        _controller.PadClicked -= RHandlePadClicked;
        _controller.PadUnclicked -= RHandlePadClickUp;
        _controller.Gripped -= HandleGripped;
        _controller.Ungripped -= HandleUngripped;
    }

    //IEnumerator MoveController()
    //{
    //    yield return new WaitForFixedUpdate();
    //    device = SteamVR_Controller.Input((int)trackedObject.index);
    //    touchSpot = new Vector2(device.GetAxis().x, device.GetAxis().y);
    //    if (touchSpot != new Vector2(0, 0))
    //    {
    //        dino.transform.Translate(0, 0, (touchSpot.y * moveSpeed) * Time.deltaTime);
    //        dino.transform.Rotate(0, (touchSpot.x * rotateSpeed) * Time.deltaTime, 0);
    //    }
    //    if (touching)
    //        StartCoroutine(MoveController());
        
    //}

    //IEnumerator ShootGun()
    //{
    //    yield return new WaitForFixedUpdate();

    //    bullet.transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

    //    yield return new WaitForSeconds(1);


    //    bullet.transform.position = nose.GetComponent<Transform>().position;
    //    bullet.transform.localEulerAngles = nose.localEulerAngles;
    //}
    
    //IEnumerator Jump()
    //{
    //    yield return new WaitForFixedUpdate();
    //    dino.transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
    //    if (clicked == true && frameCount < jumpAmount)
    //        StartCoroutine(Jump());
    //}

    
}
