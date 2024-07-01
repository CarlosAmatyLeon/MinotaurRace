using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MinotaurController : MonoBehaviour
{
    [SerializeField] float speed = 7.0f;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip roarSound;

    private Animator animator;
    private AudioSource audioSource;
    private Collider colliderElement;

    public bool isMoving = false;
    float distance = 5.0f;
    string direction = null;
    Vector3 raycastHeight = new Vector3(0.0f, 0.5f, 0.0f);
    Vector3 originalPosition;
    Vector3 destination;
    LayerMask minotaurRaycastLayer;
    Vector2 currentMazeCell;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        colliderElement = GetComponent<Collider>();
        minotaurRaycastLayer = LayerMask.GetMask("Walls", "Coins");
        currentMazeCell = new Vector2 (SceneManagerMaze.Instance.mazeSize, SceneManagerMaze.Instance.mazeSize);
        MoveStartingPosition();
    }

    void Update()
    {
        if (isMoving)
        {
            RandomMove();
        }
        
    }

    void RandomMove()
    {
        if (direction == null)
        {
            if (SceneManagerMaze.Instance.difficulty == 1) direction = ChooseRandomDirection(false);
            else direction = ChooseRandomDirection(true);

            if (direction == null)
            {
                transform.Rotate(0.0f, 90.0f, 0.0f);
            }
            else
            {
                originalPosition = transform.position;
                if (direction == "forward")
                {
                    destination = originalPosition + transform.TransformDirection(Vector3.forward) * distance;
                }
                else if (direction == "left")
                {
                    destination = originalPosition + transform.TransformDirection(Vector3.left) * distance;
                    transform.Rotate(0.0f, -90.0f, 0.0f);
                }
                else if (direction == "right")
                {
                    destination = originalPosition + transform.TransformDirection(Vector3.right) * distance;
                    transform.Rotate(0.0f, 90.0f, 0.0f);
                }
            }
        }
        else
        {
            if (Vector3.Distance(originalPosition, transform.position) < distance)
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                transform.position = destination;
                direction = null;
            }
        }
    }

    string ChooseRandomDirection(bool checkFOV)
    {
        List<string> options = new List<string>();

        if (!CheckWallInDirection(transform.TransformDirection(Vector3.forward)))
        {
            if (checkFOV)
            {
                if (CheckCoinInFOV(transform.TransformDirection(Vector3.forward)))
                {
                    return "forward";
                }
            }
            options.Add("forward");
        }
        if (!CheckWallInDirection(transform.TransformDirection(Vector3.left)))
        {
            if (checkFOV)
            {
                if (CheckCoinInFOV(transform.TransformDirection(Vector3.left)))
                {
                    return "left";
                }
            }
            options.Add("left");
        }
        if (!CheckWallInDirection(transform.TransformDirection(Vector3.right)))
        {
            if (checkFOV)
            {
                if (CheckCoinInFOV(transform.TransformDirection(Vector3.right)))
                {
                    return "right";
                }
            }
            options.Add("right"); 
        }

        if (options.Count > 0) return options[Random.Range(0, options.Count)];
        else return null;
    }

    public void MoveStartingPosition()
    {
        float lastCell = (SceneManagerMaze.Instance.mazeSize - 1) * 5 + 2.5f;
        transform.position = new Vector3(lastCell, 0f, lastCell);
    }

    public void BeginSequence()
    {
        animator.SetBool("IsRunning", true);
        audioSource.enabled = true;
        SetSpeed();
        isMoving = true;
        AudioManager.Instance.PlaySFX(roarSound);
    }

    bool CheckWallInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + raycastHeight, direction, out hit, distance, minotaurRaycastLayer))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                return true;
            }
        }
        return false;
    }

    bool CheckCoinInFOV(Vector3 direction)
    {
        RaycastHit longRay;        
        if (Physics.Raycast(transform.position + raycastHeight, direction, out longRay, SceneManagerMaze.Instance.mazeSize * distance, minotaurRaycastLayer))
        {
            if (longRay.collider.gameObject.tag == "coin")
            {
                return true;
            }
        }
        return false;
    }

    public void Stunned ()
    {
        isMoving = false;
        animator.SetBool("IsStunned", true);
        audioSource.enabled = false;
        colliderElement.enabled = false;
        AudioManager.Instance.PlaySFX(painSound);
        StartCoroutine(KipUp());
    }

    IEnumerator KipUp()
    {
        yield return new WaitForSeconds(4);
        isMoving = true;
        animator.SetBool("IsStunned", false);
        audioSource.enabled = true;
        colliderElement.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().Stunned();
        }
    }

    void SetSpeed()
    {
        if ( SceneManagerMaze.Instance.difficulty == 1 )
        {
            speed = 7.0f;
        }
        else if (SceneManagerMaze.Instance.difficulty == 2 )
        {
            speed = 9.0f;
        }
        else
        {
            speed = 11.0f;
        }
    }

    public void GameOver()
    {
        isMoving = false;
        audioSource.Stop();
        animator.SetBool("IsStunned", false);
        animator.SetBool("IsRunning", false);
    }
}
