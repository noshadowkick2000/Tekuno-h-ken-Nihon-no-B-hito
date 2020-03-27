using UnityEngine;

public class SpriteLookAt : MonoBehaviour
{
  private CharacterCameraTrack mainCamera;

  private void Awake()
  {
    mainCamera = Camera.main.GetComponent<CharacterCameraTrack>();
  }

  private void LateUpdate()
  {
    if (mainCamera.transform.hasChanged)
    {
      transform.LookAt(mainCamera.transform);
    }
  }
}
