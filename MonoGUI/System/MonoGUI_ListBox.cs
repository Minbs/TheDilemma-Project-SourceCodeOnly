using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.ViewModel;
using UnityEngine.Events;
using MonoGUI;
using System.IO.Pipes;


namespace MonoGUI
{
	public enum ListBoxAddDiraction
	{
		None = -1,
		Up = 0,
		Down = 1,
	}

	[Serializable]
	public class MonoGUI_ListBox<T> where T : MonoGUI_View
	{
		private struct AnchorsAndPivot
		{
			public AnchorsAndPivot(Vector2 min, Vector2 max, Vector2 pivot)
			{
				Min = min;
				Max = max;
				Pivot = pivot;
			}

			public Vector2 Min { get; }
			public Vector2 Max { get; }
			public Vector2 Pivot { get; }
		}

		/// <summary> 해당 배열은 enum ListBoxAddDiraction을 인덱스로 하여서 사용됩니다. </summary>
		private AnchorsAndPivot[] mSettings = new AnchorsAndPivot[]
		{
			new AnchorsAndPivot(
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.5f, 0.0f)),

			new AnchorsAndPivot(
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(0.5f, 1.0f))
		};

		private List<T> mItemList = new List<T>();

		private ScrollRectViewModel mScroll;

		private Vector2 mNextSpawnPos;

		private ListBoxAddDiraction mDiraction { set; get; }

		public int Count
		{
			get
			{
				return mItemList.Count;
			}
		}

		public void Initialize(ScrollRectViewModel scroll, ListBoxAddDiraction diraction)
		{
			if (diraction == ListBoxAddDiraction.None)
			{
				Ulog.LogError($"{nameof(diraction)} is None Plase Setting ListBoxAddDiraction");
				return;
			}
			mDiraction = diraction;
			mScroll = scroll;
			setContentAnchor();
		}

		public void Add(T item)
		{
			InitiailzeItem(item);
			item.ViewRectTransfrom.anchoredPosition = mNextSpawnPos;
			mNextSpawnPos = getNextSpawn(item);
			adjustmentContent();
			mItemList.Add(item);
		}

		public void Insert(int index, T item)
		{
			if (!canAccess(index))
			{
				return;
			}

			InitiailzeItem(item);

			mItemList.Insert(index, item);
			Refresh();
		}

		public bool Remove(T content)
		{
			var isSuccess = mItemList.Remove(content);
			if (isSuccess)
			{
				destroyContent(content);
				Refresh();
			}
			return isSuccess;
		}

		public void RemoveAt(int index)
		{
			if (!canAccess(index))
			{
				return;
			}

			var removeContent = mItemList[index];
			mItemList.RemoveAt(index);
			destroyContent(removeContent);
			Refresh();
		}

		public void Clear()
		{
			for (int i = mItemList.Count - 1; i >= 0; i--)
			{
				destroyContent(mItemList[i]);
			}

			mItemList.Clear();
			adjustmentContent();
			mNextSpawnPos = Vector2.zero;
		}

		public void Refresh()
		{
			mNextSpawnPos = Vector2.zero;
			foreach (T item in mItemList)
			{
				item.ViewRectTransfrom.anchoredPosition = mNextSpawnPos;
				mNextSpawnPos = getNextSpawn(item);
			}
			adjustmentContent();
		}

		public void BindScrollAction(UnityAction<Vector2> onScrollAction)
		{
			mScroll.BindOnValueChanged(onScrollAction);
		}

		private void adjustmentContent()
		{
			mScroll.GetContent().sizeDelta = GetContentSize();
		}

		private Vector2 getNextSpawn(T item)
		{
			Vector2 nextSpwan = mNextSpawnPos;
			nextSpwan.y -= item.ViewRectTransfrom.rect.height;
			return nextSpwan;
		}

		private void InitiailzeItem(T item)
		{
			item.ViewRectTransfrom.SetParent(mScroll.GetContent().transform);
			setItemAnchorAndPivot(item);
			item.ViewRectTransfrom.localScale = Vector3.one;
			item.ViewRectTransfrom.sizeDelta = GetItemSize(item);
			item.Show();
		}

		private bool canAccess(int index)
		{
			if (index < 0 || index >= mItemList.Count)
			{
				Ulog.LogError($"ListBox RemoveAt Failed!, List Count: {mItemList.Count} index: {index}");
				return false;
			}
			return true;
		}

		private void destroyContent(T content)
		{
			UnityEngine.Object.Destroy(content.gameObject);
		}

		private Vector2 GetContentSize()
		{
			if (mDiraction == ListBoxAddDiraction.Up || mDiraction == ListBoxAddDiraction.Down)
			{
				return new Vector2(0.0f, Mathf.Abs(mNextSpawnPos.y));
			}
			return Vector2.zero;
		}

		private Vector2 GetItemSize(T item)
		{
			if (mDiraction == ListBoxAddDiraction.Up || mDiraction == ListBoxAddDiraction.Down)
			{
				return new Vector2(0.0f, item.ViewRectTransfrom.rect.height);
			}
			return Vector2.zero;
		}

		private void setContentAnchor()
		{
			int settingIndex = (int)mDiraction;

			mScroll.GetContent().anchorMin = mSettings[settingIndex].Min;
			mScroll.GetContent().anchorMax = mSettings[settingIndex].Max;
			mScroll.GetContent().pivot = mSettings[settingIndex].Pivot;
		}

		private void setItemAnchorAndPivot(T item)
		{
			int settingIndex = (int)ListBoxAddDiraction.Down;
			item.ViewRectTransfrom.anchorMin = mSettings[settingIndex].Min;
			item.ViewRectTransfrom.anchorMax = mSettings[settingIndex].Max;
			item.ViewRectTransfrom.pivot = mSettings[settingIndex].Pivot;
		}
	}
}
