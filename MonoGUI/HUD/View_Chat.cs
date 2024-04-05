using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Utils.ViewModel;

using Sirenix.OdinInspector;
using System.Collections;
using System;
using MonoGUI;

public class View_Chat : MonoGUI_View, IPointerEnterHandler, IPointerExitHandler
{
	private struct ChetForcus
	{
		public GameObject BackGround;
		public GameObject Input;
		public ScrollbarViewModel Scrollbar;
	}

	private ChetForcus mChetForcus;

	public bool IsFocus { get; private set; } = false;
	public bool IsInteraction { get; private set; } = false;


	[SerializeField]
	private ScrollRectViewModel Scroll_Chat = new(nameof(Scroll_Chat));

	[SerializeField]
	private TextMeshProTextViewModel Text_Log = new(nameof(Text_Log));

	[SerializeField]
	private TextMeshProInputFieldViewModel TextBox_ChatInput​ = new(nameof(TextBox_ChatInput​));

	[SerializeField]
	private ImageViewModel Img_BackGround = new(nameof(Img_BackGround));

	[SerializeField]
	private ScrollbarViewModel Scrollbar = new(nameof(Scrollbar));

	public event Action<string> OnSendEvent;

	public override void OnInitialized()
	{
		Ulog.Log($"[HUD] Draw Chat");

		Text_Log.Initialize(this);
		Scroll_Chat.Initialize(this);
		TextBox_ChatInput​.Initialize(this);
		Img_BackGround.Initialize(this);
		Scrollbar.Initialize(this);

		#region HidePack

		mChetForcus = new ChetForcus();
		mChetForcus.Input = TextBox_ChatInput​.GetViewGameObject();
		mChetForcus.BackGround = Img_BackGround.GetViewGameObject();
		mChetForcus.Scrollbar = Scrollbar;

		#endregion

		//OnSendEvent = Receive;

		OffForces();

		Show();
	}

	private GameplayManager mGameplayManager;
	public void InitializeByManager(GameplayManager gameplayManager)
	{
		mGameplayManager = gameplayManager;

		// Setup events
		OnSendEvent += (message) => mGameplayManager.ChatHandler.SendChatMessageToServer(message);
		mGameplayManager.ChatHandler.OnReceivedMessage += Receive;
		OnStartHidding += () => mGameplayManager.ChatHandler.OnReceivedMessage -= Receive;
	}

	public void Send(string msg)
	{
		if (msg.Equals(""))
			return;

		TextBox_ChatInput​.SetText(null);

		OnSendEvent?.Invoke(msg);
	}

	public void Receive(string msg)
	{
		var chatLog = msg + '\n';

		Text_Log.Text += chatLog;
	}

	private string setColor(Color color, string str)
	{
		return $"<#{ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>";
	}

	private string setBold(string str)
	{
		return $"<b>{str}</b>";
	}

	private string setUnderscore(string str)
	{
		return $"<u>{str}</u>";
	}

	private string setHighlight(string str)
	{
		return $"<mark>{str}</mark>";
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		IsFocus = true;
		OnForces();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		IsFocus = false;
		if (!IsInteraction)
			OffForces();
	}

	private void OnForces()
	{
		mChetForcus.BackGround.SetActive(true);
		mChetForcus.Input.SetActive(true);
		mChetForcus.Scrollbar.SetActive(true);
	}

	private void OffForces()
	{
		mChetForcus.BackGround.SetActive(false);
		mChetForcus.Input.SetActive(false);
		mChetForcus.Scrollbar.SetActive(false);
	}

	private void StartEdit()
	{
		IsInteraction = true;
		OnForces();
		TextBox_ChatInput​.Select();
	}

	private void EndEdit()
	{
		IsInteraction = false;
		Send(TextBox_ChatInput​.InputText);
		EventSystem.current.SetSelectedGameObject(null);
		if (!IsFocus)
			OffForces();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (!IsInteraction)
				StartEdit();
			else
				EndEdit();
		}
	}

	public string ExprotLog()
	{
		return Text_Log.Text;
	}


}
