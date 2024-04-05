using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;


public class GUINavigation : MonoBehaviour
{
	[field: SerializeField]
	public GUINavigationType Type { private set; get; }



	private Dictionary<Type, GUINavigationView> mViewTable = new Dictionary<Type, GUINavigationView>();
	private Stack<GUINavigationView> mHistroy = new Stack<GUINavigationView>();

	public VisableState State { set; get; } = VisableState.None;

	public GUINavigationView CurrentView { get; private set; }

	public GUINavigationController NavigationController { private set; get; }

	private GUINavigationSequenceGenerator mSequenceGenerator;

	private readonly string masterViewName = "MasterView";

	public bool navigationEnable
	{
		set
		{
			gameObject.SetActive(value);
		}
		get
		{
			return gameObject.activeSelf;
		}
	}

	public void Init(GUINavigationController controller)
	{
		#region None type error cheack

		if (Type == GUINavigationType.None)
		{
			Ulog.LogError(UlogType.UI, $"The Navigation Type is None :{gameObject.name}");
		}

		#endregion

		#region Navigation view setting

		Transform masterTransform = transform.GetChild(0);
		if (masterTransform == null || !masterTransform.gameObject.name.Equals(masterViewName))
		{
			Ulog.LogError(UlogType.UI, $"FindChild Failed! The {masterViewName} does not exists");
			return;
		}

		for (int i = 0; i < masterTransform.childCount; i++)
		{
			var childTransform = masterTransform.GetChild(i);
			if (childTransform.TryGetComponent<GUINavigationView>(out var naviView))
			{
				naviView.NaigationViewSetup(this);

				Type guiType = naviView.GetType();

				if (!mViewTable.TryAdd(guiType, naviView))
				{
					Ulog.LogError(UlogType.UI, $"The same view type exists : {guiType.Name}, GameObject{naviView.gameObject}");
				}
			}
		}

		#endregion

		if (TryGetComponent<GUINavigationSequenceGenerator>(out var generator))
		{
			generator.Initialize(this);
			mSequenceGenerator = generator;
		}

		CurrentView = null;
		NavigationController = controller;
		close();
	}

	public void Show(Action callback = null)
	{
		if (mSequenceGenerator == null || !mSequenceGenerator.IsShowAnimatnioAvailable || CurrentView == null)
		{
			open(callback);
		}
		else
		{
			mSequenceGenerator.PlayShow(callback);
		}
	}

	public void Hide(Action callback = null)
	{
		if (mSequenceGenerator == null || !mSequenceGenerator.IsHideAnimationAvailable || CurrentView == null)
		{
			close(callback);
		}
		else
		{
			mSequenceGenerator.PlayHide(callback);
		}
	}

	private void close(Action callback = null)
	{
		callback?.Invoke();
		State = VisableState.Disappered;
		navigationEnable = false;
	}

	private void open(Action callback = null)
	{
		callback?.Invoke();
		State = VisableState.Appeared;
		navigationEnable = true;
	}

	#region Pop

	public bool TryPop<T>(out T topView) where T : GUINavigationView
	{
		bool result = TryPop(out var popedView);

		topView = popedView as T;
		return result;
	}

	public bool TryPop(out GUINavigationView topView)
	{
		if (CurrentView == null ||
			CurrentView.IsPlaying ||
			!mHistroy.TryPop(out var popView))
		{
			topView = CurrentView;
			return false;
		}

		mHistroy.TryPeek(out var lastView);

		StartCoroutine(hideWaitingShow(popView, lastView));

		CurrentView = lastView;
		topView = CurrentView;
		return true;
	}

	#endregion

	#region Push

	public void Push<T>() where T : GUINavigationView
	{
		TryPush<T>(out var pushView);
	}

	public bool TryPush<T>(out T pushedView) where T : GUINavigationView
	{
		Type guiType = typeof(T);

		if (mViewTable.TryGetValue(guiType, out var pushView))
		{
			if (!navigationEnable && mViewTable != null)
			{
				CurrentView?.Close();
				mHistroy.Push(pushView);
				pushView.Open();
				CurrentView = pushView;
				pushedView = CurrentView as T;
				return pushedView != null;
			}

			if (CurrentView?.State == VisableState.Disappearing || CurrentView?.State == VisableState.Appearing)
			{
				pushedView = null;
				return false;
			}

			mHistroy.TryPeek(out var topView);

			StartCoroutine(hideWaitingShow(topView, pushView));

			mHistroy.Push(pushView);
			CurrentView = pushView;
			pushedView = CurrentView as T;
			return pushedView != null;
		}

		logDoNotHaveUI(guiType);
		pushedView = null;
		return false;
	}

	#endregion

	private IEnumerator hideWaitingShow(GUINavigationView topView, GUINavigationView showView)
	{
		topView?.Hide();

		while (topView?.State == VisableState.Disappearing)
			yield return null;

		if (showView != null)
		{
			showView.Show();
		}
	}

	public bool TryGetValue<T>(out T view) where T : GUINavigationView
	{
		Type guiType = typeof(T);

		if (!mViewTable.TryGetValue(guiType, out var getView))
		{
			logDoNotHaveUI(guiType);
			view = null;
			return false;
		}

		view = getView as T;
		return view != null;
	}

	public bool IsHistroyEmpty()
	{
		return CurrentView == null;
	}

	//�ش� �α״� ���� ������ ���� �� �ִ� Log�̱� ������ ���� �޼ҵ�� �������.
	private void logDoNotHaveUI(Type type)
	{
		Ulog.LogError(UlogType.UI, $"The Navigation don't have a UI :{type.Name}");
	}

	private IEnumerator waitNavigationHide()
	{
		while (CurrentView?.State == VisableState.Disappearing)
			yield return null;
		navigationEnable = false;
	}
}
