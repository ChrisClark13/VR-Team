using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mover : NetworkBehaviour {

    public static Action<Controller> CallMover;
    public static GameObject thisDino;

    Controller controller;

    private float _newRotate;
    private Quaternion _focusRotate;
    private Vector3 _moveRotate;
    private Rigidbody _characterRigid;
    private float _forward;
    private bool _jumping;
    private float leftRight;


    // Use this for initialization
    void Start ()
    {

        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        thisDino = gameObject;
        _jumping = false;
        _characterRigid = gameObject.GetComponent<Rigidbody>();
        CallMover += CallMoverHandler;
    }

    private void CallMoverHandler(/*string command,*/ Controller mover)
    {
        controller = mover;
        controller.jumpSpeed = 2000;
        controller.forwardJmpSpeed = 300;
        if (controller.clicked && !_jumping)
        {
            if (!controller.gripped)
            {
                _jumping = true;
                StartCoroutine(Jump());
                // StartCoroutine(ForwardForce());
                StartCoroutine(JumpCount());
                StartCoroutine(ResetJump());
                
                StopCoroutine(MoveController());
            }
        }
        if (controller.touching || controller.gripped)
        {
             StartCoroutine(MoveController());
      
            if (controller.clicked && !_jumping)
            {
                if (controller.device.GetAxis().y > .5f)
                {
                    _jumping = true;
                    StartCoroutine(ForwardJump());
                    StartCoroutine(JumpCount());
                    StartCoroutine(ResetJump());
                }
                if (controller.device.GetAxis().y < -.5f)
                {
                    _jumping = true;
                    StartCoroutine(BackJump());
                    StartCoroutine(JumpCount());
                    StartCoroutine(ResetJump());
                }
                if (controller.device.GetAxis().x > .5f || controller.device.GetAxis().x < -.5f)
                {
                    _jumping = true;
                    StartCoroutine(SideJump());
                    StartCoroutine(JumpCount());
                    StartCoroutine(ResetJump());
                }
            }
        }
    }


    IEnumerator MoveController()
    {
        yield return new WaitForFixedUpdate();
        controller.device = SteamVR_Controller.Input((int)controller.trackedObject.index);
        controller.touchSpot = new Vector2(controller.device.GetAxis().x, controller.device.GetAxis().y);
        

        

        if (controller.touchSpot != new Vector2(0, 0) && !controller.gripped)
        {
            //set input value to always be positive
            if (controller.touchSpot != new Vector2(0,0))
            {
                _forward = controller.device.GetAxis().x + controller.device.GetAxis().y;
                if (_forward < 0)
                {
                    _forward *= -1;
                }
                if (_forward > 1)
                {
                    _forward *= .5f;
                }
            }

            //controls transformations
            _characterRigid.MovePosition(transform.localPosition + transform.TransformDirection(new Vector3(0 , 0, _forward)
                ) * controller.moveSpeed * Time.deltaTime);

            //controls rotation
            _newRotate = Mathf.Atan2(controller.device.GetAxis().y, (controller.device.GetAxis().x * -1)) * Mathf.Rad2Deg;
            Quaternion tempRotate = Quaternion.Euler(0, _newRotate - ((controller.cameraHead.transform.localEulerAngles.y -90 + controller.vrRig.localEulerAngles.y) *-1), 0);    
            transform.localRotation = Quaternion.Slerp(transform.rotation, tempRotate, Time.deltaTime * controller.rotateSpeed);

        }

        if (controller.gripped)
        {
            if (controller.focus != null)
            {
                //controlls rotation
                _focusRotate = Quaternion.LookRotation(controller.focus.position - transform.position);
                _characterRigid.MoveRotation(Quaternion.Slerp(transform.rotation, _focusRotate, Time.deltaTime * controller.rotateSpeed));
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            }
                //controlls transformation
            if (!_jumping)
            {
                _characterRigid.MovePosition(transform.localPosition + transform.TransformDirection
                (new Vector3(controller.touchSpot.x, 0, controller.touchSpot.y)) * controller.moveSpeed * Time.deltaTime);
            }

        }
        if ((controller.touching && !controller.clicked) || controller.gripped)
            StartCoroutine(MoveController());

    }

    IEnumerator Jump()
    {
        yield return new WaitForFixedUpdate();
        _characterRigid.AddForce(Vector3.up * controller.jumpSpeed * Time.deltaTime);
        //_characterRigid.AddForce(transform.forward * controller.moveSpeed * controller.forwardJmpSpeed * Time.deltaTime);
        //transform.Translate(Vector3.up * controller.jumpSpeed * Time.deltaTime);
        if (controller.frameCount < controller.jumpAmount)
            StartCoroutine(Jump());
    }

    IEnumerator ForwardJump()
    {
        yield return new WaitForFixedUpdate();
        _characterRigid.AddForce(Vector3.up * controller.jumpSpeed * Time.deltaTime);
        _characterRigid.AddForce(transform.forward * controller.moveSpeed * controller.forwardJmpSpeed * Time.deltaTime);
        if (controller.frameCount < controller.jumpAmount)
            StartCoroutine(ForwardJump());
    }

    IEnumerator BackJump()
    {
        yield return new WaitForFixedUpdate();
        _characterRigid.AddForce(Vector3.up * controller.jumpSpeed * Time.deltaTime);
        _characterRigid.AddForce((transform.forward * -1)* controller.moveSpeed * controller.forwardJmpSpeed * Time.deltaTime);
        if (controller.frameCount < controller.jumpAmount)
            StartCoroutine(BackJump());
    }

    IEnumerator SideJump()
    {
        yield return new WaitForFixedUpdate();
        _characterRigid.AddForce(Vector3.up * controller.jumpSpeed * Time.deltaTime);
        if (controller.device.GetAxis().x <= 0)
            leftRight = -1;
        if (controller.device.GetAxis().x > 0)
            leftRight = 1;

        _characterRigid.AddForce((transform.right * leftRight)* controller.moveSpeed * controller.forwardJmpSpeed * Time.deltaTime);
        _focusRotate = Quaternion.LookRotation(controller.focus.position - transform.position);
        _characterRigid.MoveRotation(Quaternion.Slerp(transform.rotation, _focusRotate, Time.deltaTime * controller.rotateSpeed));
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        if (controller.frameCount < controller.jumpAmount)
            StartCoroutine(SideJump());
    }
    
    IEnumerator JumpCount()
    {
        yield return new WaitForSeconds(.1f);
        controller.frameCount++;
        if (controller.frameCount < controller.jumpAmount)
            StartCoroutine(JumpCount());
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1f);
        controller.frameCount = 0;
        _jumping = false;
        if (controller.touching || controller.gripped)
        {
            StartCoroutine(MoveController());
        }
    }

}


