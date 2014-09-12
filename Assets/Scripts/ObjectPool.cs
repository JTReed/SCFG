using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    public int poolSize;
    public bool growToFit;
	
	private GameObject poolObject;
	private List<GameObject> pool;

	// Use this for initialization
	void Start () {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++) {
            GameObject obj = (GameObject)Instantiate(poolObject);
            obj.SetActive(false);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            pool.Add(obj);
        }
	}

    public void CreateObject()
    {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                pool[i].SetActive(true);
                pool[i].transform.position = transform.position;
                pool[i].transform.rotation = transform.rotation;
                break;
            }
        }
    }

	public void SetPoolObject(GameObject obj)
	{
		poolObject = obj;
	}

	public void StartPool()
	{
		Start();
	}
}
