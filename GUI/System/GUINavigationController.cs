using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GUINavigationController : MonoBehaviour
{
	private bool mIsSwitchingNavigation = false;

	public GUINavigation Current { get; private set; } = null;

	private Dictionary<GUINavigationType, GUINavigation> mNavigationTable = new();

	public void Init()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			var childTransform = transform.GetChild(i);

			if (childTransform.TryGetComponent<GUINavigation>(out var uiNavigation))
			{
				uiNavigation.Init(this);
				mNavigationTable.Add(uiNavigation.Type, uiNavigation);
			}
		}
	}

	public void Add(GUINavigationType type, GUINavigation navigation)
	{
		mNavigationTable.Add(type, navigation);
	}

	public void Change(GUINavigationType type)
	{
		if (mIsSwitchingNavigation)
			return;

		if (!mNavigationTable.TryGetValue(type, out var nextNavigation))
		{
			Ulog.LogError(UlogType.UI, $"This NavigationController don't have Navigation :{type}");
			return;
		}

		StartCoroutine(Switch(nextNavigation));
	}

	private IEnumerator Switch(GUINavigation nextNavigation)
	{
		mIsSwitchingNavigation = true;

		Current?.Hide();

		var hideNavigation = Current;

		Current = nextNavigation;

		while (hideNavigation?.State == VisableState.Disappearing)
			yield return null;

		Current.Show();

		mIsSwitchingNavigation = false;
	}
}
