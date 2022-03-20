using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRota : MonoBehaviour
{
    [SerializeField] Vector3 rotaAngle;
    [SerializeField] float speed = 360f;

    void OnEnable()
    {
        StartCoroutine(nameof(RotateCoroutine));
    }

    IEnumerator RotateCoroutine()
    {
        while (true)
        {
            transform.Rotate(rotaAngle * speed * Time.deltaTime);
            yield return null;
        }
    }
}
