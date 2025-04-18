using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters.Zombie
{
    public static class ZombieAgentCallables
    {
        public static TaskStatus PickNextWanderPoint(NavMeshAgent agent, ZombieAgentConfig cfg,
            ref Vector3? wanderPoint)
        {
            var radius = Random.Range(cfg.minMaxWanderDistance.x, cfg.minMaxWanderDistance.y);
            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += agent.transform.position;

            wanderPoint =
                NavMesh.SamplePosition(randomDirection, out var hit, radius, agent.areaMask)
                    ? hit.position
                    : null;

            return TaskStatus.Success;
        }

        public static TaskStatus MoveToPoint(NavMeshAgent agent, Vector3 dest, float stoppingDistance)
        {
            if (agent.destination.Equals(dest))
            {
                if (agent.remainingDistance <= stoppingDistance) return TaskStatus.Failure;
                if (!agent.pathPending && !agent.hasPath) return TaskStatus.Failure;
                return TaskStatus.Success;
            }

            agent.SetDestination(dest);
            return TaskStatus.Success;
        }
    }
}