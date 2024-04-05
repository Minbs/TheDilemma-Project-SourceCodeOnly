using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using Utils.ViewModel;

public class Entity_TestEnemy : Entity_Remote
{
	private Vector3 mStartPosition;
	private float mMoveFactor;

	public override NetObjectType Type => NetObjectType.Entity_TestEnemy;

	public override void Server_OnStart()
	{
		base.Server_OnStart();
		mStartPosition = transform.position;
	}

	public override void Server_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		base.Server_OnUpdate(deltaTimeInfo);

		mMoveFactor += deltaTimeInfo.ScaledDeltaTime * 5;
		transform.position = mStartPosition + Vector3.up * Mathf.Sin(mMoveFactor);
	}
}
