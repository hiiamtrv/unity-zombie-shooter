using System;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using Game.Characters.Input;
using Game.Interfaces;
using Game.SkinPool;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Characters.Zombie
{
    public class CasualZombieAgent : MonoBehaviour, IVisualPoolObjectConsumer
    {
        [SerializeField]
        private ZombieAgentConfig agentConfig;
        
        [SerializeField]
        private BehaviorTree tree;

        [SerializeField]
        private bool isSeen;

        private Vector3? wanderPoint;
        private Collider playerCol;
        
        private CharacterActor actor;
        private NavMeshAgent agent;
        private float height;

        private CharacterAgentInput input;
        private bool isChasing;

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
                            isChasing = false;
                            return TaskStatus.Success;
                        })
                        .Selector()
                            .Do(() => ZombieAgentCallables.MoveToPoint(agent, playerCol.transform.position, agentConfig.attackRange))
                            .Do(() =>
                                {
                                    agent.isStopped = true;
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
            isChasing = true;
            isSeen = false;
            SnapToNavMesh();
        }

        private void OnDisable()
        {
            ResetStats();
        }

        private void Update()
        {
            if (!isSeen) return;
            
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
            isChasing = false;
            isSeen = false;
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
                agent.transform.position = hit.position;
                agent.Warp(hit.position); 
            }
        }

        public void LoadVisualPoolObject(VisualPoolObject visual)
        {
            isSeen = true;
        }

        public void UnloadVisualPoolObject()
        {
            isSeen = false;
        }
    }
}