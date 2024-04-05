#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class SortOrderPrebinder : MonoBehaviour
{
	private void OnValidate()
	//private void FixedUpdate()
	{
		int baseSortOrder = Global.RoundByDepthOrderOffset(transform);
		var childrenRenderer = transform.GetComponentsInChildren<Renderer>();
		foreach (var renderer in childrenRenderer)
		{
			renderer.sortingOrder = baseSortOrder;
		}
	}
}

#endif