using ECCLibrary.Data;
using ECCLibrary.Mono;
using System;

namespace ECCLibrary;

/// <summary>
/// Utility methods to help with constructing a creature prefab.
/// </summary>
public static partial class CreaturePrefabUtils
{
    /// <summary>
    /// Makes a given GameObject scannable with the Scanner Room by adding the <see cref="ResourceTracker"/> component.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="updatePositionPeriodically">Whether to automatically update the position of this ResourceTracker or not (should always be true for creatures).</param>
    public static void MakeObjectScannerRoomScannable(GameObject gameObject, bool updatePositionPeriodically)
    {
        ResourceTracker resourceTracker = gameObject.AddComponent<ResourceTracker>();
        resourceTracker.prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
        resourceTracker.rb = gameObject.GetComponent<Rigidbody>();
        if (updatePositionPeriodically == true)
        {
            gameObject.AddComponent<ResourceTrackerUpdater>();
        }
    }

    /// <summary>
    /// Multiplies incoming damage of the given DamageType <paramref name="type"/> by <paramref name="multiplier"/>.
    /// </summary>
    public static DamageModifier AddDamageModifier(GameObject creature, DamageType type, float multiplier)
    {
        var modifier = creature.AddComponent<DamageModifier>();
        modifier.damageType = type;
        modifier.multiplier = multiplier;
        return modifier;
    }

    /// <summary>
    /// Adds the <see cref="VFXFabricating"/> component onto a prefab to set up a model for when it is crafted in the Fabricator.
    /// This utility attempts to automatically determine settings when <paramref name="data"/> is null.
    /// </summary>
    public static VFXFabricating AddVFXFabricating(GameObject creature, VFXFabricatingData data)
    {
        GameObject modelObject = creature;

        if (data != null && !string.IsNullOrEmpty(data.pathToModel))
        {
            modelObject = creature.transform.Find(data.pathToModel).gameObject;
        }

        VFXFabricating vfxFabricating = modelObject.AddComponent<VFXFabricating>();

        // Determine automatic values if needed (unreliably so)

        if (data == null)
        {
            Renderer renderer = modelObject.GetComponentInChildren<Renderer>();

            vfxFabricating.scaleFactor = modelObject.transform.localScale.x;
            vfxFabricating.eulerOffset = modelObject.transform.localEulerAngles;
            vfxFabricating.posOffset = new Vector3(0f, renderer.bounds.extents.y, 0f);
            vfxFabricating.localMinY = -renderer.bounds.extents.y;
            vfxFabricating.localMaxY = renderer.bounds.extents.y;

            return vfxFabricating;
        }

        // Otherwise use 'data' as intended

        vfxFabricating.localMinY = data.minY;
        vfxFabricating.localMaxY = data.maxY;
        vfxFabricating.posOffset = data.posOffset;
        vfxFabricating.scaleFactor = data.scaleFactor;
        vfxFabricating.eulerOffset = data.eulerOffset;

        return vfxFabricating;
    }

    /// <summary>
    /// Adds the <see cref="Eatable"/> component to the given GameObject.
    /// </summary>
    public static Eatable AddEatable(GameObject prefab, EdibleData data)
    {
        var e = prefab.EnsureComponent<Eatable>();
        e.foodValue = data.foodAmount;
        e.waterValue = data.waterAmount;
        e.kDecayRate = 0.015f * data.decomposeSpeed;
        e.decomposes = data.decomposes;
        return e;
    }

    private static AnimationCurve maxRangeMultiplierCurve = new AnimationCurve(new(0, 1, 0, 0, .333f, .333f), new(0.5f, 0.5f, 0, 0, .333f, .333f), new(1, 1, 0, 0, .333f, .333f));
    private static AnimationCurve distanceAggressionMultiplierCurve = new AnimationCurve(new(0, 1, 0, 0, .333f, .333f), new(1, 0, -3, -3, .333f, .333f));

    /// <summary>
    /// Adds the <see cref="AggressiveWhenSeeTarget"/> component onto the object with the given <paramref name="data"/>.
    /// </summary>
    public static AggressiveWhenSeeTarget AddAggressiveWhenSeeTarget(GameObject creature, AggressiveWhenSeeTargetData data, LastTarget lastTarget, Creature creatureComponent)
    {
        var component = creature.AddComponent<AggressiveWhenSeeTarget>();
        component.maxRangeMultiplier = maxRangeMultiplierCurve;
        component.distanceAggressionMultiplier = distanceAggressionMultiplierCurve;
        component.lastTarget = lastTarget;
        component.creature = creatureComponent;
        component.targetType = data.targetType;
        component.aggressionPerSecond = data.aggressionPerSecond;
        component.maxRangeScalar = data.maxRangeScalar;
        component.maxSearchRings = data.maxSearchRings;
        component.ignoreSameKind = data.ignoreSameKind;
#if SUBNAUTICA
        component.targetShouldBeInfected = data.targetShouldBeInfected;
#endif
        component.minimumVelocity = data.minimumVelocity;
        component.hungerThreshold = data.hungerThreshold;
        return component;
    }

    #region Trail Managers

    /// <summary>
    /// Creates a <see cref="TrailManager"/>, which controls the procedural animations of tail-like objects.
    /// </summary>
    /// <param name="trailParent">The root of the spine and object that the <see cref="TrailManager"/> is added to. The first child of this object and all children of the first child are used for the trail.</param>
    /// <param name="components">The CreatureComponents of this creature.</param>
    /// <param name="segmentSnapSpeed">How fast each segment snaps back into the default position. A higher value gives a more rigid appearance.</param>
    /// <param name="maxSegmentOffset">How far each segment can be from the original position.</param>
    /// <param name="multiplier">The total strength of the movement. A value too low or too high will break the trail completely.</param>
    [Obsolete("See ECCLibrary.Data.TrailManagerBuilder instead.")]
    public static TrailManager CreateTrailManagerWithAllChildren(GameObject trailParent, CreatureComponents components, float segmentSnapSpeed, float maxSegmentOffset = -1f, float multiplier = 1f)
    {
        return CreateTrailManagerWithAllChildren(trailParent, components.BehaviourLOD, components.Creature.transform, segmentSnapSpeed, maxSegmentOffset, multiplier);
    }

    /// <summary>
    /// Creates a <see cref="TrailManager"/>, which controls the procedural animations of tail-like objects.
    /// </summary>
    /// <param name="trailParent">The root of the spine and object that the <see cref="TrailManager"/> is added to. The first child of this object and all children of the first child are used for the trail.</param>
    /// <param name="behaviourLOD">The BehaviourLOD of this creature.</param>
    /// <param name="creatureRoot">The creature's uppermost Transform.</param>
    /// <param name="segmentSnapSpeed">How fast each segment snaps back into the default position. A higher value gives a more rigid appearance.</param>
    /// <param name="maxSegmentOffset">How far each segment can be from the original position.</param>
    /// <param name="multiplier">The total strength of the movement. A value too low or too high will break the trail completely.</param>
    [Obsolete("See ECCLibrary.Data.TrailManagerBuilder instead.")]
    public static TrailManager CreateTrailManagerWithAllChildren(GameObject trailParent, BehaviourLOD behaviourLOD, Transform creatureRoot, float segmentSnapSpeed, float maxSegmentOffset = -1f, float multiplier = 1f)
    {
        trailParent.gameObject.SetActive(false);

        TrailManager tm = trailParent.AddComponent<TrailManager>();
        tm.trails = trailParent.transform.GetChild(0).GetComponentsInChildren<Transform>();
        tm.rootTransform = creatureRoot;
        tm.rootSegment = tm.transform;
        tm.levelOfDetail = behaviourLOD;
        tm.segmentSnapSpeed = segmentSnapSpeed;
        tm.maxSegmentOffset = maxSegmentOffset;
        tm.allowDisableOnScreen = false;
        TrailManagerUtilities.SetAllMultiplierCurves(tm, TrailManagerUtilities.FlatMultiplierAnimationCurve);

        trailParent.gameObject.SetActive(true);
        return tm;
    }

    /// <summary>
    /// Creates a <see cref="TrailManager"/>, which controls the procedural animations of tail-like objects.
    /// </summary>
    /// <param name="trailRoot">The root of the spine and object that the <see cref="TrailManager"/> is added to.</param>
    /// <param name="components">The CreatureComponents of this creature.</param>
    /// <param name="trails">Any objects that are simulated. Should NOT include the <paramref name="trailRoot"/>'s transform.</param>
    /// <param name="segmentSnapSpeed">How fast each segment snaps back into the default position. A higher value gives a more rigid appearance.</param>
    /// <param name="maxSegmentOffset">How far each segment can be from the original position.</param>
    [Obsolete("See ECCLibrary.Data.TrailManagerBuilder instead.")]
    public static TrailManager CreateTrailManagerManually(GameObject trailRoot, CreatureComponents components, Transform[] trails, float segmentSnapSpeed, float maxSegmentOffset = -1f)
    {
        return CreateTrailManagerManually(trailRoot, components.BehaviourLOD, components.Creature.transform, trails, segmentSnapSpeed, maxSegmentOffset); 
    }

    /// <summary>
    /// Creates a <see cref="TrailManager"/>, which controls the procedural animations of tail-like objects.
    /// </summary>
    /// <param name="trailRoot">The root of the spine and object that the <see cref="TrailManager"/> is added to.</param>
    /// <param name="behaviourLOD">The BehaviourLOD of this creature.</param>
    /// <param name="creatureRoot">The creature's uppermost Transform.</param>
    /// <param name="trails">Any objects that are simulated. Should NOT include the <paramref name="trailRoot"/>'s transform.</param>
    /// <param name="segmentSnapSpeed">How fast each segment snaps back into the default position. A higher value gives a more rigid appearance.</param>
    /// <param name="maxSegmentOffset">How far each segment can be from the original position.</param>
    [Obsolete("See ECCLibrary.Data.TrailManagerBuilder instead.")]
    public static TrailManager CreateTrailManagerManually(GameObject trailRoot, BehaviourLOD behaviourLOD, Transform creatureRoot, Transform[] trails, float segmentSnapSpeed, float maxSegmentOffset = -1f)
    {
        trailRoot.gameObject.SetActive(false);

        TrailManager tm = trailRoot.AddComponent<TrailManager>();
        tm.trails = trails;
        tm.rootTransform = creatureRoot;
        tm.rootSegment = tm.transform;
        tm.levelOfDetail = behaviourLOD;
        tm.segmentSnapSpeed = segmentSnapSpeed;
        tm.maxSegmentOffset = maxSegmentOffset;
        tm.allowDisableOnScreen = false;
        TrailManagerUtilities.SetAllMultiplierCurves(tm, TrailManagerUtilities.FlatMultiplierAnimationCurve);

        trailRoot.gameObject.SetActive(true);
        return tm;
    }

    #endregion

    /// <summary>
    /// Adds an instance of the <see cref="OnTouch"/> component onto <paramref name="triggerObject"/> that calls the method as defined by the parameters.
    /// </summary>
    /// <param name="triggerObject">The object that holds the touch trigger, for example a creature's mouth collider. Must have Collider with <see cref="Collider.isTrigger"/> set to TRUE.</param>
    /// <param name="callbackObject">The GameObject that holds the callback component and method.</param>
    /// <param name="callbackComponentTypeName">The name of the type that holds the action performed when the object is collided with.</param>
    /// <param name="callbackMethodName">The name of the method that is performed when the object is collided with.</param>
    /// <returns></returns>
    public static OnTouch AddOnTouchTrigger(GameObject triggerObject, GameObject callbackObject, string callbackComponentTypeName, string callbackMethodName)
    {
        triggerObject.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.organic;
        var collider = triggerObject.GetComponent<Collider>();
        if (collider == null) ECCPlugin.logger.LogError($"No Collider found on trigger object '{triggerObject}'. This WILL cause errors!");
        else if (!collider.isTrigger) ECCPlugin.logger.LogError($"Collider '{collider}' is not a trigger! This will NOT work!");
        var onTouch = triggerObject.EnsureComponent<OnTouch>();
        var setDelayed = triggerObject.EnsureComponent<SetOnTouchCallbackDelayed>();
        setDelayed.onTouch = onTouch;
        setDelayed.callbackGameObject = callbackObject;
        setDelayed.callbackTypeName = callbackComponentTypeName;
        setDelayed.callbackMethodName = callbackMethodName;
        return onTouch;
    }

    /// <summary>
    /// Assigns the essential fields of any sort of MeleeAttack component. This utility does not cover all possible features.
    /// </summary>
    /// <typeparam name="T">Type which must be the same as or inherit from MeleeAttack.</typeparam>
    /// <param name="creature">The creature prefab root.</param>
    /// <param name="components">Components reference object.</param>
    /// <param name="mouth">The object that has the bite trigger.</param>
    /// <param name="automaticallyAddOnTouchCallback">If true, the <see cref="OnTouch"/> component will be added and properly assigned.</param>
    /// <param name="damage">Bite damage.</param>
    /// <param name="interval">Seconds between each bite.</param>
    /// <param name="canBiteVehicle">If false, this creature is unable to attack the Seamoth and PRAWN Suit.</param>
    /// <returns>The newly added instance of the component.</returns>
    public static T AddMeleeAttack<T>(GameObject creature, CreatureComponents components, GameObject mouth, bool automaticallyAddOnTouchCallback, float damage, float interval = 1f, bool canBiteVehicle = true) where T : MeleeAttack
    {
        var meleeAttack = creature.AddComponent<T>();

        meleeAttack.mouth = mouth;

        if (automaticallyAddOnTouchCallback)
        {
            AddOnTouchTrigger(mouth, creature, typeof(T).Name, "OnTouch");
        }

        meleeAttack.lastTarget = components.LastTarget;
        meleeAttack.creature = components.Creature;
        meleeAttack.liveMixin = components.LiveMixin;
        meleeAttack.animator = components.Animator;

        meleeAttack.biteDamage = damage;
        meleeAttack.biteInterval = interval;
        meleeAttack.canBiteVehicle = canBiteVehicle;

        return meleeAttack;
    }
}