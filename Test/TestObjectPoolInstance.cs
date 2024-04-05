using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestObjectPoolInstance : MonoBehaviour
{
	[Button]
	public void ReleaseSelf()
	{
		GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.Release(gameObject);
	}
}
