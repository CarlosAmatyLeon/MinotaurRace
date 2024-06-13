using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
        public GameObject player;
        float cameraTilt = 60.0f;
        float cameraHeight = 6.0f;
        float xOffset = 0.0f;
        float zOffset = -3.0f;

        void Start()
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(cameraTilt, 0, 0));
            transform.rotation = rotation;
        }

    void Update()
    {
        Vector3 newPosition = new Vector3(player.transform.position.x + xOffset, cameraHeight, player.transform.position.z + zOffset);
        transform.position = newPosition;
    }
}
