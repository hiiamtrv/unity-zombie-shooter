using System;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using Game.Characters.Input;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters.Zombie
{
    public class CasualZombieAgent : MonoBehaviour
    {
        [SerializeField]
        private ZombieAgentConfig agentConfig;
        
        [SerializeField]
        private BehaviorTree tree;

        private Vector3? wanderPoint;
        private Collider playerCol;
        
        private CharacterActor actor;
        private NavMeshAgent agent;
        private float height;

        private CharacterAgentInput input;

        private void Awake()
        {
            height = GetComponent<Collider>().bounds.size.y;
            
            input = GetComponent<CharacterAgentInput>();
            actor = GetComponent<CharacterActor>();
            agent = GetComponent<NavMeshAgent>();
            playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
            
            ImportCharacterStats();
            tree = new BehaviorTreeBuilder(gameObject)
                .Selector()
                    .Sequence("Dead") 
                        .Condition(() => actor?.IsDead() ?? true)
                    .End()
                    .Sequence("Chase n Attack")
                        .Condition(() => playerCol != null) 
                        .Condition(() => ZombieAgentCallables.CanSeePlayer(
                        transform.position + Vector3.up * height,
                            playerCol, 
                            agentConfig.maxSight, 
                            agentConfig.sightBlockerMask
                        ))
                        .Do(() =>
                        {
                            wanderPoint = null;
                            return TaskStatus.Success;
                        })
                        .Selector()
                            .Do(() => ZombieAgentCallables.MoveToPoint(agent, playerCol.transform.position, agentConfig.attackRange))
                            .Do(() =>
                                {
                                    agent.SetDestination(agent.transform.position);
                                    input.SetAttacking(true);
                                    return TaskStatus.Success;
                                })
                        .End()
                    .End()
                    .Sequence("Wander")
                        .Selector()
                            .Sequence()
                                .Condition(() => !wanderPoint.HasValue)
                                .Do(() => ZombieAgentCallables.PickNextWanderPoint(agent, agentConfig, ref wanderPoint))
                            .End()
                            .Do(() => ZombieAgentCallables.MoveToPoint(agent, wanderPoint ?? agent.transform.position, agent.stoppingDistance))
                        .End()
                    .End()
                    .Sequence("Reset")
                        .Do(ResetStats)
                    .End()
                .End()
                .Build();
        }

        private void OnEnable()
        {
            SnapToNavMesh();
            agent.enabled = true;
        }

        private void OnDisable()
        {
            ResetStats();
            agent.enabled = false;
        }

        private void Update()
        {
            // Reset tick stats
            input.SetAttacking(false);
            agent.speed = actor.GetMoveSpeed();
            if (agent.speed == 0)
            {
                agent.velocity = Vector3.zero; //reset velocity
            }
            
            // Update our tree every frame
            tree.Tick();
        }

        private TaskStatus ResetStats()
        {
            wanderPoint = null;
            return TaskStatus.Success;
        }

        private void ImportCharacterStats()
        {
            agent.speed = actor.config.moveSpeed;
            agent.acceleration = agent.speed * 2.0f;
        }
      
        public void SnapToNavMesh()
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var hit, 1000f, NavMesh.GetAreaFromName("Walkable")))
            {
                Debug.Log($"Snap to mesg {hit.position}");
                agent.transform.position = hit.position;
                agent.Warp(hit.position); 
            }
            else
            {
                agent.Warp(transform.position);
            }
        }
    }
}