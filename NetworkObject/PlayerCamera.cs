using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCamera : MonoBehaviour
{
	//[field : SerializeField]
	public float CameraSpeed { get; private set; } = 8.0F;
	//[field : SerializeField]
	public float CameraOffsetZ { get; private set; } = -20;
	private Transform mTarget;

	private Vector3 mLastTargetPosition;
	public Vector3 TargetPosition
	{
		get
		{
			var pos = (mTarget == null) ? mLastTargetPosition : mTarget.position;
			mLastTargetPosition = new Vector3(pos.x, pos.y, CameraOffsetZ);
			return mLastTargetPosition;
		}
	}

	public void LateUpdate()
	{
		transform.position = Vector3.Lerp
		(
			transform.position, 
			TargetPosition, 
			CameraSpeed * Time.deltaTime
		);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			var impulse = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
			AddImpulse(impulse);
		}
	}

	public void AddImpulse(Vector2 impulse)
	{
		transform.position += impulse.ToVector3();
	}

	public void SetTarget(Transform target)
	{
		this.mTarget = target;
	}

	public void RemoveTarget()
	{
		this.mTarget = null;
	}
}
