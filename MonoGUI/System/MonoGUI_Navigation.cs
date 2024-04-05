using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Utils;

namespace MonoGUI
{
	/// <summary>고유한 GUI View를 관리하는 네비게이션입니다.</summary>
	public class MonoGUI_Navigation : MonoBehaviour
	{
		private List<(Type ViewType, GameObject Instance)> mHideViewStack = new();
		private List<(MonoGUI_View View, GameObject Instance)> mViewList = new();

		public int Count => mViewList.Count;
		public bool HasView => Count != 0;

		public MonoGUI_View CurrentView
		{
			get
			{
				if (mViewList == null || mViewList.IsEmpty())
				{
					return null;
				}

				return mViewList[mViewList.Count - 1].View;
			}
		}

		private bool tryGetGuiInstance<T>(out T gui, out GameObject go) where T : MonoGUI_View
		{
			int findIndex = mHideViewStack.FindIndex((e) => e.ViewType == typeof(T));

			if (findIndex >= 0)
			{
				go = mHideViewStack[findIndex].Instance;
				gui = go.GetComponent<T>();

				mHideViewStack.RemoveAt(findIndex);
				go.transform.SetAsLastSibling();
				mViewList.Add((gui, go));
				return true;
			}
			
			if (MonoGUI_View.TryGetGuiInstance(typeof(T), transform, out go))
			{
				gui = go.GetComponent<T>();
				mViewList.Add((gui, go));
				return true;
			}

			gui = null;
			return false;
		}

		private void releaseView(GameObject instance)
		{
			int findIndex = mViewList.FindIndex((e) => e.Instance == instance);

			if (findIndex >= 0)
			{
				var returnView = mViewList[findIndex];
				mHideViewStack.Add((returnView.View.GetType(), returnView.Instance));
				mViewList.RemoveAt(findIndex);
				return;
			}

			Ulog.LogError(this, $"정상적이지 않은 UI 객체 반환입니다. 객체 이름 : {instance.name}");
		}

		/// <summary>네비게이션 내부의 모든 View를 제거합니다.</summary>
		public void Clear()
		{
			var list = mViewList.ToList();

			foreach (var view in list)
			{
				view.View.Hide();
				releaseView(view.Instance);
			}
		}

		/// <summary>해당 View로 전환합니다. 이전 View는 모두 제거됩니다.</summary>
		/// <typeparam name="T">전환할 View의 타입입니다.</typeparam>
		/// <returns>전환된 View입니다.</returns>
		public T Switch<T>(Action callback = null) where T : MonoGUI_View
		{
			Clear();
			return Push<T>(callback);
		}

		/// <summary>해당 View를 추가합니다.</summary>
		/// <typeparam name="T">추가할 View의 타입입니다.</typeparam>
		/// <returns>추가된 View입니다.</returns>
		public T Push<T>(Action callback = null) where T : MonoGUI_View
		{
			if (tryGetGuiInstance<T>(out var gui, out var go))
			{
				gui.Show(callback);
				return gui;
			}

			return null;
		}

		/// <summary>최상단 View를 제거합니다.</summary>
		public void Pop(Action callback = null)
		{
			if (mViewList.IsEmpty())
			{
				return;
			}

			CurrentView.Hide(callback);
			releaseView(mViewList[mViewList.Count - 1].Instance);
		}

		/// <summary>해당되는 GUI 객체를 제거합니다.</summary>
		/// <param name="guiObject">GUI 객체</param>
		public void PopByObject(GameObject guiObject)
		{
			int index = mViewList.FindIndex((e) => e.Instance == guiObject);

			if (index >= 0)
			{
				mViewList[index].View.Hide();
				releaseView(mViewList[index].Instance);
			}
		}

		/// <summary>조건에 일치하는 GUI 객체를 제거합니다.</summary>
		/// <param name="predicate"></param>
		public void PopMatch(Predicate<(MonoGUI_View View, GameObject Instance)> predicate, Action callback = null)
		{
			int index = mViewList.FindIndex(predicate);

			if (index >= 0)
			{
				mViewList[index].View.Hide(callback);
				releaseView(mViewList[index].Instance);
			}
		}

		/// <summary>매칭되는 타입의 GUI 객체를 제거합니다.</summary>
		/// <typeparam name="T">GUI 객체 타입</typeparam>
		public void Pop<T>(Action callback = null)
		{
			PopMatch((e) => e.View.GetType() == typeof(T), callback);
		}

		/// <summary>매칭되는 타입의 GUI 객체가 있다면 반환합니다.</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryFind<T>(out T view) where T : MonoGUI_View
		{
			int index = mViewList.FindIndex((i) => i.View.GetType() == typeof(T));

			if (index >= 0)
			{
				view = mViewList[index].View as T;
				return true;
			}
			else
			{
				view = null;
				return false;
			}
		}

		/// <summary>매칭되는 GUI 객체가 있다면 반환합니다.</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryFind(Predicate<(MonoGUI_View View, GameObject Instance)> predicate, out MonoGUI_View view)
		{
			int index = mViewList.FindIndex(predicate);

			if (index >= 0)
			{
				view = mViewList[index].View;
				return true;
			}
			else
			{
				view = null;
				return false;
			}
		}
	}
}
