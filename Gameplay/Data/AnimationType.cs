using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
	public enum AnimationType : byte
	{
		None,

		Idle_Front,
		Idle_Back,

		IdleWithoutArm_Front,
		IdleWithoutArm_Back,

		Run_Front,
		Run_Back,

		RunWithoutArm_Front,
		RunWithoutArm_Back,

		Move_Front,
		Move_Back,

		Attack_Front,
		Attack_Back,

		Death_Front,
		Death_Back,
	}

	public static class AnimationTypeExtension
	{
		public const string BASE_LAYER_TAG = "Base Layer.";

		public static string GetAnimationName(this AnimationType type)
		{
			return BASE_LAYER_TAG + type.ToString();
		}
	}
}
