using UnityEngine;

/// <summary>
/// Attach to your Ladder GameObject.
/// The ladder Collider must have "Is Trigger" checked.
/// Player must have the tag "Player".
/// </summary>
[RequireComponent(typeof(Collider))]
public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 4f;

    private FPSController _fps;
    private CharacterController _cc;
    private bool _onLadder;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _fps = other.GetComponent<FPSController>();
        _cc = other.GetComponent<CharacterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || _fps == null) return;

        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical != 0f)
        {
            // Disable FPSController so its gravity/movement doesn't interfere
            if (!_onLadder)
            {
                _onLadder = true;
                _fps.enabled = false;
            }

            _cc.Move(Vector3.up * vertical * climbSpeed * Time.deltaTime);
        }
        else if (_onLadder)
        {
            // Player stopped pressing - hold them in place (no gravity)
            // Just don't move; FPSController is still disabled so they won't fall
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ExitLadder();
    }

    private void ExitLadder()
    {
        if (_fps != null) _fps.enabled = true;
        _onLadder = false;
    }
}