using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using KaNet.Synchronizers;
using UnityEngine;
using Utils;
using Utils.Service;

public class PhysicsTestScene : SceneManageService
{
	[field: SerializeField] public SceneType CurrentScene { get; private set; } = SceneType.None;
	public Transform bulletPoolTransform;

	protected override void bindAllService()
	{
		//this.bindService(MonoObjectPoolService);
	}

	protected override void onInitialize()
	{
		// Setup Object Pool Service
		GlobalServiceLocator.MonoObjectPoolService.RegisterService(new MonoObjectPoolService(bulletPoolTransform));
		var a = GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull();
		if (a == null)
			Debug.Log("null");
	}
}
