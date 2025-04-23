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

            wanderPoint = NavMesh.SamplePosition(randomDirection, out var hit, radius, agent.areaMask)
                ? hit.position
                : null;

            return TaskStatus.Success;
        }

        public static TaskStatus MoveToPoint(NavMeshAgent agent, Vector3 dest, float stoppingDistance)
        {
            Debug.DrawLine(agent.transform.position, dest, Color.blue);

            if ((agent.transform.position - dest).magnitude <= stoppingDistance) return TaskStatus.Failure;

            if (agent.destination.Equals(dest))
            {
                return !agent.hasPath ? TaskStatus.Failure : TaskStatus.Success;
            }

            agent.isStopped = false;
            agent.SetDestination(dest);
            return TaskStatus.Success;
        }

        public static bool CanSeePlayer(Vector3 agentEyePos, Collider playerCol, float maxSight, LayerMask envMask)
        {
            var sqrDist = Mathf.Pow(maxSight, 2);
            var lookDir = playerCol.transform.position - agentEyePos;
            if (lookDir.sqrMagnitude > sqrDist) return false;

            var spot = playerCol.transform.position;

            //check see feet
            spot.y = playerCol.bounds.min.y;
            if (!Physics.Linecast(agentEyePos, spot, envMask)) return true;

            //check see head
            spot.y = playerCol.bounds.max.y;
            return !Physics.Linecast(agentEyePos, spot, envMask);
        }
    }
}