using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

/// <summary>네트워크 객체의 표현을 담당하는 Proxy클래스입니다.</summary>
public abstract class ProxyBase : MonoBehaviour
{
	public virtual void OnValidate()
	{
		Renderers = GetComponentsInChildren<Renderer>();
	}

	[field: SerializeField] public Renderer[] Renderers { get; private set; }

	public virtual void OnUpdate(in DeltaTimeInfo deltaTimeInfo)
	{
		if (transform.hasChanged)
		{
			int baseSortOrder = Global.RoundByDepthOrderOffset(transform);
			foreach (var renderer in Renderers)
			{
				renderer.sortingOrder = baseSortOrder;
			}
		}
	}
}
