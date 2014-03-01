using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject player;

    private GameCamera mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GetComponent<GameCamera>();
        SpawnPlayer();
	}
	
	private void SpawnPlayer() {
        // Have spawnpoint gizmo
        Object clone = Instantiate(player, Vector2.zero, Quaternion.identity);
        mainCamera.SetTarget((clone as GameObject).transform);

    }
}
