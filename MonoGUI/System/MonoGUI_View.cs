using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MonoGUI
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class MonoGUI_View : MonoBehaviour
	{
		// Events
		public event Action OnStartShowing;
		public event Action OnShowed;

		public event Action OnStartHidding;
		public event Action OnHided;

		// Animations
		[field: TabGroup("Show Animation"), SerializeField]
		public List<GUISeqenceElement> ShowTweenList { get; private set; }
		[field: TabGroup("Hide Animation"), SerializeField]
		public List<GUISeqenceElement> HideTweenList { get; private set; }

		private Sequence mShowAnimation;
		private Sequence mHideAnimation;

		public bool IsShowAnimatinoAble { private set; get; } = false;
		public bool IsHideAnimatinoAble { private set; get; } = false;

		// RectTransform
		private RectTransform mViewRectTransfrom;
		public RectTransform ViewRectTransfrom
		{
			get
			{
				if (mViewRectTransfrom == null)
				{
					mViewRectTransfrom = GetComponent<RectTransform>();
				}

				return mViewRectTransfrom;
			}
			private set
			{
				mViewRectTransfrom = value;
			}
		}

		protected ResourcesService mResourcesService 
			=> GlobalServiceLocator.ResourcesService.GetServiceOrNull();

		public void StretchToParent(Transform transform)
		{
			ViewRectTransfrom.StretchToParent(transform);
		}

		public void StretchToParent()
		{
			ViewRectTransfrom.StretchToParent();
		}

		public void SetParent(Transform transfrom)
		{
			ViewRectTransfrom.parent = transfrom;
		}

		// State
		public GuiState CurrentState { get; protected set; } = GuiState.None;

		private void Awake()
		{
			OnInitialized();

			//보여지는 애니메이션이 있는 경우를 체크
			if (ShowTweenList.Count > 0)
				IsShowAnimatinoAble = true;

			//사라지는 애니메이션이 있는 경우를 체크
			if (HideTweenList.Count > 0)
				IsHideAnimatinoAble = true;
		}

		public abstract void OnInitialized();

		public bool IsShow =>
			CurrentState == GuiState.Showing ||
			CurrentState == GuiState.Showed;
		public bool IsHide =>
			CurrentState == GuiState.Hidding ||
			CurrentState == GuiState.Hided;

		public virtual void OnShow() { }
		public virtual void OnHide() { }

		public virtual void Show(Action callback = null)
		{
			OnShow();

			//현재 Show가 호출중이면 해당 애니메이션을 죽이고 다시 시작
			if (IsShow)
			{
				killTween(ref mShowAnimation);
			}

			if (!IsShowAnimatinoAble)
			{
				gameObject.SetActive(true);
				OnStartShowing?.Invoke();
				OnShowed?.Invoke();
				callback?.Invoke();
				CurrentState = GuiState.Showed;
			}
			else
			{
				if (CurrentState == GuiState.Hidding)
					HideSkip();

				gameObject.SetActive(true);

				createShowAnimation(callback, out mShowAnimation);
				CurrentState = GuiState.Showing;
				mShowAnimation.Play();
			}

		}

		//현재 Hide중이면 해당 애니메이션을 죽이고 다시 시작
		public virtual void Hide(Action callback = null)
		{
			OnHide();

			if (IsHide)
			{
				killTween(ref mHideAnimation);
			}

			if (!IsHideAnimatinoAble)
			{
				OnStartHidding?.Invoke();
				OnHided?.Invoke();

				gameObject.SetActive(false);

				CurrentState = GuiState.Hided;
				callback?.Invoke();
			}
			else
			{
				if (CurrentState == GuiState.Showing)
					ShowSkip();

				//Play Show Animation
				if (mHideAnimation != null)
				{
					mHideAnimation.Kill();
				}
				createHideAnimation(callback,out mHideAnimation);
				CurrentState = GuiState.Hidding;
				mHideAnimation.Play();
			}
		}

		public static bool TryGetGuiInstance(Type guiType, Transform transform, out GameObject guiInstance)
		{
			if (GlobalServiceLocator.ResourcesService.TryGetService(out var resourcesService))
			{
				var guiTable = resourcesService.GuiTable;

				Ulog.Log($"GUI table is null? : {guiTable == null}");
				Ulog.Log($"GUI table count : {guiTable.Count}");

				if (guiTable.TryGetValue(guiType, out var guiObject))
				{
					guiInstance = Instantiate(guiObject, transform);
					return true;
				}
			}

			Ulog.LogError(UlogType.UI, $"There is no such GUI in table : {guiType.Name}");
			guiInstance = null;
			return false;
		}

		private void createShowAnimation(Action callback, out Sequence sequence)
		{
			sequence = null;

			if (!IsShowAnimatinoAble)
				return;

			createSequence(ShowTweenList, out sequence);

			sequence.OnStart(() =>
			{
				Debug.Log("A: Ani Start");
				foreach (var element in ShowTweenList)
				{
					element.TweenAnimation.OnSequenceStart();
				}

				OnStartShowing?.Invoke();
			});

			sequence.OnComplete(() =>
			{
				Debug.Log("A: Ani Complete");
				CurrentState = GuiState.Showed;
				mShowAnimation = null;
				OnShowed?.Invoke();
				callback?.Invoke();
			});

		}

		private void createHideAnimation(Action callback, out Sequence sequence)
		{
			sequence = null;
			if (!IsHideAnimatinoAble)
				return;

			createSequence(HideTweenList, out mHideAnimation);

			mHideAnimation.OnStart(() =>
			{
				foreach (var element in HideTweenList)
				{
					element.TweenAnimation.OnSequenceStart();
				}

				OnStartHidding?.Invoke();
			});

			mHideAnimation.OnComplete(() =>
			{
				CurrentState = GuiState.Hided;
				gameObject.SetActive(false);
				mHideAnimation = null;
				OnHided?.Invoke();
				callback?.Invoke();
			});
		}

		private void createSequence(List<GUISeqenceElement> tweenList, out Sequence sequence)
		{
			foreach(GUISeqenceElement element in tweenList)
			{
				element.Initilze(this);
			}

			sequence = DOTween.Sequence();

			foreach (var element in tweenList)
			{
				if (element.SequenceType == GUISequenceType.Append)
					sequence.Append(element.TweenAnimation.GetTween());
				else if (element.SequenceType == GUISequenceType.Join)
					sequence.Join(element.TweenAnimation.GetTween());
			}
		}

		protected void ShowSkip()
		{
			if (CurrentState == GuiState.Showing)
				mShowAnimation?.Complete();
		}

		protected void HideSkip()
		{
			if (CurrentState == GuiState.Hidding)
				mHideAnimation?.Complete();
		}

		private void killTween(ref Sequence sequence)
		{
			if(sequence != null)
			{
				DOTween.Kill(sequence);
				sequence = null;
			}
		}
	}
}
