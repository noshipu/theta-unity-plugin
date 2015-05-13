using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraControl : MonoBehaviour
{
	private float rotateSpeed = 0.1f;
	private float upDownSpeed = 0.1f;
	
	void Update()
	{
		int touchCount = Input.touches.Count(t => t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled);
		if (touchCount == 1)
		{
			Touch touch = Input.touches.First();
			switch (touch.phase)
			{
			case TouchPhase.Moved:
				
				//移動量
				float xDelta = touch.deltaPosition.x * rotateSpeed;
				float yDelta = touch.deltaPosition.y * upDownSpeed;
				
				//camera
				transform.Rotate(0, -xDelta, 0, Space.World);
				transform.Rotate(yDelta, 0, 0, Space.Self);
				break;
			}
		}
	}
}
