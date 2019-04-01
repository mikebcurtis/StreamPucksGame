using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectSnake : MonoBehaviour
{
    public Vector3 normalVector;

    private void OnTriggerEnter(Collider other)
    {
        SnakeHeadScript snakeHead = other.gameObject.GetComponent<SnakeHeadScript>();
        if (snakeHead != null)
        {
            snakeHead.movementVector = Vector3.Reflect(snakeHead.movementVector, normalVector);
        }
    }
}
