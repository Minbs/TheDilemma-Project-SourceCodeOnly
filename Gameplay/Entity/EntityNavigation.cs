using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

namespace Gameplay
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class EntityNavigation : MonoBehaviour
	{
#if UNITY_EDITOR
		public virtual void OnValidate()
		{
			mNavigationAgent = GetComponent<NavMeshAgent>();
		}
#endif

		[SerializeField] private NavMeshAgent mNavigationAgent;
		[ShowInInspector] public Vector2 Velocity => mNavigationAgent.velocity;
		[ShowInInspector] public Vector2 MoveDirection => mNavigationAgent.velocity.normalized;

		//private float mDecisionDelay = 0;

		protected Entity_Creature Mine;

		public Vector3 Destination { get; private set; }

		#region Events

		public void Initialize(Entity_Creature entity)
		{
			Mine = entity;

			if (!Mine.IsServerSide)
			{
				mNavigationAgent.enabled = false;
				return;
			}

			// Initialize Position
			mNavigationAgent.Warp(transform.position);
			mNavigationAgent.updateRotation = false;
			mNavigationAgent.updateUpAxis = false;

			// Initialize NavMesh
			mNavigationAgent.speed = 0;
			mNavigationAgent.destination = entity.SpawnPosition;
			mNavigationAgent.isStopped = false;

			Destination = mNavigationAgent.destination;
			//this.Stop();
		}

		public void OnUpdateDestinaion(in DeltaTimeInfo deltaTimeInfo)
		{
			mNavigationAgent.speed = Mine.MoveSpeed;
			mNavigationAgent.destination = Destination;
		}

		#endregion

		#region Navigation

		public bool CanReach(Vector3 target)
		{
			NavMeshPath path = new NavMeshPath();

			return mNavigationAgent.CalculatePath(target, path) &&
				path.status == NavMeshPathStatus.PathComplete;
		}

		public void SetDestination(EntityBase target)
		{
			if (target == null)
			{
				return;
			}

			this.Destination = target.transform.position;
		}

		public void SetDestination(Vector3 destination)
		{
			this.Destination = destination;
		}

		public void Stop()
		{
			//mNavigationAgent.isStopped = true;
		}

		public void Warp(Vector3 position)
		{
			mNavigationAgent.Warp(position);
		}

		#endregion
	}
}
