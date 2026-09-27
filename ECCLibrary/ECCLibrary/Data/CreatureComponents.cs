namespace ECCLibrary.Data;

/// <summary>
/// Holds references to many common components used by creatures. Some properties will be unassigned/null.
/// </summary>
public struct CreatureComponents
{
    /// <summary> </summary>
    public PrefabIdentifier PrefabIdentifier { get; internal set; }
    /// <summary> </summary>
    public TechTag TechTag { get; internal set; }
    /// <summary> </summary>
    public LargeWorldEntity LargeWorldEntity { get; internal set; }
    /// <summary> </summary>
    public EntityTag EntityTag { get; internal set; }
    /// <summary> </summary>
    public SkyApplier SkyApplier { get; internal set; }
    /// <summary> </summary>
    public Animator Animator { get; internal set; }
    /// <summary> </summary>
    public EcoTarget EcoTarget { get; internal set; }
    /// <summary> </summary>
    public Eatable Eatable { get; internal set; }
    /// <summary> </summary>
    public VFXSurface VfxSurface { get; internal set; }
    /// <summary> </summary>
    public BehaviourLOD BehaviourLOD { get; internal set; }
    /// <summary> </summary>
    public Rigidbody Rigidbody { get; internal set; }
    /// <summary> </summary>
    public WorldForces WorldForces { get; internal set; }
    /// <summary> </summary>
    public Creature Creature { get; internal set; }
    /// <summary> </summary>
    public LiveMixin LiveMixin { get; internal set; }
    /// <summary> </summary>
    public AttackLastTarget AttackLastTarget { get; internal set; }
    /// <summary> </summary>
    public AttackCyclops AttackCyclops { get; internal set; }
    /// <summary> </summary>
    public LastTarget LastTarget { get; internal set; }
    /// <summary> </summary>
    public SwimBehaviour SwimBehaviour { get; internal set; }
    /// <summary> </summary>
    public Locomotion Locomotion { get; internal set; }
    /// <summary> </summary>
    public SplineFollowing SplineFollowing { get; internal set; }
    /// <summary> </summary>
    public SwimRandom SwimRandom { get; internal set; }
    /// <summary> </summary>
    public InfectedMixin InfectedMixin { get; internal set; }
    /// <summary> </summary>
    public Pickupable Pickupable { get; internal set; }
    /// <summary> </summary>
    public AnimateByVelocity AnimateByVelocity { get; internal set; }
    /// <summary> </summary>
    public CreatureDeath CreatureDeath { get; internal set; }
    /// <summary> </summary>
    public CreatureFlinch CreatureFlinch { get; internal set; }
    /// <summary> </summary>
    public DeadAnimationOnEnable DeadAnimationOnEnable { get; internal set; }
    /// <summary> </summary>
    public CreatureFear CreatureFear { get; internal set; }
    /// <summary> </summary>
    public FleeWhenScared FleeWhenScared { get; internal set; }
    /// <summary> </summary>
    public FleeOnDamage FleeOnDamage { get; internal set; }
    /// <summary> </summary>
    public Scareable Scareable { get; internal set; }
    /// <summary> </summary>
    public SoundOnDamage SoundOnDamage { get; internal set; }
    /// <summary> </summary>
    public StayAtLeashPosition StayAtLeashPosition { get; internal set; }
    /// <summary> </summary>
    public AggressiveToPilotingVehicle AggressiveToPilotingVehicle { get; internal set; }
}