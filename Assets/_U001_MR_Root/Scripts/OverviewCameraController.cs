using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewCameraController : MonoBehaviour
{
    Vector3 StartPosition;
    Vector3 EndPosition;
    bool canMove = false;
    float duration = 5.0f;
    float elapsedTime = 0;
    public void MoveCamera()
    {
        StartPosition = new Vector3(-15.0f, 14.0f, -20.0f);
        EndPosition = new Vector3(-10.0f, 10.0f, (SceneManagerMaze.Instance.mazeSize * 5.0f) / 2 + 5.0f);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, t);

            if (t >= 1.0f)
            {
                canMove = false;
            }
        }
    }
}
