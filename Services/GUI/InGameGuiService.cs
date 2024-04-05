using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Session;
using MonoGUI;
using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine;
using Utils;
using Utils.Service;
using Utils.ViewModel;


public partial class InGameGuiService : MonoService
{
	[field : SerializeField]
	public GUINavigationController GuiNavController { get; private set; }

    [field : SerializeField]
	public GUIDynamicManager SystemDialogManager { get; private set; }

	[Title("Set GUI HUD")]
	[SerializeField]
	private TransformViewModel Transfrom_GUI_InGameHUD = new(nameof(Transfrom_GUI_InGameHUD));
	[SerializeField]
	private GameObject View_InGameHudPrefab;

	[Title("Set GUI Escape")]
	[SerializeField]
	private TransformViewModel Transfrom_GUI_Escape = new(nameof(Transfrom_GUI_Escape));
	[SerializeField]
	private GameObject View_EscapePrefab;
	private View_Escape mView_Escape;

	[Title("Set GUI System Dialog")]
	[SerializeField]
	private TransformViewModel Transfrom_GUI_SystemDialog = new(nameof(Transfrom_GUI_SystemDialog));
	[SerializeField] private GameObject View_SystemDialogPrefab;
	//private Dictionary<NetOperationType, View_SystemDialog> mView_SystemDialogTable = new();

	[Title("Set World GUI")]
	[SerializeField]
	private TransformViewModel Transfrom_GUI_WorldGUI = new(nameof(Transfrom_GUI_WorldGUI));
	[SerializeField]
	private GameObject View_WorldGUIPrefab;
	private View_WorldGUIView mView_WorldGUI;

	// DI Field
	private NetworkManageService mNetworkManageService;
	private GameHandler mGameHandler;
	private InputService mInputService;
	public GameStateType CurrentGameState
	{
		get
		{ 
			if (mGameHandler == null)
			{
				return GameStateType.Hideout;
			}

			return mGameHandler.CurrentGameState.Data.GetEnum();
		}
	}

	public override void OnRegistered()
	{
		base.OnRegistered();

		Transfrom_GUI_InGameHUD.Initialize(this);
		Transfrom_GUI_Escape.Initialize(this);
		Transfrom_GUI_SystemDialog.Initialize(this);
		Transfrom_GUI_WorldGUI.Initialize(this);

		// Initialize GUI Navigation
		GuiNavController.Init();

		// Initialize dynamic manager
		SystemDialogManager.Initialize();

		// Set GUI HUD
		//mView_InGameHUD = Instantiate(View_InGameHudPrefab, Transfrom_GUI_InGameHUD.Transform)
		//	.GetComponent<View_InGameHUD>();

		// Set GUI Escape
		mView_Escape = Instantiate(View_EscapePrefab, Transfrom_GUI_Escape.Transform)
			.GetComponent<View_Escape>();

		//Set GUI World_GUI
		mView_WorldGUI = Instantiate(View_WorldGUIPrefab, Transfrom_GUI_InGameHUD.Transform)
			.GetComponent<View_WorldGUIView>();
	}

	public void Initialize
	(
		NetworkManageService networkManageService,
		GameHandler gameHandler,
		InputService inputService
	)
	{
		// Setup DI Field
		mNetworkManageService = networkManageService;
		mGameHandler = gameHandler;
		mInputService = inputService;
	}


	private Queue<Action> mThreadUnwinder = new();

	public void FixedUpdate()
	{
		while (mThreadUnwinder.Count != 0)
		{
			var action = mThreadUnwinder.Dequeue();
			action();
		}
	}

	public View_WorldGUIView GetWorldGUIView()
	{
		return mView_WorldGUI;
	}
	
}
