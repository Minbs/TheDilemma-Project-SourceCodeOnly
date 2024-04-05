using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Service;

namespace Gameplay
{
	public class PlayerInputService : IServiceable
	{
		private GameplayManager mGameplayManager;
		private EntityService mEntityService;

		private InputService mInputService;

		// Mouse
		private InputAction Mouse_Left;
		private InputAction Mouse_Right;

		// Movement
		private InputAction Key_ArrowDown;
		private InputAction Key_ArrowUp;
		private InputAction Key_ArrowRight;
		private InputAction Key_ArrowLeft;

		// Swap
		private InputAction Key_D1;
		private InputAction Key_D2;
		private InputAction Key_D3;

		// Interact
		private InputAction Key_R;
		private InputAction Key_E;
		private InputAction Key_Q;

		public Vector2 Input_Move { get; private set; }

		public PlayerInputService(EntityService mEntityService)
		{
			this.mEntityService = mEntityService;	
		}

		public void InitializeByManager(GameplayManager gameplayManager)
		{
			mGameplayManager = gameplayManager;
		}

		public void OnRegistered()
		{
			mInputService = GlobalServiceLocator.InputService.GetServiceOrNull();

			mInputService.OnUpdateInput += onUpdateInput;

			#region Assignee Input Actions

			Key_ArrowRight = mInputService.GetInputAction(InputType.Key_ArrowRight);
			Key_ArrowLeft = mInputService.GetInputAction(InputType.Key_ArrowLeft);
			Key_ArrowUp = mInputService.GetInputAction(InputType.Key_ArrowUp);
			Key_ArrowDown = mInputService.GetInputAction(InputType.Key_ArrowDown);

			Key_D1 = mInputService.GetInputAction(InputType.Key_SwapEquipment_Primary);
			Key_D2 = mInputService.GetInputAction(InputType.Key_SwapEquipment_Secondary);
			Key_D3 = mInputService.GetInputAction(InputType.Key_SwapEquipment_Auxiliary);

			Key_R = mInputService.GetInputAction(InputType.Key_Reload);
			Key_E = mInputService.GetInputAction(InputType.Key_Interact);
			Key_Q = mInputService.GetInputAction(InputType.Key_HealSelf);

			Mouse_Left = mInputService.GetInputAction(InputType.Mouse_Left);
			Mouse_Right = mInputService.GetInputAction(InputType.Mouse_Right);

			#endregion

			#region Bind input actions

			Key_ArrowRight.OnIsPressing += onKeyRight;
			Key_ArrowLeft.OnIsPressing += onKeyLeft;
			Key_ArrowUp.OnIsPressing += onKeyUp;
			Key_ArrowDown.OnIsPressing += onKeyDown;

			Key_D1.OnPressed += onSwapToPrimary;
			Key_D2.OnPressed += onSwapToSecondary;
			Key_D3.OnPressed += onSwapToAuxiliary;

			Key_R.OnPressed += mGameplayManager.ProcessReload;
			Key_E.OnPressed += mGameplayManager.ProcessInteract;
			Key_Q.OnPressed += mGameplayManager.ProcessHealSelf;

			Mouse_Left.OnPressed += onMouseLeftPressed;
			Mouse_Left.OnPressing += onMouseLeftPressing;
			Mouse_Right.OnPressed += onMouseRightPressed;
			Mouse_Right.OnPressing += onMouseRightPressing;

			#endregion
		}

		public void OnUnregistered()
		{
			#region Unbind input actions

			mInputService.OnUpdateInput -= onUpdateInput;

			Key_ArrowRight.OnIsPressing -= onKeyRight;
			Key_ArrowLeft.OnIsPressing -= onKeyLeft;
			Key_ArrowUp.OnIsPressing -= onKeyUp;
			Key_ArrowDown.OnIsPressing -= onKeyDown;

			Key_D1.OnPressed -= onSwapToPrimary;
			Key_D2.OnPressed -= onSwapToSecondary;
			Key_D3.OnPressed -= onSwapToAuxiliary;

			Key_R.OnPressed -= mGameplayManager.ProcessReload;
			Key_E.OnPressed -= mGameplayManager.ProcessInteract;
			Key_Q.OnPressed -= mGameplayManager.ProcessHealSelf;

			Mouse_Left.OnPressed -= onMouseLeftPressed;
			Mouse_Left.OnPressing -= onMouseLeftPressing;
			Mouse_Right.OnPressed -= onMouseRightPressed;
			Mouse_Right.OnPressing -= onMouseRightPressing;

			#endregion
		}

		private void onUpdateInput()
		{
			mGameplayManager.ProcessMovementInput(Input_Move);
			Input_Move = Vector2.zero;
		}

		private void onSwapToPrimary() => mGameplayManager.ProcessEquipmentSwapInput(1);
		private void onSwapToSecondary() => mGameplayManager.ProcessEquipmentSwapInput(2);
		private void onSwapToAuxiliary() => mGameplayManager.ProcessEquipmentSwapInput(3);

		private void onMouseLeftPressed() => mGameplayManager.ProcessMousePressed(true);
		private void onMouseLeftPressing() => mGameplayManager.ProcessMousePressing(true);
		private void onMouseRightPressed() => mGameplayManager.ProcessMousePressed(false);
		private void onMouseRightPressing() => mGameplayManager.ProcessMousePressing(false);

		#region Movement

		public void onKeyRight(bool isPressed)
		{
			Input_Move += isPressed ? Vector2.right : Vector2.left;
		}

		public void onKeyLeft(bool isPressed)
		{
			Input_Move += isPressed ? Vector2.left : Vector2.right;
		}

		public void onKeyUp(bool isPressed)
		{
			Input_Move += isPressed ? Vector2.up : Vector2.down;
		}

		public void onKeyDown(bool isPressed)
		{
			Input_Move += isPressed ? Vector2.down : Vector2.up;
		}

		#endregion
	}
}
