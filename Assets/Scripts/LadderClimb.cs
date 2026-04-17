using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LadderClimb : MonoBehaviour
{
    [Header("Climbing")]
    public float climbSpeed = 4f;

    [Header("Top of ladder - empty GameObject placed at the top rung")]
    public Transform ladderTop;

    [Header("Offset")]
    public float ladderOffset = 0.8f;

    [Header("Jump off")]
    public float jumpOffForce = 4f;

    private bool _onLadder = false;
    private FPSController _fps;
    private CharacterController _cc;
    private Transform _player;
    private Collider _ladderCollider;
    private Vector3 _offsetDir;

    void Awake()
    {
        _ladderCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (!_onLadder) return;

        // Keep mouse look running manually while FPSController is disabled
        HandleMouseLook();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopClimbing(doJump: true);
            return;
        }

        float v = Input.GetAxisRaw("Vertical");

        if (ladderTop != null && v > 0f && _player.position.y >= ladderTop.position.y)
        {
            StopClimbing(doJump: false);
            return;
        }

        _cc.Move(Vector3.up * v * climbSpeed * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (_fps == null) return;

        float mouseX = Input.GetAxis("Mouse X") * _fps.mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _fps.mouseSensitivity * 100f * Time.deltaTime;

        // Vertical look — stored on FPSController so clamping stays consistent
        // We replicate the same logic FPSController uses
        float xRot = _fps.playerCamera.localEulerAngles.x;

        // Convert from 0-360 back to -180/180 range so Clamp works
        if (xRot > 180f) xRot -= 360f;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -_fps.maxLookAngle, _fps.maxLookAngle);

        _fps.playerCamera.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        // Horizontal look — rotate the player body left/right
        _player.Rotate(Vector3.up * mouseX);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _player = other.transform;
        _cc = other.GetComponent<CharacterController>();
        _fps = other.GetComponent<FPSController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (_onLadder) return;
        if (!other.CompareTag("Player")) return;
        if (Input.GetAxisRaw("Vertical") == 0f) return;

        _onLadder = true;
        _fps.enabled = false;

        Vector3 ladderCenter = _ladderCollider.bounds.center;
        Vector3 toPlayer = (_player.position - ladderCenter);
        toPlayer.y = 0f;
        _offsetDir = toPlayer.normalized;

        _cc.enabled = false;
        _player.position = new Vector3(
            ladderCenter.x + _offsetDir.x * ladderOffset,
            _player.position.y,
            ladderCenter.z + _offsetDir.z * ladderOffset
        );
        _cc.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StopClimbing(doJump: false);
    }

    void StopClimbing(bool doJump)
    {
        if (!_onLadder) return;
        _onLadder = false;
        _fps.enabled = true;

        if (doJump)
            StartCoroutine(ApplyJumpOff());
    }

    IEnumerator ApplyJumpOff()
    {
        Vector3 dir = (_offsetDir + Vector3.up * 0.5f).normalized * jumpOffForce;
        float t = 0f;
        while (t < 0.2f)
        {
            _cc.Move(dir * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }
}