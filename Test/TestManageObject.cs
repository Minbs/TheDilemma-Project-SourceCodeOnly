using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class TestManageObject : MonoManageObject
{
	[SerializeField] private TestMonoService mTestMonoService;

	public override void OnInitialize()
	{
		base.OnInitialize();

		//GlobalServiceLocator.SceneManageServiceLocator
		//	.GetServiceOrNull<TitleSceneManageService>()?
		//	.TestMonoServiceLocator.RegisterService(mTestMonoService);

		Ulog.Log(this, "OnInitialize");
	}

	public override void OnFinalize()
	{
		Ulog.Log(this, "OnFinalize");

		base.OnFinalize();
	}
}
