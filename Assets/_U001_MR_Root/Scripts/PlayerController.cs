using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{   
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 0.2f;
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference showMap;
    [SerializeField] InputActionReference attack;
    [SerializeField] Camera mapCamera;
    [SerializeField] AudioClip movingSound;
    [SerializeField] AudioClip movingDisabled;
    [SerializeField] AudioClip stunnedSound;
    [SerializeField] AudioClip electricitySound;
    [SerializeField] GameObject minotaur;
    [SerializeField] ParticleSystem particlesStunned;
    [SerializeField] ParticleSystem particlesElectricity;
    [SerializeField] TextMeshProUGUI ammo;

    private Rigidbody rb;
    private Animator animator;
    private AudioSource sound;
    private bool isStunned = false;
    private bool isMoving = false;
    private bool isMapShowing = false;
    private int lightnings;

    Vector2 moveDirection = Vector2.zero;


    void OnEnable()
    {
        showMap.action.started += ShowMap;
        attack.action.started += Attack;
    }
    private void OnDisable()
    {
        showMap.action.started -= ShowMap;
        attack.action.started -= Attack;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        SetLightningAmmo();
        SetLightningAmmoText();
    }

    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        
        if (SceneManagerMaze.Instance.GameStarted && !isStunned && !isMapShowing)
        {   
            if (moveDirection.magnitude > 0)
            {   
                if (!sound.isPlaying) sound.Play();
                animator.SetBool("IsMoving", true);
                float angleRadians = Mathf.Atan2(moveDirection.x, moveDirection.y);
                Quaternion newRotation = Quaternion.Euler(new Vector3(0, angleRadians * Mathf.Rad2Deg, 0));
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                if (sound.isPlaying) sound.Stop();
                animator.SetBool("IsMoving", false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (SceneManagerMaze.Instance.GameStarted && !isStunned && !isMapShowing)
        {
            rb.velocity = new Vector3(moveDirection.x * speed, 0f, moveDirection.y * speed);
        }
    }

    void ShowMap(InputAction.CallbackContext context)
    {
        if (SceneManagerMaze.Instance.GameStarted && SceneManagerMaze.Instance.difficulty != 3)
        {
            if (isMapShowing)
            {   
                isMapShowing = false;
                mapCamera.gameObject.SetActive(false);
            }
            else
            {
                rb.velocity = Vector3.zero;
                sound.Stop();
                isMapShowing = true;
                mapCamera.gameObject.SetActive(true);
            }
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (lightnings > 0 && !isStunned)
        {   
            particlesElectricity.Play();
            AudioManager.Instance.PlaySFX(electricitySound);
            if (Vector3.Distance(transform.position, minotaur.transform.position) < 3.0f)
            {
                minotaur.gameObject.GetComponent<MinotaurController>().Stunned();
            }
            lightnings--;
            SetLightningAmmoText();
        }
    }

    public void Stunned()
    {
        if (!isStunned)
        {
            rb.velocity = Vector3.zero;
            isStunned = true;
            particlesStunned.Play();
            animator.SetBool("IsStunned", true);
            if (sound.isPlaying)
            {
                sound.Stop();
            }
            AudioManager.Instance.PlaySFX(stunnedSound);
            StartCoroutine(KipUp());
        }
    }

    IEnumerator KipUp()
    {
        yield return new WaitForSeconds(3);
        isStunned = false;
        animator.SetBool("IsStunned", false);
    }

    public void SetIsMoving(bool value)
    {
        isMoving = value;
    }

    void SetLightningAmmo ()
    {
        if (SceneManagerMaze.Instance.difficulty == 1)
        {
            lightnings = 10;
        }
        else if (SceneManagerMaze.Instance.difficulty == 2)
        {
            lightnings = 5;
        }
        else
        {
            lightnings = 3;
        }
    }

    void SetLightningAmmoText()
    {
        ammo.text = $"{lightnings}";
    }

    public void GameOver()
    {
        isMoving = false;
        rb.velocity = Vector3.zero;
        sound.Stop();
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsStunned", false);
    }
}
