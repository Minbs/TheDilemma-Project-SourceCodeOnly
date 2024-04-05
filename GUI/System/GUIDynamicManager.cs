using UnityEngine;
using Utils;

public class GUIDynamicManager : MonoBehaviour
{
	[field: SerializeField]
	private DynamicView mDrawView { set; get; }

	public RectTransform CanvasRect { private set; get; }

	public void Initialize()
	{
		CanvasRect = GetComponent<RectTransform>();
		mDrawView.Initialize(CanvasRect);
	}

	public T CreateDynamicPopupView<T>(GameObject guiObject) where T : GUIPopupView
	{
		GameObject dynamicView = Instantiate(guiObject, mDrawView.transform);

		if (!dynamicView.TryGetComponent<T>(out var popupView))
		{
			Ulog.LogError(UlogType.UI, $"The Instance GameObject is does not have a {typeof(T).Name}");
			Destroy(dynamicView);
			return null;
		}

		popupView.PopupViewSetup();

		return popupView;
	}

	public T CreateDynamicPopupView<T>(GameObject guiObject, Vector2 viewSpacePos) where T : GUIPopupView
	{
		GameObject dynamicView = Instantiate(guiObject, mDrawView.transform);

		if (!dynamicView.TryGetComponent<T>(out var popupView))
		{
			Ulog.LogError(UlogType.UI, $"The Instance GameObject is does not have a {typeof(T).Name}");
			Destroy(dynamicView);
			return null;
		}

		popupView.PopupViewSetup();
		popupView.ViewRectTransfrom.anchoredPosition = viewSpacePos;
		mDrawView.InBoundarySetting(popupView.ViewRectTransfrom);

		return popupView;
	}
}
