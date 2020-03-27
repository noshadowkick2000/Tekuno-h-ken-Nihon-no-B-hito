using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace BezierSolution
{
  [RequireComponent(typeof(BezierPhysicsController))]
  public class BezierRailWalker : BezierWalker
	{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float runWindupTime = 0.75f;
    [SerializeField] private float runAccelerationTime = 2.25f;

    private float timeSpentMoving = 0;

    [Header("In Air Movement")]
    [SerializeField] private float airFrictionModifier = 1.8f;
    [SerializeField] private float airMoveModifier = 0.8f;
    [SerializeField] private float airBoostModifier = 0.7f;

    [Header("Jumping")]
    [SerializeField] private float standardJumpHeight = 2f;
    [SerializeField] private float maxJumpHeight = 2.5f;
    [SerializeField] private float gravity = 30f;

    private float initialJumpVelocity;

    private float airFriction;
    private float airMoveForce;
    private float airBoost;

    private const float SPLINE_COOLDOWN = 0.5f;

    private float moveSpeed;
    private float finalSpeed;

    private float jumpAcceleration = 0f;
    private bool firstDirectionMoveMadeInAir = false;
    private float airMoveSpeed;
    private float airMoveSpeedLimit;

    float totalHeight = 0;

    public float GroundHeight { get; private set; }

    // Transitions
    public bool InTransition { get; private set; } = false;
    private float beforeTransitionT;
    private BezierSpline beforeTransitionSpline;

    // Holds the previous spline the user was on so it does not automatically switch back to this. Resets to null after SPLINE_COOLDOWN amount of time or a new action is made
    private BezierSpline previousSpline = null;
    private float previousSplineCooldown = 0;

    [Space(8)]
    public BezierSpline spline;

    [SerializeField]
		[Range( 0f, 1f )]
		private float m_normalizedT = 0f;

		public override BezierSpline Spline { get { return spline; } }

		public override float NormalizedT
		{
			get { return m_normalizedT; }
			set { m_normalizedT = value; }
		}

		private bool isGoingForward = true;
		public override bool MovingForward { get { return ( finalSpeed > 0f ) == isGoingForward; } }

    // Spline data
    public int numTransitions = 0;
    public TransitionSpline[] transitionData;
    public List<TransitionSpline> validTransitions = new List<TransitionSpline>();
    public TransitionSpline currentlyTransitioning;

    // Component parts
    private BezierPhysicsController physicsController = null;
    
    //input
    private MoveInput _moveInput;

    public struct BezierPointTuple
    {
      public readonly BezierPoint point1, point2;
      public readonly float t;

      public BezierPointTuple(BezierPoint point1, BezierPoint point2, float t)
      {
        this.point1 = point1;
        this.point2 = point2;
        this.t = t;
      }
    }

    private void Awake()
    {
      physicsController = GetComponent<BezierPhysicsController>();

      _moveInput = FindObjectOfType<MoveInput>();
      _animationMover = GetComponentInChildren<AnimationMover>();

      moveSpeed = baseSpeed;

      airFriction = baseSpeed * airFrictionModifier;
      airMoveForce = (baseSpeed * airMoveModifier) + airFriction;
      airBoost = baseSpeed * airBoostModifier;

      UpdateSplineInformation();
    }

    private void Start()
    {
      InitialisePosition();
    }

    private void Update()
    {
      SplineCooldown();
      InAirPhysics(Time.deltaTime);

      CalculateNewPosition();
    }

    public Quaternion GetForwardDirection()
    {
      return Quaternion.LookRotation(spline.GetTangent(m_normalizedT));
    }

    /************************************************ PUBLIC FUNCTIONS ************************************************/

    public void FirstMove()
    {
      if (physicsController.InAir)
      {
        if (!firstDirectionMoveMadeInAir && airMoveSpeed == 0)
        {
          isGoingForward = _moveInput.GetMoveHorizontal() >= 0 ? true : false;

          firstDirectionMoveMadeInAir = true;
        }

        if ((_moveInput.GetMoveHorizontal() > 0 && isGoingForward) || (_moveInput.GetMoveHorizontal() < 0 && !isGoingForward))
        {
          airMoveSpeed += airBoost;
        }
      }

      else
      {
        timeSpentMoving = 0;
      }

      previousSpline = null;
    }

    public void Move()
    {
      if (!InTransition)
      {
        if (!physicsController.InAir)
        {
          GroundMove();
        }
        
        else
        {
          InAirMove();
        }
      }

      else if (InTransition)
      {
        GroundMove(true);
      }

      UpdateAvailableTransitions();
    }

    private void GroundMove(bool ignoreInput = false)
    {
      timeSpentMoving += Time.deltaTime;
      float timeInAcceleration = timeSpentMoving - runWindupTime;

      moveSpeed = Mathf.Lerp(baseSpeed, runSpeed, timeInAcceleration / runAccelerationTime);

      if (!ignoreInput)
      {
        isGoingForward = _moveInput.GetMoveHorizontal() >= 0 ? true : false;
      }

      finalSpeed = moveSpeed;
    }

    private void InAirMove()
    {
      float modifier = isGoingForward ? 1f : -1f;
      float horizontalAxisValue = _moveInput.GetMoveHorizontal();

      airMoveSpeed += horizontalAxisValue * airMoveForce * modifier * Time.deltaTime;

      finalSpeed = airMoveSpeed;
    }

    public void StopMove()
    {
      moveSpeed = baseSpeed;
      timeSpentMoving = 0;
    }

    public void Jump()
    {
      // If player is on the ground
      if (!InTransition && !physicsController.InAir)
      {
        // Player jumps higher the faster they run
        float t = (moveSpeed - baseSpeed) / (runSpeed - baseSpeed);
        float currentJumpHeight = Mathf.Lerp(standardJumpHeight, maxJumpHeight, t);
        initialJumpVelocity = Mathf.Sqrt(-2 * -gravity * currentJumpHeight);

        jumpAcceleration += initialJumpVelocity;
        physicsController.AddHeightOffset(0.001f);

        firstDirectionMoveMadeInAir = false;

        airMoveSpeed = finalSpeed;
        airMoveSpeedLimit = airMoveSpeed < baseSpeed ? baseSpeed : airMoveSpeed;

        previousSpline = null;
      }
    }

    public void JumpOffPlatform()
    {
      if (spline.splineType == SplineType.Platform)
      {
        if (!CheckForSplineBelow(MovingForward))
        {
          Jump();
        }
      }

      else
      {
        Jump();
      }
    }

    public void StopJump()
    {
      if (physicsController.InAir)
      {
        jumpAcceleration = jumpAcceleration > 0 ? 0 : jumpAcceleration;
      }
    }

    private void InAirPhysics(float deltaTime)
    {
      if (physicsController.InAir)
      {
        totalHeight += jumpAcceleration * deltaTime;

        physicsController.AddHeightOffset(jumpAcceleration * deltaTime);

        // Slows down the jump speed over time
        jumpAcceleration -= gravity * deltaTime;

        // Slows down the player movement over time
        airMoveSpeed -= airFriction * deltaTime;
        airMoveSpeed = Mathf.Clamp(airMoveSpeed, 0, airMoveSpeedLimit);

        finalSpeed = airMoveSpeed;

        CheckForPlatform();
      }

      else
      {
        physicsController.ResetHeight();
        jumpAcceleration = 0;
        airMoveSpeed = 0;
        totalHeight = 0;
      }
    }

    private void CalculateNewPosition()
    {
      Vector3 newPosition;
      
      //TESTING---------------------------------------------------------------------------------------------------------
      newPosition = animating ? ExecuteAnimation(_animationMover.GetDelta()) : Execute(Time.deltaTime);
      
      physicsController.SetTargetBezierPosition(newPosition);

      // This MUST be AFTER physicsController.SetTargetBezierPosition otherwise physicsController's InAir would check against new GroundHeight but old position.y
      GroundHeight = newPosition.y;
      
      finalSpeed = 0;
    }

    public override Vector3 Execute(float deltaTime)
    {
      float oldNormalisedT = m_normalizedT;
      float targetSpeed = (isGoingForward) ? finalSpeed : -finalSpeed;
      targetSpeed *= deltaTime;

      Vector3 targetPos = spline.MoveAlongSpline(ref m_normalizedT, targetSpeed);

      bool movingForward = MovingForward;

      if (!InTransition)
      {
        if (movingForward)
        {
          if (m_normalizedT >= 1f)
          {
            if (SwitchToLinkedSpline(movingForward))
            {
              targetPos = OvershootSpline(movingForward, targetSpeed, oldNormalisedT);
            }

            else if (CheckForSplineBelow(movingForward))
            {
              targetPos = OvershootSpline(movingForward, targetSpeed, oldNormalisedT);
              airMoveSpeed = moveSpeed;
            }

            else
            {
              m_normalizedT = 1f;
            }
          }
        }
        else
        {
          if (m_normalizedT <= 0f)
          {
            if (SwitchToLinkedSpline(movingForward))
            {
              targetPos = OvershootSpline(movingForward, targetSpeed, oldNormalisedT);
            }

            else if (CheckForSplineBelow(movingForward))
            {
              targetPos = OvershootSpline(movingForward, targetSpeed, oldNormalisedT);
              airMoveSpeed = moveSpeed;
            }

            else
            {
              m_normalizedT = 0f;
            }
          }
        }
      }

      return targetPos;
    }

    private bool SwitchToLinkedSpline(bool movingForward)
    {
      BezierPoint point = null;

      if (movingForward)
      {
        point = spline[spline.Count - 1];
      }

      else
      {
        point = spline[0];
      }

      if (point.linkedPoint != null)
      {
        BezierSpline newSpline = point.linkedPoint.GetComponentInParent<BezierSpline>();

        SwitchSplines(newSpline, GroundHeight);

        return true;
      }

      return false;
    }

    private bool CheckForSplineBelow(bool movingForward)
    {
      BezierSpline splineBelow = null;
      splineBelow = FindClosestAlignedSplineBelow(out float newT, out float newGroundHeight);

      if (splineBelow != null)
      {
        SwitchSplines(splineBelow, newGroundHeight, newT);

        return true;
      }

      return false;
    }

    private BezierSpline FindClosestAlignedSplineBelow(out float normalisedT, out float newGroundHeight)
    {
      BezierSpline[] allSplines = SplineCacher.GetAllSplines();
      BezierSpline closestSpline = null;
      float closestDistance = Mathf.Infinity;
      newGroundHeight = 0;

      normalisedT = -1f;

      foreach (BezierSpline individualSpline in allSplines)
      {
        if (individualSpline != spline && individualSpline != previousSpline)
        {
          float tempT;

          Vector3 nearestPoint = individualSpline.FindNearestPointOnXZPlane(physicsController.GetPosition(), out tempT);

          // Check if x and z values are aligned
          if (Utils.FP.IsEqual(physicsController.GetPosition().x, nearestPoint.x) && Utils.FP.IsEqual(physicsController.GetPosition().z, nearestPoint.z))
          {
            // Check if the player's y is above the spline
            if (physicsController.GetPosition().y >= nearestPoint.y)
            {
              float heightDistance = physicsController.GetPosition().y - nearestPoint.y;

              if (heightDistance < closestDistance)
              {
                closestDistance = heightDistance;
                closestSpline = individualSpline;

                newGroundHeight = nearestPoint.y;

                normalisedT = tempT;
              }
            }
          }
        }
      }

      return closestSpline;
    }

    private Vector3 OvershootSpline(bool movingForward, float targetSpeed, float oldNormalisedT)
    {
      finalSpeed = movingForward ? targetSpeed - previousSpline.GetLengthApproximately(oldNormalisedT, 1f, 5f) : -targetSpeed - previousSpline.GetLengthApproximately(0, oldNormalisedT, 5f);

      return Execute(1f);
    }

    private void CheckForPlatform()
    {
      BezierSpline[] platformSplines = SplineCacher.GetAllPlatformSplines();

      foreach (BezierSpline platformSpline in platformSplines)
      {
        if (platformSpline != spline && platformSpline != previousSpline)
        {
          Vector3 nearestPoint = platformSpline.FindNearestPointOnXZPlane(physicsController.GetPosition(), out float t);

          //Debug.Log(nearestPoint.ToString("F10") + " " + targetBezierPosition.ToString("F10"));

          if (Utils.FP.IsEqual(physicsController.GetPosition().x, nearestPoint.x) && Utils.FP.IsEqual(physicsController.GetPosition().z, nearestPoint.z))
          {
            // Check if the player's y is above it
            if (physicsController.GetPosition().y >= nearestPoint.y)
            {
              SwitchSplines(platformSpline, nearestPoint.y, t);

              return;
            }
          }
        }
      }
    }

    //private Quaternion GetTargetRotation()
    //{
    //  return isGoingForward ? Quaternion.LookRotation(spline.GetTangent(m_normalizedT)) : Quaternion.LookRotation(spline.GetTangent(-m_normalizedT));
    //}

    public BezierPointTuple GetCorrespondingPoints()
    {
      var pointTupleIndex = spline.GetNearestPointIndicesTo(m_normalizedT);

      return new BezierPointTuple(spline[pointTupleIndex.index1], spline[pointTupleIndex.index2], pointTupleIndex.t);
    }

    public void CheckTransitSplines(TransitionKey key)
    {
      // There is another spline the player can currently transit to
      if (validTransitions.Count > 0)
      {
        if (!physicsController.InAir)
        {
          // Check if the key being pressed matches with any of the valid transitions
          for (int i = 0; i < validTransitions.Count; ++i)
          {
            if (key == validTransitions[i].transitionKey)
            {
              if (!InTransition || (InTransition && validTransitions[i].transitionKey != currentlyTransitioning.transitionKey))
              {
                currentlyTransitioning = validTransitions[i];
                beforeTransitionSpline = spline;
                beforeTransitionT = m_normalizedT;
                InTransition = true;
              }

              // Transition using this transition spline
              StopAllCoroutines();
              StartCoroutine("StartTransition", currentlyTransitioning);
            }
          } // for
        }
      } // if validTransitions Count
    } // func

    public void CancelTransitSpline()
    {
      if (InTransition)
      {
        StopAllCoroutines();
        StartCoroutine("CancelTransition", currentlyTransitioning);
      }
    }

    /************************************************ PRIVATE FUNCTIONS ************************************************/

    /// <summary>
    /// Only to be called on awake, initialises and sets the first position of the player on the spline
    /// </summary>
    private void InitialisePosition()
    {
      finalSpeed = 0;

      physicsController.SetTargetBezierPosition(Execute(0), true);
    }

    private void SplineCooldown()
    {
      if (previousSplineCooldown < SPLINE_COOLDOWN)
      {
        previousSplineCooldown += Time.deltaTime;
      }

      else
      {
        previousSplineCooldown = 0;
        previousSpline = null;
      }
    }

    private void UpdateSplineInformation()
    {
      numTransitions = spline.transitionSplines.Length;

      transitionData = spline.transitionSplines;
      validTransitions.Clear();
    }

    private void UpdateAvailableTransitions()
    {
      for (int i = 0; i < transitionData.Length; ++i)
      {
        if (transitionData[i].NormalisedTIsInRegion(m_normalizedT))
        {
          if (!validTransitions.Contains(transitionData[i]))
          {
            validTransitions.Add(transitionData[i]);
          }
        }

        else
        {
          validTransitions.Remove(transitionData[i]);
        }
      }
    }

    private void SwitchSplines( BezierSpline newSpline, float newGroundHeight )
    {
      previousSpline = spline;
      spline = newSpline;
      GroundHeight = newGroundHeight;

      // Find the new normalised T
      spline.FindNearestPointTo(transform.position, out m_normalizedT);

      UpdateSplineInformation();
    }

    private void SwitchSplines( BezierSpline newSpline, float newGroundHeight, float t )
    {
      previousSpline = spline;
      spline = newSpline;
      GroundHeight = newGroundHeight;

      m_normalizedT = t;

      UpdateSplineInformation();
    }

    private IEnumerator StartTransition(TransitionSpline transitionData)
    {
      bool destinationReached = false;
      bool contactPointReached = false;

      float targetNormalisedT = 0;

      // Check if it has already reached the contact point (the spline would be the destination spline already)
      if (spline == transitionData.destinationSpline)
      {
        contactPointReached = true;

        // Target T is simply the destination T
        targetNormalisedT = transitionData.destinationT;
      }

      // Still has to reach contact point
      else
      {
        // Target T is the contact point's T
        spline.FindNearestPointTo(transitionData.contactPoint.position, out targetNormalisedT);
      }

      moveSpeed = moveSpeed < baseSpeed ? baseSpeed : moveSpeed;

      // Loop until player has reached its destination (the transition spline T)
      while (!destinationReached)
      {
        // Moves the player towards the target T
        yield return TransitionMove(targetNormalisedT);

        // Once it reaches here, player has arrived at target T
        // Contact point not reached yet
        if (!contactPointReached)
        {
          // Then switch to the destination spline
          contactPointReached = true;

          SwitchSplines(transitionData.destinationSpline, GroundHeight);

          targetNormalisedT = transitionData.destinationT;
        }

        // Contact point already reached, so destination is reached
        else
        {
          destinationReached = true;
        }
      } // contactPointReached

      InTransition = false;
    } // func

    private IEnumerator CancelTransition(TransitionSpline transitionData)
    {
      // If player reached the point where transition first started
      bool departurePointReached = false;
      // If player already reached the contact point
      bool contactPointReached = false;
      float targetNormalisedT = 0;

      // Check if it has already reached the contact point (the spline would be the destination spline already)
      if (spline == transitionData.destinationSpline)
      {
        // Reverse of StartTransition, if contact point already reached, player has to first reach the contact point then departure point
        contactPointReached = true;

        spline.FindNearestPointTo(transitionData.contactPoint.position, out targetNormalisedT);
      }

      else
      {
        targetNormalisedT = beforeTransitionT;
      }

      // Loop until transition complete
      while (!departurePointReached)
      {
        // Moves the player towards the target T
        yield return TransitionMove(targetNormalisedT);

        // Reverse
        if (contactPointReached)
        {
          contactPointReached = false;

          SwitchSplines(beforeTransitionSpline, GroundHeight);

          targetNormalisedT = beforeTransitionT;
        }

        else
        {
          departurePointReached = true;
        }
      } // contactPointReached

      InTransition = false;
    } // func

    private IEnumerator TransitionMove(float targetNormalisedT)
    {
      if (m_normalizedT < targetNormalisedT)
      {
        isGoingForward = true;

        while (m_normalizedT < targetNormalisedT)
        {
          Move();

          yield return null;
        }
      }

      else
      {
        isGoingForward = false;

        while (m_normalizedT > targetNormalisedT)
        {
          Move();

          yield return null;
        }
      }
    }
    
    /************************************************ TESTING FUNCTIONS ************************************************/
    
    public Vector3 ExecuteAnimation(float deltaPosition)
    {
      float oldNormalisedT = m_normalizedT;

      Vector3 targetPos = spline.MoveAlongSplineAnimation(_animationMover.baseValue, ref m_normalizedT, deltaPosition);

      bool movingForward = MovingForward;

      if (!InTransition)
      {
        if (movingForward)
        {
          if (m_normalizedT >= 1f)
          {
            if (SwitchToLinkedSpline(movingForward))
            {
              targetPos = OvershootSpline(movingForward, deltaPosition, oldNormalisedT);
            }

            else if (CheckForSplineBelow(movingForward))
            {
              targetPos = OvershootSpline(movingForward, deltaPosition, oldNormalisedT);
              airMoveSpeed = moveSpeed;
            }

            else
            {
              m_normalizedT = 1f;
            }
          }
        }
        else
        {
          if (m_normalizedT <= 0f)
          {
            if (SwitchToLinkedSpline(movingForward))
            {
              targetPos = OvershootSpline(movingForward, deltaPosition, oldNormalisedT);
            }

            else if (CheckForSplineBelow(movingForward))
            {
              targetPos = OvershootSpline(movingForward, deltaPosition, oldNormalisedT);
              airMoveSpeed = moveSpeed;
            }

            else
            {
              m_normalizedT = 0f;
            }
          }
        }
      }

      return targetPos;
    }

    public bool animating;
    private AnimationMover _animationMover;

    public Vector3 GetMoveClosestSpline(Vector3 otherLocation)
    {
      return spline.FindNearestPointTo(otherLocation, out m_normalizedT);
    }

  } // class
}
 