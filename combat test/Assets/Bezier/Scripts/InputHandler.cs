using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
  public static event Action<KeyCode> OnKeyPressed;

  private void Update()
  {

  }
}

public class Buttons
{
  public const string Horizontal = "MoveHorizontal";
  public const string Vertical = "MoveVertical";
  public const string Jump = "Jump";
  public const string UpAction = "Up Action";
  public const string DownAction = "Down Action";
}