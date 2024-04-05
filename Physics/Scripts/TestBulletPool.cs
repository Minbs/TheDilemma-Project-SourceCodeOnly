using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Service;

public class TestBulletPool : MonoBehaviour
{
	public GameObject bullet1;
	public GameObject laser;
	public GameObject Effect_BulletMark;

	private bool isInit = false;

	private MonoObjectPoolService mMonoObjectPool;

	private Dictionary<Type, GameObject> mPrefabByType = new();

	public void Start()
	{
		mPrefabByType.Add(typeof(TestBullet), bullet1);
		mPrefabByType.Add(typeof(LaserEffect), laser);
		mPrefabByType.Add(typeof(Effect_HitscanLaser), Effect_BulletMark);

		GlobalServiceLocator.MonoObjectPoolService.RegisterService(new MonoObjectPoolService(transform));
		mMonoObjectPool = GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull();
	}

	public T GetObejct<T>(Vector3 position) where T : MonoBehaviour
	{
		var go = mMonoObjectPool.CreateObject(mPrefabByType[typeof(T)], position, Quaternion.identity);
		return go.GetComponent<T>();
	}
}
