using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBackGround : MonoBehaviour
{
    [SerializeField] Vector2 offsetVelocity;

    Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private IEnumerator Start()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += offsetVelocity*Time.deltaTime;
            yield return null;  
        }
    }
}
