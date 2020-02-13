using UnityEngine;

public class HeadRotation : MonoBehaviour
{
    /// <summary>Empty gameObject in front of the player that we use the position of to reset our head's rotation.</summary>
    Transform headReset;
    FieldOfView fov;
    PlayerMovement player;

    Transform[] pointsOfInterest;
    Transform closestPointOfInterest;
    float closestPointOfInterestDist;

    Vector3 dir;
    float angle;
    
    void Start()
    {
        fov = GetComponentInParent<FieldOfView>();
        player = FindObjectOfType<PlayerMovement>();
        headReset = fov.transform.Find("Head Reset");
    }
    
    void FixedUpdate()
    {
        if (fov.gameObject.tag == "Player" && player.isLockedOn)
            LookAtLockOnTarget();
        else
            LookAtPointsOfInterest();
    }

    void LookAtPointsOfInterest()
    {
        // TODO: Add other kinds of points of interest (valuable treasure, events, signs, unique locations, etc.)
        if (fov.visibleTargets.Count > 0)
        {
            closestPointOfInterest = null;
            closestPointOfInterestDist = 0;

            foreach (Transform target in fov.visibleTargets)
            {
                float distance = Vector2.Distance(transform.position, target.position);
                if (distance < closestPointOfInterestDist || closestPointOfInterestDist == 0)
                {
                    closestPointOfInterestDist = distance;
                    closestPointOfInterest = target;
                }
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, closestPointOfInterest.position - transform.position), 200f * Time.fixedDeltaTime);
        }
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, headReset.position - transform.position), 200f * Time.fixedDeltaTime);
    }

    void LookAtLockOnTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, player.lockOnTarget.position - transform.position), 200f * Time.fixedDeltaTime);
    }
}
