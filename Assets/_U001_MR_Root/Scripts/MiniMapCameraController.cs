using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    [SerializeField] Camera MiniMapCamera;
    
    public void SetupCamera()
    {
        MiniMapCamera.orthographicSize = SceneManagerMaze.Instance.mazeSize * 2.75f;
        transform.position = new Vector3(SceneManagerMaze.Instance.mazeSize * 2.5f, 10.0f, SceneManagerMaze.Instance.mazeSize * 2.5f);
    }

    // The minimap camera will follow the player.
    /*    
    void Update()
    {
        Vector3 newPosition = new Vector3(Player.transform.position.x, 10.0f, Player.transform.position.z);
        transform.position = newPosition;
    }
    */
}
