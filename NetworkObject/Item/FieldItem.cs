using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

using KaNet;
using KaNet.Synchronizers;
using KaNet.Synchronizers.Prebinder;
using KaNet.Utils;

using Sirenix.OdinInspector;
using System;

namespace Gameplay.Legacy
{
	[Obsolete("더 이상 사용하지 않음")]
	public class FieldItem : NetworkObject
	{
		/// <summary>네트워크 객체의 고유 타입을 나타냅니다.</summary>
		public override NetObjectType Type => NetObjectType.Item_FieldItem;

		// Synchronize Position
		[SyncVar(SyncType.UnreliableFixed, SyncAuthority.ServerOnly)]
		private readonly SyncFieldByOrder<NetVector3> mPositionByServer = new();

		public ItemBase ItemData { get; protected set; }

		public override void Common_OnStart()
		{
			mPositionByServer.ResetTimestamp();
			mPositionByServer.ResetDeserializeEvent();
		}

		public override void Client_OnUpdate(in DeltaTimeInfo deltaTimeInfo)
		{
			if (IsServerSide)
			{
				mPositionByServer.Data = transform.position;
			}
			else
			{
				transform.position = Vector3.Lerp
				(
					transform.position,
					mPositionByServer.Data,
					KaNetGlobal.NETWORK_INTERPOLATION_VALUE * deltaTimeInfo.DeltaTime
				);
			}
		}

		public void Initialize(ItemBase itemData)
		{
			ItemData = itemData;
		}
	}
}
