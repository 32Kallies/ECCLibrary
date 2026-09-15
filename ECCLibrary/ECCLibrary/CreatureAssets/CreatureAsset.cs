using ECCLibrary.Data;
using ECCLibrary.Mono;

namespace ECCLibrary;

/// <summary>
/// Override this class to define a new creature. Call the <see cref="Register"/> method on an instance to add it to the game.
/// </summary>
public partial class CreatureAsset
{
    internal CreatureTemplate Template { get; set; }

    /// <summary>
    /// Essential information for registering the prefab.
    /// </summary>
    public PrefabInfo PrefabInfo { get; }

    /// <summary>
    /// A reference to the custom prefab instance for this creature. Note that this custom prefab is registered once the <see cref="Register"/> method is called.
    /// </summary>
    public ICustomPrefab CustomPrefab => CustomPrefabInstance;

    private CustomPrefab CustomPrefabInstance { get; }

    private bool _registered;

    /// <summary>
    /// Instantiates a Creature Asset with the given PrefabInfo. Call the Register method to add the creature to the game.
    /// </summary>
    /// <param name="prefabInfo">
    /// <para>Information required for spawning. Must be unique.</para>
    /// <para>An instance of this struct can be easily created by calling <see cref="PrefabInfo.WithTechType"/>.</para></param>
    public CreatureAsset(PrefabInfo prefabInfo)
    {
        PrefabInfo = prefabInfo;
        CustomPrefabInstance = new CustomPrefab(prefabInfo);
    }

    /// <summary>
    /// The ClassID of this creature, sourced from the PrefabInfo property.
    /// </summary>
    public string ClassID => PrefabInfo.ClassID;

    /// <summary>
    /// The TechType of this creature, sourced from the PrefabInfo property.
    /// </summary>
    public TechType TechType => PrefabInfo.TechType;

    /// <summary>
    /// The EntityInfo for this creature, only assigned <i>after</i> <see cref="Register"/> is called.
    /// </summary>
    public UWE.WorldEntityInfo EntityInfo { get; private set; }

    /// <summary>
    /// Registers this creature's CustomPrefab into the game. This should only ever be called once, after everything else related to the creature has been finalized.
    /// </summary>
    public void Register()
    {
        if (_registered)
        {
            ECCPlugin.logger.LogError($"Preventing double-registration of creature '{this}'. Only ever call Register once per CreatureAsset.");
            return;
        }
        
        _registered = true;
        
        Template = CreateTemplate();

        // Check validity of essentials

        if (PrefabInfo.TechType == TechType.None)
        {
            ECCPlugin.logger.LogError($"Attempting to register creature '{this}' without a valid TechType! Exiting early.");
            return;
        }

        var registeringTechTypeForFirstTime = SanityChecking.TryRegisterTechTypeForFirstTime(TechType); 
        if (!registeringTechTypeForFirstTime)
        {
            ECCPlugin.logger.LogWarning($"Registering multiple creatures with the same TechType ('{TechType}')! " +
                                        $"The new creature of Class ID '{ClassID}' will NOT override any TechType-specific data.");
        }

        // Assign patch-time data

        if (registeringTechTypeForFirstTime)
        {
            RegisterTechTypeData();
        }
        
        EntityInfo = new UWE.WorldEntityInfo { cellLevel = Template.CellLevel, classId = ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Creature, techType = TechType };
        WorldEntityDatabaseHandler.AddCustomInfo(ClassID, EntityInfo);

        // Register the Custom Prefab

        if (Template.TechTypeToClone is not TechType.None)
        {
            CustomPrefabInstance.SetGameObject(new CloneTemplate(PrefabInfo, Template.TechTypeToClone)
            {
                ModifyPrefabAsync = ModifyPrefabAsync
            });
        }
        else
        { 
            CustomPrefabInstance.SetGameObject(GetGameObject);
        }

        CustomPrefabInstance.Register();

        PostRegister();
    }

    private void RegisterTechTypeData()
    {
        if (Template.AcidImmune) CreatureDataUtils.SetAcidImmune(TechType);
        if (Template.BioReactorCharge > 0f) CreatureDataUtils.SetBioreactorCharge(TechType, Template.BioReactorCharge);
        if (Template.PickupableFishData != null && Template.PickupableFishData.CanBeHeld) CraftDataHandler.SetEquipmentType(TechType, EquipmentType.Hand);
        CreatureDataUtils.SetBehaviorType(TechType, Template.BehaviourType);
        CreatureDataUtils.SetItemSounds(TechType, Template.ItemSoundsType);
    }

    /// <summary>
    /// An empty method that can be overriden to insert code that runs directly after the prefab is registered (runs at patch time immediately after <see cref="Register"/> is called).
    /// </summary>
    protected virtual void PostRegister() { }

    /// <summary>
    /// This method expects a <see cref="CreatureTemplate"/> instance. This class holds all the settings regarding the creature's automatic prefab initialization, alongside various patch-time factors.
    /// </summary>
    /// <returns></returns>
    protected abstract CreatureTemplate CreateTemplate();

    /// <summary>
    /// Changes to the prefab can be applied here.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    protected abstract IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components);

    /// <summary>
    /// By default calls <see cref="MaterialUtils.ApplySNShaders"/> to convert the materials of the entire prefab. Can be overriden to have more control over the process.
    /// </summary>
    /// <param name="prefab"></param>
    protected virtual void ApplyMaterials(GameObject prefab)
    {
        MaterialUtils.ApplySNShaders(prefab);
    }
}