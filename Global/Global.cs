using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class Global
{
	public const int DRAW_PPU = 32;
	public const int DEPTH_ORDER_OFFSET = 64;

	public static void InitializeByProcessHandler(ProcessHandler gameProcessHandler)
	{

	}

	public static int RoundByDepthOrderOffset(Transform transform)
	{
		return -(int)(transform.position.y * DEPTH_ORDER_OFFSET);
	}

	public static int RoundByDepthOrderOffset(float value)
	{
		return -(int)(value * DEPTH_ORDER_OFFSET);
	}

	public static int RoundByHitscanDepthOrder(Transform transform)
	{
		return RoundByDepthOrderOffset(transform);
	}

	public static int RoundByHitscanDepthOrder(float value)
	{
		return RoundByDepthOrderOffset(value - 0.5f);
	}

	public static float RoundByPPU(float value)
	{
		return (int)(value * DRAW_PPU) / (float)DRAW_PPU;
	}

	public static Vector2 RoundByPPU(Vector2 value)
	{
		float x = RoundByPPU(value.x);
		float y = RoundByPPU(value.y);
		return new Vector2(x, y);
	}
}

public static class GlobalLayer
{
	#region For Raycast

	public static readonly int LAYER_RAYCAST_HITBOX			= LayerMask.GetMask("Entity_Hitbox", "Wall_Hitbox");
	public static readonly int LAYER_RAYCAST_WALL_AREA_HIGH = LayerMask.GetMask("Wall_Area_High");
	public static readonly int LAYER_RAYCAST_ENTITY_AREA	= LayerMask.GetMask("Entity_Area", "Entity_Area_Ignore_Invisible");
	public static readonly int LAYER_RAYCAST_ITEM_AREA		= LayerMask.GetMask("Item_Area");
	public static readonly int LAYER_RAYCAST_HITSCAN		= LayerMask.GetMask("Hitscan");
	public static readonly int LAYER_RAYCAST_LOCATOR_AREA	= LayerMask.GetMask("Locator_Area");

	#endregion

	#region Layer Mask

	public static readonly int LAYER_FLOOR_AREA						= LayerMask.NameToLayer("Floor_Area");
	public static readonly int LAYER_ENTITY_AREA					= LayerMask.NameToLayer("Entity_Area");
	public static readonly int LAYER_ENTITY_AREA_IGNORE_INVISIBLE	= LayerMask.NameToLayer("Entity_Area_Ignore_Invisible");
	public static readonly int LAYER_ENTITY_HITBOX					= LayerMask.NameToLayer("Entity_Hitbox");
	public static readonly int LAYER_PROJECTILE_AREA				= LayerMask.NameToLayer("Projectile_Area");
	public static readonly int LAYER_HITSCAN						= LayerMask.NameToLayer("Hitscan");
	public static readonly int LAYER_WALL_AREA_HIGH					= LayerMask.NameToLayer("Wall_Area_High");
	public static readonly int LAYER_WALL_AREA_LOW					= LayerMask.NameToLayer("Wall_Area_Low");
	public static readonly int LAYER_WALL_HITBOX					= LayerMask.NameToLayer("Wall_Hitbox");
	public static readonly int LAYER_WALL_INVISIBLE					= LayerMask.NameToLayer("Wall_Invisible");
	public static readonly int LAYER_ITEM_AREA						= LayerMask.NameToLayer("Item_Area");
	public static readonly int LAYER_LOCATOR_AREA					= LayerMask.NameToLayer("Locator_Area");

	public static int GetMask(Collider2D collider)
	{
		return GetMask(collider.gameObject.layer);
	}

	public static int GetMask(int layerIndex)
	{
		return 1 << layerIndex;
	}

	public static bool IsMatch(int lhsLayer, int rhsLayer)
	{
		return (lhsLayer & rhsLayer) != 0;
	}

	#endregion
}