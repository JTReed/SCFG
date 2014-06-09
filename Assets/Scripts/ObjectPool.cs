using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    public int poolSize;
    public bool growToFit;
	
	private GameObject m_poolObject;
	private List<GameObject> m_pool;

	// Use this for initialization
	void Start () {
        m_pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++) {
            GameObject obj = (GameObject)Instantiate(m_poolObject);
            obj.SetActive(false);
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            m_pool.Add(obj);
        }
	}

    public void CreateObject()
    {
        for (int i = 0; i < m_pool.Count; i++) {
            if (!m_pool[i].activeInHierarchy) {
                m_pool[i].SetActive(true);
                m_pool[i].transform.position = transform.position;
                m_pool[i].transform.rotation = transform.rotation;
                break;
            }
        }
    }

	public void SetPoolObject(GameObject obj)
	{
		m_poolObject = obj;
	}

	public void StartPool()
	{
		Start();
	}
}
