using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPool : ObjectPool {

	public GameObject bulletStage1;
	public GameObject bulletStage2;
	public GameObject bulletStage3;

	private List<GameObject> chargedBullets;

	void Start() {
		poolSize = 3;
		growToFit = false;
		base.SetPoolObject(bulletStage1);
		base.StartPool();

		chargedBullets = new List<GameObject>();

		GameObject obj1 = (GameObject)Instantiate(bulletStage2);
		obj1.SetActive(false);
		obj1.transform.position = transform.position;
		obj1.transform.rotation = transform.rotation;
		chargedBullets.Add(obj1);

		GameObject obj2 = (GameObject)Instantiate(bulletStage3);
		obj2.SetActive(false);
		obj2.transform.position = transform.position;
		obj2.transform.rotation = transform.rotation;
		chargedBullets.Add(obj2);
	}
	
	public void CreateObject(int stage)
	{
		if (stage == 1) {
			base.CreateObject();
		} else if (stage == 2) {
			// only one charged shot onscreen for now
			if (!chargedBullets[0].activeInHierarchy) {
				chargedBullets[0].SetActive(true);
				chargedBullets[0].transform.position = transform.position;
				chargedBullets[0].transform.rotation = transform.rotation;
			}
		} else if (stage == 3) {
			// only one charged shot onscreen for now
			if (!chargedBullets[1].activeInHierarchy) {
				chargedBullets[1].SetActive(true);
				chargedBullets[1].transform.position = transform.position;
				chargedBullets[1].transform.rotation = transform.rotation;
			}
		} else {
			// something terrible has happened, just shoot a regular bullet
			CreateObject(1);
		}
	}
}
