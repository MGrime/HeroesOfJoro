using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementScript : MonoBehaviour
{
    public CharacterController mController;
    public Transform PlayerCam;
    public Animator mPlayerAnimation;
    public float mSpeed = 10.0f;
    public float mTurnSmootTime = 0.1f;
    float mTurnSmoothVelocity;
    // Update is called once per frame
    void Update()
    {
        float mHorizontal = Input.GetAxisRaw("Horizontal");
        float mVertical = Input.GetAxisRaw("Vertical");
        Vector3 mDirection = new Vector3(mHorizontal, 0.0f, mVertical).normalized;
        if (mDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(mDirection.x, mDirection.z)*Mathf.Rad2Deg+PlayerCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref mTurnSmoothVelocity,mTurnSmootTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            Vector3 mMoveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            mController.Move(mMoveDirection.normalized * mSpeed * Time.deltaTime);
            mPlayerAnimation.SetBool("Moving", true);
        }
        else mPlayerAnimation.SetBool("Moving", false);

    }
}
