using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

namespace Game.Characters.Zombie
{
    public class CasualZombieAgent : MonoBehaviour
    {
        [SerializeField]
        private ZombieAgentConfig agentConfig;
        
        [SerializeField]
        private BehaviorTree tree;

        private Vector3? wanderPoint;
        private GameObject player;
        
        private CharacterActor actor;
        private NavMeshAgent agent;
        private void Awake()
        {
            actor = GetComponent<CharacterActor>();
            agent = GetComponent<NavMeshAgent>();
            
            ImportCharacterStats();
            ResetStats();
            tree = new BehaviorTreeBuilder(gameObject)
                .Selector()
                    .Sequence()
                        .Condition(() => player != null) 
                        .Selector()
                            .Do(() => ZombieAgentCallables.MoveToPoint(agent, player.transform.position, 2f))
                            .Do(() =>
                            {
                                Debug.Log($"Attack {player.name}");
                                return TaskStatus.Success;
                            })
                        .End()
                    .End()
                    .Sequence()
                        .Condition(() => !wanderPoint.HasValue)
                        .Do(() => ZombieAgentCallables.PickNextWanderPoint(agent, agentConfig, ref wanderPoint))
                    .End()
                    .Sequence()
                        .Condition(() => wanderPoint.HasValue)
                        .Do(() => ZombieAgentCallables.MoveToPoint(agent, wanderPoint ?? agent.transform.position, agent.stoppingDistance))
                    .End()
                    .Sequence()
                        .Do(ResetStats)
                    .End()
                .End()
                .Build();

        }

        private void Update()
        {
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
        }
    }
}