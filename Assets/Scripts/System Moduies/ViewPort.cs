using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPort : Singleton<ViewPort>
{
    float minX;
    float minY;
    float maxX;
    float maxY;
    float middleX;

    public float MaxX => maxX;

    private void Start()
    {
        Camera mainCamera = Camera.main;
        //记得将摄像机转化为正交投影
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        minX = bottomLeft.x;
        minY = bottomLeft.y;

        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f));
        maxX = topRight.x;
        maxY = topRight.y;

        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
    }

    public Vector3 PlayerMoveablePositon(Vector3 playerPositon,float paddingX,float paddingY)
    {
        Vector3 positon=Vector3.zero;

        positon.x = Mathf.Clamp(playerPositon.x, minX+paddingX, maxX-paddingX);
        positon.y = Mathf.Clamp(playerPositon.y, minY+paddingY, maxY-paddingY);
        return positon;
    }


    //敌人生成位置
    public Vector3 RandomEnemySpawnPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = maxX+paddingX;
        position.y = Random.Range(minY+paddingY, maxY-paddingY);

        return position;
    }


    //敌人移动
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

}
