using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
    public GameObject _poolPrefab;
    public int _amount;
    public GameObject _parentObj;

    private Queue<GameObject> _poolQueue = new Queue<GameObject>();

    public void Initialize(GameObject parentObj = null)
    {
        if (parentObj != null)
            _parentObj = parentObj;

        for (int index = 0; index < _amount; index++)
        {
            _poolQueue.Enqueue(CreateNewObject());
        }
    }

    public GameObject CreateNewObject()
    {
        // GameObject tempObj = Instantiate(_poolPrefab);
        GameObject tempObj = GameObject.Instantiate(_poolPrefab);

        tempObj.SetActive(false);

        if (_parentObj != null)
            tempObj.transform.SetParent(_parentObj.transform);

        return tempObj;
    }

    // 필요 시 overload
    public GameObject GetObject(Vector3 position, Quaternion quaternion)
    {
        if (_poolQueue.Count > 0)
        {
            GameObject tempObj = _poolQueue.Dequeue();

            tempObj.transform.position = position;
            tempObj.transform.eulerAngles = quaternion.eulerAngles;

            tempObj.SetActive(true);

            return tempObj;
        }
        else
        {
            GameObject newObj = CreateNewObject();

            newObj.transform.position = position;
            newObj.transform.eulerAngles = quaternion.eulerAngles;

            newObj.SetActive(true);

            return newObj;
        }
    }

    // 필요 시 overload
    public void ReturnObject(GameObject obj)
    {
        _poolQueue.Enqueue(obj);

        if (_parentObj != null)
            obj.transform.SetParent(_parentObj.transform);

        obj.SetActive(false);
    }
}
