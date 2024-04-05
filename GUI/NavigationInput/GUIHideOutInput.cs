using UnityEngine;

public class GUIHideOutInput : MonoBehaviour
{
	[SerializeField] private GUINavigationController mNavigationController;

	private void Start()
	{
		mNavigationController.Init();
		mNavigationController.Change(GUINavigationType._Loadout);
		mNavigationController.Current.Push<View_UserLoadout>();
	}
}
