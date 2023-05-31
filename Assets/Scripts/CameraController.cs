using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _lookAt;
    private readonly float _boundX = 0.3f;
    private readonly float _boundY = 0.15f;

    private void Awake()
    {
        _lookAt = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        //check if we're inside the bounds of the Y axis
        float deltaX = _lookAt.position.x - transform.position.x;
        if (deltaX > _boundX || deltaX < -_boundX)
        {
            if (transform.position.x < _lookAt.position.x)
                delta.x = deltaX - _boundX;
            else delta.x = deltaX + _boundX;
        }

        // check if we're inside the bounds of the Y axis
        float deltaY = _lookAt.position.y - transform.position.y;
        if (deltaY > _boundY || deltaY < -_boundY)
        {
            if (transform.position.y < _lookAt.position.y)
                delta.y = deltaY - _boundY;
            else delta.y = deltaY + _boundY;
        }

        transform.position += new Vector3(delta.x, delta.y, 0);


    }
}
