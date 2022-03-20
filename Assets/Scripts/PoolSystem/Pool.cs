using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [SerializeField] GameObject prefab;
    public GameObject Prefab => prefab;

    public int Size => size;
    public int RuntimeSize => queue.Count;

    [SerializeField] int size;
    Queue<GameObject> queue;

    Transform parent;

    public void Initialize(Transform parent)
    {
        this.parent = parent;
        queue = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            queue.Enqueue(Copy());
        }
    }

    //����Ԥ���岢����رգ��ȴ�����
    GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab, parent);
        copy.SetActive(false);
        return copy;
    }

    //ȡ������ʹ��
    GameObject AvailableObject()
    {
        GameObject availableObject=null;

        if (queue.Count > 0&&!queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy();
        }
        queue.Enqueue(availableObject);
        return availableObject;   
    }

    //���ÿ��ö���
    public GameObject PreparedObject()
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 positon)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = positon;
        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 positon,Quaternion rotation)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = positon;
        preparedObject.transform.rotation = rotation;
        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 positon, Quaternion rotation,Vector3 localScale)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = positon;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale;
        return preparedObject;
    }

    //����
    //public void Return(GameObject gameObject)
    //{
    //    queue.Enqueue(gameObject);
    //}
}
