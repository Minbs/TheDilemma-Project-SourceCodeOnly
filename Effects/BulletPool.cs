using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BulletPool : MonoBehaviour
{
	public int Count => mObjectStack.Count;
	[field : SerializeField] public int InstanceCount { get; set; }
	private Stack<GameObject> mObjectStack = new();
	[field : SerializeField] public GameObject ReferenceInstance { get; private set; }

	public BulletPool()	{}

	private void Start()
	{
		InstantiateObjects();
	}

	public void InstantiateObjects()
	{
		for(int i = 0; i < InstanceCount; i++)
		{
			var go = Instantiate(ReferenceInstance, transform);
			go.SetActive(false);
			mObjectStack.Push(go);
		}
	}

	public GameObject Get(Vector3 position, Quaternion rotation)
	{
		if (mObjectStack.TryPop(out GameObject instance))
		{
			instance.SetActive(true);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			return instance;
		}

		var go = Instantiate(ReferenceInstance, position, rotation, transform);
		go.SetActive(true);
		mObjectStack.Push(go);
		return go;
	}

	public void Release(GameObject instance)
	{
		instance.SetActive(false);
		mObjectStack.Push(instance);
	}

	public void Clear()
	{
		while (mObjectStack.Count > 0)
		{
			var instance = mObjectStack.Pop();
			Destroy(instance);
		}
	}
}