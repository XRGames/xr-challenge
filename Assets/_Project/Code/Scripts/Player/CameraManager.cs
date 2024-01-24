using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  public Transform playerHead;

  void Update()
  {
    transform.position = playerHead.transform.position;
  }
}
