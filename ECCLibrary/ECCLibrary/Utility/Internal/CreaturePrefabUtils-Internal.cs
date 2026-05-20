using ECCLibrary.Data;
using ECCLibrary.Mono;

namespace ECCLibrary;

/// <summary>
/// Utilities related to creating a creature prefab object.
/// </summary>
public static partial class CreaturePrefabUtils
{
    internal static SwimBehaviour AddSwimBehaviour(GameObject creature, SwimBehaviourData data, SplineFollowing splineFollowing)
    {
        var component = creature.EnsureComponent<SwimBehaviour>();
        component.turnSpeed = data.turnSpeed;
        component.splineFollowing = splineFollowing;
        return component;
    }

    internal static SwimRandom AddSwimRandom(GameObject creature, SwimRandomData data)
    {
        var component = creature.EnsureComponent<SwimRandom>();
        component.evaluatePriority = data.evaluatePriority;
        component.swimRadius = data.swimRadius;
        component.swimForward = data.swimForward;
        component.swimVelocity = data.swimVelocity;
        component.swimInterval = data.swimInterval;
        component.onSphere = data.onSphere;
        return component;
    }

    internal static Locomotion AddLocomotion(GameObject creature, LocomotionData data, BehaviourLOD levelOfDetail, Rigidbody rigidbody)
    {
        var component = creature.EnsureComponent<Locomotion>();
        component.levelOfDetail = levelOfDetail;
        component.useRigidbody = rigidbody;

        component.maxAcceleration = data.maxAcceleration;
        component.forwardRotationSpeed = data.forwardRotationSpeed;
        component.upRotationSpeed = data.upRotationSpeed;
        component.driftFactor = data.driftFactor;
        component.canMoveAboveWater = data.canMoveAboveWater;
        component.canWalkOnSurface = data.canWalkOnSurface;
        component.freezeHorizontalRotation = data.freezeHorizontalRotation;
        return component;
    }

    internal static SplineFollowing AddSplineFollowing(GameObject creature, Rigidbody rb, Locomotion locomotion, BehaviourLOD behaviourLOD)
    {
        var component = creature.EnsureComponent<SplineFollowing>();
        component.useRigidbody = rb;
        component.locomotion = locomotion;
        component.levelOfDetail = behaviourLOD;
        return component;

    }

    internal static LiveMixin AddLiveMixin(GameObject creature, LiveMixinData data)
    {
        var component = creature.EnsureComponent<LiveMixin>();
        component.data = data;
        component.health = data.maxHealth;
        return component;
    }

    internal static StayAtLeashPosition AddStayAtLeashPosition(GameObject creature, StayAtLeashData stayAtLeashData)
    {
        var component = creature.EnsureComponent<StayAtLeashPosition>();
        component.evaluatePriority = stayAtLeashData.evaluatePriority;
        component.leashDistance = stayAtLeashData.leashDistance;
        component.swimVelocity = stayAtLeashData.swimVelocity;
        component.swimInterval = stayAtLeashData.swimInterval;
        component.minSwimDuration = stayAtLeashData.minSwimDuration;
        return component;
    }

    internal static FleeOnDamage AddFleeOnDamage(GameObject creature, FleeOnDamageData data)
    {
        var component = creature.EnsureComponent<FleeOnDamage>();
        component.evaluatePriority = data.evaluatePriority;
        component.damageThreshold = data.damageThreshold;
        component.fleeDuration = data.fleeDuration;
        component.minFleeDistance = data.minFleeDistance;
        component.swimVelocity = data.swimVelocity;
        component.swimInterval = data.swimInterval;
        return component;
    }

    internal static Scareable AddScareable(GameObject creature, ScareableData data, CreatureFear fear, Creature creatureComponent, FleeWhenScared fleeWhenScared)
    {
        var component = creature.EnsureComponent<Scareable>();
        component.targetType = data.targetType;
        component.creatureFear = fear;
        component.creature = creatureComponent;
        component.fleeAction = fleeWhenScared;
        component.scarePerSecond = data.scarePerSecond;
        component.maxRangeScalar = data.maxRangeScalar;
        component.minMass = data.minMass;
        component.updateTargetInterval = data.updateTargetInterval;
        component.updateRange = data.updateRange;
        return component;
    }

    internal static AnimateByVelocity AddAnimateByVelocity(GameObject creatureRoot, GameObject modelRoot, AnimateByVelocityData data, Animator animator, Rigidbody rigidbody, BehaviourLOD behaviourLOD)
    {
        var component = modelRoot.EnsureComponent<AnimateByVelocity>();
        component.animator = animator;
        component.animationMoveMaxSpeed = data.animationMoveMaxSpeed;
        component.animationMaxPitch = data.animationMaxPitch;
        component.animationMaxTilt = data.animationMaxTilt;
        component.useStrafeAnimation = data.useStrafeAnimation;
        component.rootGameObject = creatureRoot;
        component.dampTime = data.dampTime;
        component.levelOfDetail = behaviourLOD;
        return component;
    }

    internal static FleeWhenScared AddFleeWhenScared(GameObject creature, FleeWhenScaredData data, CreatureFear fear)
    {
        var component = creature.EnsureComponent<FleeWhenScared>();
        component.creatureFear = fear;
#if SUBNAUTICA
        component.exhausted = new CreatureTrait(0f, 0.05f);
#elif BELOWZERO
        component.exhausted = new CreatureTrait {falloff = 0.05f};
#endif
        component.evaluatePriority = data.evaluatePriority;
        component.swimVelocity = data.swimVelocity;
        component.swimInterval = data.swimInterval;
        component.avoidanceIterations = data.avoidanceIterations;
        component.swimTiredness = data.swimTiredness;
        component.tiredVelocity = data.tiredVelocity;
        component.swimExhaustion = data.swimExhaustion;
        component.exhaustedVelocity = data.exhaustedVelocity;
        return component;
    }

    internal static AvoidObstacles AddAvoidObstacles(GameObject creature, AvoidObstaclesData data, LastTarget lastTarget)
    {
        var component = creature.EnsureComponent<AvoidObstacles>();
        component.lastTarget = lastTarget;
        component.evaluatePriority = data.evaluatePriority;
        component.avoidTerrainOnly = data.avoidTerrainOnly;
        component.avoidanceIterations = data.avoidanceIterations;
        component.avoidanceDistance = data.avoidanceDistance;
        component.avoidanceDuration = data.avoidanceDuration;
        component.scanInterval = data.scanInterval;
        component.scanDistance = data.scanDistance;
        component.scanRadius = data.scanRadius;
        component.swimVelocity = data.swimVelocity;
        component.swimInterval = 1f;
        component.minActionCheckInterval = 3; // this is crucial to update frequently enough
        return component;
    }

    internal static AvoidTerrain AddAvoidTerrain(GameObject creature, AvoidTerrainData data)
    {
        var component = creature.EnsureComponent<AvoidTerrain>();
        component.evaluatePriority = data.evaluatePriority;
        component.avoidanceDistance = data.avoidanceDistance;
        component.avoidanceForward = data.avoidanceForward;
        component.swimVelocity = data.swimVelocity;
        component.scanDistance = data.scanDistance;
        component.avoidanceIterations = data.avoidanceIterations;
        component.minActionCheckInterval = 2;
        return component;
    }

    internal static SwimInSchool AddSwimInSchool(GameObject creature, SwimInSchoolData data)
    {
        var component = creature.EnsureComponent<SwimInSchool>();
        component.evaluatePriority = data.evaluatePriority;
        component.kBreakDistance = data.breakDistance;
        component.percentFindLeaderRespond = data.percentFindLeaderRespond;
        component.chanceLoseLeader = data.chanceLoseLeader;
        component.schoolSize = data.schoolSize;
        component.swimVelocity = data.swimVelocity;
        component.swimInterval = data.swimInterval;
        var setter = creature.AddComponent<SwimInSchoolFieldSetter>();
        setter.behaviour = component;
        setter.breakDistance = data.breakDistance;
        setter.chanceLoseLeader = data.chanceLoseLeader;
        setter.percentFindLeaderRespond = data.percentFindLeaderRespond;
        return component;
    }

    internal static CreatureFlinch AddCreatureFlinch(GameObject creature, Animator animator)
    {
        var component = creature.EnsureComponent<CreatureFlinch>();
        component.animator = animator;
        return component;
    }

    internal static AttackLastTarget AddAttackLastTargetData(GameObject creature, AttackLastTargetData data, LastTarget lastTarget)
    {
        var component = creature.AddComponent<AttackLastTarget>();
        component.evaluatePriority = data.evaluatePriority;
        component.swimVelocity = data.swimVelocity;
        component.aggressionThreshold = data.aggressionThreshold;
        component.minAttackDuration = data.minAttackDuration;
        component.maxAttackDuration = data.maxAttackDuration;
        component.pauseInterval = data.pauseInterval;
        component.rememberTargetTime = data.rememberTargetTime;
        component.resetAggressionOnTime = data.resetAggressionOnTime;
        component.lastTarget = lastTarget;
        component.minActionCheckInterval = 3;
        return component;
    }

    internal static AttackCyclops AddAttackCyclops(GameObject creature, AttackCyclopsData data, LastTarget lastTarget)
    {
        var component = creature.AddComponent<AttackCyclops>();
        component.lastTarget = lastTarget;
        component.evaluatePriority = data.evaluatePriority;
        component.aggressPerSecond = data.aggressPerSecond;
        component.attackAggressionThreshold = data.attackAggressionThreshold;
        component.attackPause = data.attackPause;
        component.maxDistToLeash = data.maxDistToLeash;
        component.swimVelocity = data.swimVelocity;
#if SUBNAUTICA
        component.aggressiveToNoise = new CreatureTrait(0f, data.aggressionFalloff);
#elif BELOWZERO
        component.aggressiveToNoise = new CreatureTrait { falloff = data.aggressionFalloff };
#endif
        component.minActionCheckInterval = 2;
        return component;
    }

    internal static AggressiveToPilotingVehicle AddAggressiveToPilotingVehicle(GameObject creature, AggressiveToPilotingVehicleData data, Creature creatureComponent, LastTarget lastTarget)
    {
        var component = creature.AddComponent<AggressiveToPilotingVehicle>();
        component.lastTarget = lastTarget;
        component.creature = creatureComponent;
        component.range = data.range;
        component.aggressionPerSecond = data.aggressionPerSecond;
        component.updateAggressionInterval = data.updateAggressionInterval;
        return component;
    }

    internal static WaterParkCreature AddWaterParkCreature(GameObject creature, WaterParkCreatureData dataScriptableObject)
    {
        var c = creature.EnsureComponent<WaterParkCreature>();
        c.data = dataScriptableObject;
        return c;
    }
}