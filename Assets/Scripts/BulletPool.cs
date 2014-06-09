using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPool : ObjectPool {

	public GameObject bulletStage1;
	public GameObject bulletStage2;
	public GameObject bulletStage3;

	private List<GameObject> m_chargedBullets;

	void Start() {
		poolSize = 3;
		growToFit = false;
		base.SetPoolObject(bulletStage1);
		base.StartPool();

		m_chargedBullets = new List<GameObject>();

		GameObject obj1 = (GameObject)Instantiate(bulletStage2);
		obj1.SetActive(false);
		obj1.transform.position = transform.position;
		obj1.transform.rotation = transform.rotation;
		m_chargedBullets.Add(obj1);

		GameObject obj2 = (GameObject)Instantiate(bulletStage3);
		obj2.SetActive(false);
		obj2.transform.position = transform.position;
		obj2.transform.rotation = transform.rotation;
		m_chargedBullets.Add(obj2);
	}
	
	public void CreateObject(int stage)
	{
		if (stage == 1) {
			base.CreateObject();
		} else if (stage == 2) {
			// only one charged shot onscreen for now
			if (!m_chargedBullets[0].activeInHierarchy) {
				m_chargedBullets[0].SetActive(true);
				m_chargedBullets[0].transform.position = transform.position;
				m_chargedBullets[0].transform.rotation = transform.rotation;
			}
		} else if (stage == 3) {
			// only one charged shot onscreen for now
			if (!m_chargedBullets[1].activeInHierarchy) {
				m_chargedBullets[1].SetActive(true);
				m_chargedBullets[1].transform.position = transform.position;
				m_chargedBullets[1].transform.rotation = transform.rotation;
			}
		} else {
			// something terrible has happened, just shoot a regular bullet
			CreateObject(1);
		}
	}
}
