using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneController : MonoBehaviour {

    public string sceneToLoad;
    public Vector3 spawnPosition;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ChangeScene()
    {
        player.GetComponent<PlayerInfo>().SetSpawnPosition(spawnPosition);

        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ChangeScene();
        }
    }
}
