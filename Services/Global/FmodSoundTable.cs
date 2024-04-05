using System.Collections.Generic;

public static class FmodSoundTable
{
	private static Dictionary<SoundType, FmodEvent> mSoundTable = new()
	{

		{SoundType.SFX_FootStep, new FmodEvent("event:/Character/Player Footsteps")},


		{SoundType.BGM_0, new FmodEvent("event:/BGM/bgm_0") },
		{SoundType.BGM_1, new FmodEvent("event:/BGM/bgm_1") },
		{SoundType.BGM_2, new FmodEvent("event:/BGM/bgm_2") },
		{SoundType.BGM_3, new FmodEvent("event:/BGM/bgm_3") },
		{SoundType.BGM_Satan0, new FmodEvent("event:/BGM/bgm_satan0") },
		{SoundType.BGM_Satan1, new FmodEvent("event:/BGM/bgm_satan1") },
		{SoundType.BGM_TheGenesis, new FmodEvent("event:/BGM/bgm_TheGenesis") },

		{SoundType.SFX_Explosion, new FmodEvent("event:/Weapons/Explosion") },
		{SoundType.SFX_Pistol, new FmodEvent("event:/Weapons/Pistol") },
		{SoundType.SFX_GUN_AMMO_Pickup04, new FmodEvent("event:/Weapons/Gun_Ammo_Pickup04") },
		{SoundType.SFX_GUN_AMMO_Pickup05, new FmodEvent("event:/Weapons/Gun_Ammo_Pickup05") },
		{SoundType.SFX_GUN_AR_PowerShot1, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot1") },
		{SoundType.SFX_GUN_AR_PowerShot2, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot2") },
		{SoundType.SFX_GUN_AR_PowerShot3, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot3") },
		{SoundType.SFX_GUN_AR_PowerShot4, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot4") },
		{SoundType.SFX_GUN_AR_PowerShot5, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot5") },
		{SoundType.SFX_GUN_AR_PowerShot6, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot6") },
		{SoundType.SFX_GUN_AR_PowerShot7, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot7") },
		{SoundType.SFX_GUN_AR_PowerShot8, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot8") },
		{SoundType.SFX_GUN_AR_PowerShot9, new FmodEvent("event:/Weapons/Gun_AssaultRifle_PowerShot9") },
		{SoundType.SFX_DrawSelect_Scifi1, new FmodEvent("event:/Weapons/Gun_DrawSelect_Scifi01") },
		{SoundType.SFX_DrawSelect_Scifi2, new FmodEvent("event:/Weapons/Gun_DrawSelect_Scifi02") },
		{SoundType.SFX_DrawSelect_Scifi3, new FmodEvent("event:/Weapons/Gun_DrawSelect_Scifi03") },
		{SoundType.SFX_DrawSelect_Scifi4, new FmodEvent("event:/Weapons/Gun_DrawSelect_Scifi04") },
		{SoundType.SFX_DrawSelect_Scifi5, new FmodEvent("event:/Weapons/Gun_DrawSelect_Scifi05") },

		{SoundType.Ambience_City, new FmodEvent("event:/Ambience/City") },
		{SoundType.Ambience_ScifiAlien02, new FmodEvent("event:/Ambience/Ambience-Scifi-Alien-02") },
		{SoundType.Ambience_ScifiCircuits, new FmodEvent("event:/Ambience/Ambience-Scifi-Circuits") },
		{SoundType.Ambience_ScifiMachine03, new FmodEvent("event:/Ambience/Ambience-Scifi-Machine-03") },
		{SoundType.Ambience_ScifiWind03, new FmodEvent("event:/Ambience/Ambience-Scifi-Wind-03") },
		{SoundType.Ambience_Werewolf, new FmodEvent("event:/Ambience/Werewolf Background Atmosphere_Loop") },

		{SoundType.GUI_SoundPack8_Back, new FmodEvent("event:/GUI/UI_SoundPack8_Back_v3") },
		{SoundType.GUI_SoundPack8_Error, new FmodEvent("event:/GUI/UI_SoundPack8_Error_v1") },
		{SoundType.GUI_SoundPack8_Scroll, new FmodEvent("event:/GUI/UI_SoundPack8_Scroll_v2") },
		{SoundType.GUI_SoundPack11_Back, new FmodEvent("event:/GUI/UI_SoundPack11_Back_v1")},
		{SoundType.GUI_SoundPack11_Error, new FmodEvent("event:/GUI/UI_SoundPack11_Error_v3")},
		{SoundType.GUI_SoundPack13_Error, new FmodEvent("event:/GUI/UI_SoundPack13_Error_version01") },
		{SoundType.GUI_SoundPack13_Scroll, new FmodEvent("event:/GUI/UI_SoundPack13_Scroll_version05") },
		{SoundType.GUI_SoundPack13_Select, new FmodEvent("event:/GUI/UI_SoundPack13_Select_version10") },


	};

	public static FmodEvent GetFmodEvent(SoundType type)
	{
		return mSoundTable[type];
	}
}
