using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using UnityEngine;

using Sirenix.OdinInspector;
using KaNet.Synchronizers;
using Utils;

namespace NetworkAI
{
	/// <summary>FSM�� ���� ��Ʈ�ѷ��Դϴ�.</summary>
	public abstract class StateController : MonoBehaviour
	{
		[field: SerializeField, Header("�ʱ� ����")]
		public StateGroup InitialCurrentStateGroup { get; protected set; }

		[field: SerializeField, Header("���� ����")]
		public StateGroup CurrentStateGroup { get; protected set; }
		
		[MinMaxSlider(0F, 0.5F), Header("AI ������Ʈ �ֱ�")]
		public Vector2 UpdateIntervalRange = new Vector2(0.125F, 0.25F); //  �ֱ� ������Ʈ�� ���� ���� �ּ�, �ִ� ��
		[field: SerializeField, Header("���� ����Ʈ")]
		public SerializableDictionary<string, StateGroup> StateGroupsList { get; set; } = new();

		public virtual void OnValidate()
		{
			var stateGroups = GetComponentsInChildren<StateGroup>();

			foreach(var state in stateGroups)
			{
				if (StateGroupsList.ContainsKey(state.name))
					continue;

				if(!StateGroupsList.TryAdd(state.name, state))
				{
					Ulog.LogError(this, $"{state.name}�� �ִµ� �����Ͽ����ϴ�.");
				}
			}
		}

		private float mUpdateDelay;
		private float mUpdateInterval;

		public virtual void OnInitialize(DeltaTimeInfo deltaTimeInfo)
		{
			// Initialize state
			CurrentStateGroup = InitialCurrentStateGroup;

			// Initial start
			CurrentStateGroup.OnStart(this, deltaTimeInfo);
		}

		public virtual void OnUpdate(DeltaTimeInfo deltaTimeInfo)
		{
			if (mUpdateDelay < mUpdateInterval)
			{
				mUpdateDelay += deltaTimeInfo.ScaledDeltaTime;
				return;
			}

			DeltaTimeInfo stateDeltaTimeInfo= new DeltaTimeInfo
			(
				mUpdateDelay,
				deltaTimeInfo.GlobalTimeScale
			);

			CurrentStateGroup.OnUpdate(this, stateDeltaTimeInfo);

			mUpdateDelay = 0;
			mUpdateInterval = UpdateIntervalRange.GetRandomFromMinMax();
		}

		public void ChangeState(DeltaTimeInfo deltaTimeInfo, StateGroup stateGroup)
		{
			CurrentStateGroup?.OnEnd(this, deltaTimeInfo);
			CurrentStateGroup = stateGroup;
			CurrentStateGroup.OnStart(this, deltaTimeInfo);
		}
	}
}
