using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaNet.Session;
using UnityEngine;
using Utils;
using Utils.ViewModel;

namespace MonoGUI
{
	public class Navigation_HUD : MonoGUI_Navigation
	{
		[SerializeField] private GenericViewModel<Navigation_PlayerInfoGroup> Navigation_PlayerInfoGroup = new(nameof(Navigation_PlayerInfoGroup));

		[SerializeField] private TransformViewModel Pivot_SystemMessagePanel = new(nameof(Pivot_SystemMessagePanel));

		[SerializeField] private TransformViewModel Pivot_ChatMessagePanel = new (nameof(Pivot_ChatMessagePanel));

		[SerializeField] private TransformViewModel Pivot_PlayerInfoPanel = new(nameof(Pivot_PlayerInfoPanel));

		[SerializeField] private TransformViewModel Pivot_QuickSlotlPanel = new(nameof(Pivot_QuickSlotlPanel));

		[SerializeField] private TransformViewModel Pivot_PlayTimePanel = new(nameof(Pivot_PlayTimePanel));

		[SerializeField] private TransformViewModel Pivot_ProgresslinePanel = new(nameof(Pivot_ProgresslinePanel));

		[SerializeField] private TransformViewModel Pivot_MiniMapPanel = new(nameof(Pivot_MiniMapPanel));

		public void Awake()
		{
			Navigation_PlayerInfoGroup.Initialize(this);

			Pivot_SystemMessagePanel.Initialize(this);
			Pivot_ChatMessagePanel.Initialize(this);
			Pivot_PlayerInfoPanel.Initialize(this);
			Pivot_QuickSlotlPanel.Initialize(this);
			Pivot_PlayTimePanel.Initialize(this);
			Pivot_ProgresslinePanel.Initialize(this);
			Pivot_MiniMapPanel.Initialize(this);
		}

		public void OpenHUD(GameplayManager gameplayManager)
		{
			// Chatting
			var chat = Push<View_Chat>();
			chat.StretchToParent(Pivot_ChatMessagePanel.Transform);
			chat.InitializeByManager(gameplayManager);

			// Team Session Shower
			Navigation_PlayerInfoGroup.Model.Initialize(gameplayManager);

			// Quick Slot
			var quickSlot = Push<View_QuickSlot>();
			quickSlot.StretchToParent(Pivot_QuickSlotlPanel.Transform);
			quickSlot.Initilaized(gameplayManager);

			Push<View_PlayTime>().StretchToParent(Pivot_PlayTimePanel.Transform);
			Push<View_ProgressLine>().StretchToParent(Pivot_ProgresslinePanel.Transform);
			Push<View_MinMap>().StretchToParent(Pivot_MiniMapPanel.Transform);

			// SystemMessage
			var systemMessage = Push<View_SystemMessage>();
			systemMessage.StretchToParent(Pivot_SystemMessagePanel.Transform);
			systemMessage.Close();

			// Player Info
			var playerInfoView = Push<View_PlayerInfo>();
			playerInfoView.StretchToParent(Pivot_PlayerInfoPanel.Transform);
			gameplayManager.IngameSessionHandler.TryGetMySessionInfo(out var clientInfo);
			playerInfoView.Initialize(gameplayManager, clientInfo);
		}

		public void DrawSystemMessage(MessageType type, string msg, float durlation)
		{
			if (TryFind<View_SystemMessage>(out var view))
			{
				view.Message(type, msg, durlation);
			}
		}
	}
}
