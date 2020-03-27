using UnityEngine;

public class PlayerControls : MonoBehaviour
{
  private MoveInput _moveInput;
  private BezierSolution.BezierRailWalker bezierWalker = null;

  private void Awake()
  {
    _moveInput = FindObjectOfType<MoveInput>();
    bezierWalker = GetComponent<BezierSolution.BezierRailWalker>();
  }

  private void Update()
  {
    HandleInputs();
  }

  private void HandleInputs()
  {
    // Movement
    if (_moveInput.StartMoving())
    {
      bezierWalker.FirstMove();
    }

    if (_moveInput.Moving())
    {
      bezierWalker.Move();
    }

    if (_moveInput.LeftTriggerDown())
    {
      if (_moveInput.GetMoveVertical() == -1)
      {
        bezierWalker.JumpOffPlatform();
      }

      else
      {
        bezierWalker.Jump();
      }
    }

    if (_moveInput.TransitionUp())
    {
      bezierWalker.CheckTransitSplines(TransitionKey.UP);
    }

    if (_moveInput.TransitionDown())
    {
      bezierWalker.CheckTransitSplines(TransitionKey.DOWN);
    }

    if (_moveInput.StopMoving())
    {
      bezierWalker.StopMove();
    }

    if (_moveInput.GetMoveVerticalUp())
    {
      bezierWalker.CancelTransitSpline();
    }

    if (_moveInput.LeftTriggerUp())
    {
      bezierWalker.StopJump();
    }
  }
}
