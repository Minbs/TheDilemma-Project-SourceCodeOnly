using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Service;

public class GlobalMonoObjectPoolService : MonoBehaviour
{
	private MonoObjectPoolService mMonoObjectPoolService;

	public void Start()
	{
		mMonoObjectPoolService = new MonoObjectPoolService(transform);
		GlobalServiceLocator.MonoObjectPoolService.RegisterService(mMonoObjectPoolService);
	}
}
