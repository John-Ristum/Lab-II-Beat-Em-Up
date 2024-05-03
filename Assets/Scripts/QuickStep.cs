using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStep : GameBehaviour
{
    public float dashSpeed = 10f;
    public float dashTime = 0.25f;
    float startTime;
    public int dashNum;
    public int dashLimit = 2;
    Vector3 dashDir;
    string quickStepAnim;

    void Update()
    {
        if (_PLAYER.state == PlayerState.Dead || _PLAYER.inCutscene == true)
            return;

        if (Input.GetButtonDown("QuickStep") && _PLAYER.state != PlayerState.Damage)
        {
            if (dashNum >= dashLimit)
                return;

            dashNum++;
            //Stops previous quickstep
            StopCoroutine(QuickStepCo());
            //dashDir = new Vector3(0, 0, 0);
            //_PLAYER.controller.Move(dashDir);
            //Starts new quickstep
            StartCoroutine(QuickStepCo());
        }
    }

    IEnumerator QuickStepCo()
    {
        //Gets direction of quickstep (Allows for directional quickstepping)
        if (_PLAYER.inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_PLAYER.inputDirection.x, _PLAYER.inputDirection.z) * Mathf.Rad2Deg + _PLAYER.cam.eulerAngles.y;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            dashDir = Quaternion.Euler(_PLAYER.x, targetAngle, _PLAYER.z) * Vector3.forward;

            //dashDir = _PLAYER.moveDirection.normalized;
            quickStepAnim = "QuickStepForward";
        }
        else
        {
            dashDir = _PLAYER.transform.forward * -1;
            quickStepAnim = "QuickStepBack";
        }    

        //Prevents normal player movement
        _PLAYER.state = PlayerState.QuickStep;

        startTime = Time.time;

        int dashNum2 = dashNum;

        _PLAYER.anim.SetTrigger(quickStepAnim);

        //Quickstep movement
        while(Time.time < startTime + dashTime)
        {
            _PLAYER.controller.Move(dashDir * dashSpeed * Time.deltaTime);

            if (dashNum2 != dashNum)
                break;

            yield return null;
        }
    }
}
