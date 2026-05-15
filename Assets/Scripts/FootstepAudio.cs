using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;

    [Header("Footstep Sounds")]
    public AudioClip[] footstepClips;

    [Header("Step Timing")]
    public float walkStepRate = 0.5f;
    public float sprintStepRate = 0.35f;
    public float crouchStepRate = 0.7f;

    private AudioSource audioSource;
    private float stepTimer;

    private FPSController fpsController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpsController = GetComponent<FPSController>();
    }

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        // Check if player is moving
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0;

        bool isMoving = horizontalVelocity.magnitude > 0.1f;
        bool isGrounded = controller.isGrounded;

        if (!isMoving || !isGrounded)
        {
            stepTimer = 0;
            return;
        }

        // Determine step rate
        float currentStepRate = walkStepRate;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentStepRate = sprintStepRate;
        }

        if (Input.GetKey(KeyCode.C))
        {
            currentStepRate = crouchStepRate;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0)
        {
            PlayRandomFootstep();
            stepTimer = currentStepRate;
        }
    }

    void PlayRandomFootstep()
    {
        if (footstepClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, footstepClips.Length);

        audioSource.PlayOneShot(footstepClips[randomIndex]);
    }
}