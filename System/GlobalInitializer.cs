using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet;
using KaNet.Utils;

/// <summary>게임 초기화를 위한 정적 클래스입니다. 정적 클래스를 초기화합니다.</summary>
public static class GlobalInitializer
{
	/// <summary>게임 프로세스 시작시 호출됩니다.</summary>
	/// <param name="gameProcessHandler">함수를 호출한 Mono 게임 프로세스 컨트롤러 클래스입니다.</param>
	public static void InitializeByProcessHandler(ProcessHandler gameProcessHandler)
	{
		// Initialize Unity Global
		try
		{
			Global.InitializeByProcessHandler(gameProcessHandler);
			gameProcessHandler.AddProcessInitialMessage($"{typeof(Global).Name} initialized!");
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(Global).Name} initialize error!", true);
			gameProcessHandler.AddProcessInitialMessage(e.Message, true);
		}

		// Initialize Service Locator
		try
		{
			GlobalServiceLocator.InitializeByProcessHandler(gameProcessHandler);
			gameProcessHandler.AddProcessInitialMessage($"{typeof(GlobalServiceLocator).Name} initialized!");
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(GlobalServiceLocator).Name} initialize error!", true);
			gameProcessHandler.AddProcessInitialMessage(e.Message, true);
		}

		// Initialize KaNet System
		try
		{
			KaKetGlobalSystem.InitializeByProcessHandler();
			gameProcessHandler.AddProcessInitialMessage($"{typeof(KaKetGlobalSystem).Name} initialized!");
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(KaKetGlobalSystem).Name} initialize error!", true);
			gameProcessHandler.AddProcessInitialMessage(e.Message, true);
		}

		// Initialize Network Object Field Prebinder
		try
		{
			NetworkObjectPrebinder.InitializeByProcessHandler(typeof(GlobalInitializer));
			gameProcessHandler.AddProcessInitialMessage($"{typeof(NetworkObjectPrebinder).Name} initialized!");
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(NetworkObjectPrebinder).Name} initialize error!", true);
			gameProcessHandler.AddProcessInitialMessage(e.Message, true);
		}

		// Allocate NetPackets
		try
		{
			PacketPool.TryAllocate();
			gameProcessHandler.AddProcessInitialMessage($"{typeof(PacketPool).Name} allocated!");
		}
		catch (Exception e)
		{
			gameProcessHandler.AddProcessInitialMessage($"{typeof(PacketPool).Name} initialize error!", true);
			gameProcessHandler.AddProcessInitialMessage(e.Message, true);
		}
	}
}
