using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestObjectPoolController : MonoBehaviour
{
	public GameObject TestPrefab_1;
	public GameObject TestPrefab_2;
	public GameObject TestPrefab_3;
	public GameObject TestPrefab_4;
	public GameObject TestPrefab_5;

	[Button]
	public void CreatePrefab_1() => GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.CreateObject(TestPrefab_1, Vector3.zero, Quaternion.identity);

	[Button]
	public void CreatePrefab_2() => GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.CreateObject(TestPrefab_2, Vector3.zero, Quaternion.identity);

	[Button]
	public void CreatePrefab_3() => GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.CreateObject(TestPrefab_3, Vector3.zero, Quaternion.identity);

	[Button]
	public void CreatePrefab_4() => GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.CreateObject(TestPrefab_4, Vector3.zero, Quaternion.identity);

	[Button]
	public void CreatePrefab_5() => GlobalServiceLocator.MonoObjectPoolService.GetServiceOrNull()?.CreateObject(TestPrefab_5, Vector3.zero, Quaternion.identity);
}
