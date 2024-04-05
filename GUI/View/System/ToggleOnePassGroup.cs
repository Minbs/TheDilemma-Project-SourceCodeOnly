using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;

/// <summary>
/// 해당 그룹에 들어온 토글 중에 무조건 하나만 참이 될 수 있습니다.
/// </summary>
[Serializable]
public class ToggleOnePassGroup
{
	private List<ToggleViewModel> mGroupList = new List<ToggleViewModel>();

	public void Add(ToggleViewModel toggle)
	{
		toggle.AddAction((onValueChange) =>
		{
			if (!onValueChange)
				return;

			foreach (var item in mGroupList)
			{
				if (!item.Equals(toggle))
				{
					item.IsOn = false;
				}
			}
		});

		mGroupList.Add(toggle);
	}

	/// <summary>그룹 내에 있는 토글을 모두 false 합니다.</summary>
	public void Reset()
	{
		foreach (var item in mGroupList)
		{
			item.IsOn = false;
		}
	}

}
