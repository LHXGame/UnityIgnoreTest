using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonalControler : MonoBehaviour
{

    public float rotateSpeed = 200.0f; //每秒200度

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
    }

 
    private void RotateView()
    {
        float xValue = Input.GetAxis("Mouse X");
        float yValue = Input.GetAxis("Mouse Y");

        // transform.RotateAroundLocal(Vector3.up, rotateSpeed * xValue * Time.deltaTime);
        transform.Rotate(transform.up, rotateSpeed * xValue * Time.deltaTime,Space.World);
        transform.Rotate(transform.right, rotateSpeed * -yValue * Time.deltaTime, Space.World);

     //   //限制垂直方向的旋转(不能过高或者过低),绕自身的右方向
     //   Quaternion originRotation = transform.rotation;

     ////这个方向影响的属性有rotation和position



     //   float x = transform.eulerAngles.x;
     //   Debug.Log(x);
     //   //if (x > 90.0f || x < 10.0f) //还原
     //   //{
           
     //   //    transform.rotation = originRotation;
     //   //}
    }
}
