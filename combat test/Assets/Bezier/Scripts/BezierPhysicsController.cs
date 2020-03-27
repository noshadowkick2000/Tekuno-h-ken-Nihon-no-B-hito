using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BezierSolution.BezierRailWalker))]
public class BezierPhysicsController : MonoBehaviour
{
  private BezierSolution.BezierRailWalker bezierWalker;
  private Rigidbody rigidBody;

  private Vector3 tempTargetPosition;
  private Vector3 targetPosition;
  private Vector3 positionOffset;

  private bool awaitingChanges = false;
  private bool changesSinceTempCalculation = false;

  public bool InAir
  {
    get
    {
      return (targetPosition.y - bezierWalker.GroundHeight) > 0.0001f;
    }
  }

  private void Awake()
  {
    bezierWalker = GetComponent<BezierSolution.BezierRailWalker>();
    rigidBody = GetComponent<Rigidbody>();
    rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
  }

  private void FixedUpdate()
  {
    ConsolidateForces();

    rigidBody.MovePosition(targetPosition);
  }

  private void ConsolidateForces()
  {
    targetPosition += positionOffset;

    positionOffset = Vector3.zero;

    awaitingChanges = false;
    changesSinceTempCalculation = false;
  }

  private Vector3 ConsolidateTempForces()
  {
    if (changesSinceTempCalculation)
    {
      tempTargetPosition = targetPosition;
      tempTargetPosition += positionOffset;

      changesSinceTempCalculation = false;

      return tempTargetPosition;
    }

    else
    {
      return tempTargetPosition;
    }
  }

  public void SetTargetBezierPosition(Vector3 targetBezierPosition, bool placeOnGround = false)
  {
    float oldY = targetPosition.y;
    float newGroundHeight = targetBezierPosition.y;

    Vector3 oldPosition = targetBezierPosition;

    // InAir here checks old ground height and old position
    oldPosition.y = placeOnGround ? newGroundHeight : (InAir ? oldY : newGroundHeight);

    targetPosition = oldPosition;
  }

  public void ResetHeight()
  {
    targetPosition.y = bezierWalker.GroundHeight;
  }

  public void AddHeightOffset(float y)
  {
    targetPosition.y += y;

    targetPosition.y = Mathf.Clamp(targetPosition.y, bezierWalker.GroundHeight, targetPosition.y);
  }

  // Mainly used for forces controlled by external scripts, such as jumping
  public void AddPositionOffset(Vector3 offset)
  {
    positionOffset += offset;

    //awaitingChanges = true;
    //changesSinceTempCalculation = true;
  }

  // Mainly used for forces controlled by external scripts, such as jumping
  public void AddPositionOffset( float x, float y, float z )
  {
    positionOffset.x += x;
    positionOffset.y += y;
    positionOffset.z += z;

    awaitingChanges = true;
    changesSinceTempCalculation = true;
  }

  public void AddDissipatingForce(Vector3 force, float magnitudeDissipationRate)
  {
    awaitingChanges = true;
    changesSinceTempCalculation = true;
  }

  public Vector3 GetPosition()
  {
    if (awaitingChanges)
    {
      return ConsolidateTempForces();
    }

    else
    {
      return targetPosition;
    }
  }
}
