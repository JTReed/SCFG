using UnityEngine;
using System.Collections;

public class CameraTriggerController : MonoBehaviour {

    void OnTriggerEnter2D()
    {
        Camera.main.GetComponent<GameCamera>().SetStatic(true);
    }

    void OnTriggerExit2D()
    {
        Camera.main.GetComponent<GameCamera>().SetStatic(false);
    }
}
