using System.Reflection;
using ECCLibrary.Data;

namespace ECCLibrary;

/// <summary>
/// Manages the creation of cooked and cured creatures.
/// </summary>
public static class CookedCreatureHandler
{
    /// <summary>
    /// Advanced method to create a cooked/cured creature.
    /// </summary>
    public static void RegisterEdibleVariant(PrefabInfo prefabInfo, GameObject creatureModel, EdibleData edibleData, RecipeData recipe,
        string[] fabricatorSteps, TechCategory category, VFXFabricatingData vfxFabricatingSettings = default)
    {
        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetRecipe(recipe)
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab(fabricatorSteps);
        prefab.SetPdaGroupCategory(TechGroup.Survival, category);
        prefab.SetGameObject(new CookedCreatureTemplate(prefabInfo, creatureModel, edibleData, vfxFabricatingSettings));
        prefab.Register();
    }

    /// <summary>
    /// The fastest way to create a cooked AND cured variant of the given creature. Automatically assigns the Analysis Tech so it is unlocked with the creature.
    /// </summary>
    /// <param name="creature">The original uncooked creature. MUST have been registered already.</param>
    /// <param name="cookedName">Name of the cooked fish item.</param>
    /// <param name="cookedDescription">Description of the cooked fish item.</param>
    /// <param name="cookedSprite">Icon of the cooked fish item.</param>
    /// <param name="curedName">Name of the cured fish item.</param>
    /// <param name="curedDescription">Description of the cured fish item.</param>
    /// <param name="curedSprite">Icon of the cured fish item.</param>
    /// <param name="cookedData">Food values of the cooked, which will automatically determine the cured values.</param>
    /// <param name="vfxFabricatingSettings">Fabricator settings. If not assigned, automatic values will be determined. Do NOT rely on those being perfect.</param>
    /// <param name="sourceAssembly">The <see cref="Assembly"/> that adds these prefabs to the game. If null, will resolve to whichever Assembly this method was called by.</param>
    public static CookedAndCuredPrefabs RegisterAllCreatureFood(CreatureAsset creature,
        string cookedName, string cookedDescription, Sprite cookedSprite,
        string curedName, string curedDescription, Sprite curedSprite,
        EdibleData cookedData, VFXFabricatingData vfxFabricatingSettings = default, Assembly sourceAssembly = null)
    {
        var creatureItemAssembly = sourceAssembly != null ? sourceAssembly : Assembly.GetCallingAssembly();
        
        var cooked = RegisterCookedFish(creature, cookedName, cookedDescription, cookedData, vfxFabricatingSettings, creatureItemAssembly).WithIcon(cookedSprite);
        var cured = RegisterCuredFish(creature, curedName, curedDescription, cookedData.foodAmount, vfxFabricatingSettings, creatureItemAssembly).WithIcon(curedSprite);

        KnownTechHandler.SetAnalysisTechEntry(
            creature.TechType,
            new TechType[] { cooked.TechType, cured.TechType },
            KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound
        );
        return new CookedAndCuredPrefabs(cooked, cured);
    }

    /// <summary>
    /// Holds both the cooked and cured version of the creature.
    /// </summary>
    public record struct CookedAndCuredPrefabs(PrefabInfo Cooked, PrefabInfo Cured);

    /// <summary>
    /// Simple method to create a cooked variant of the given creature.
    /// </summary>
    /// <param name="creature">The original uncooked creature. MUST have been registered already.</param>
    /// <param name="name">Name of the cooked fish item.</param>
    /// <param name="description">Description of the cooked fish item.</param>
    /// <param name="cookedData">Food values.</param>
    /// <param name="vfxFabricatingSettings">Fabricator settings. If not assigned, automatic values will be determined. Do NOT rely on those being perfect.</param>
    /// <param name="ownerAssembly">The <see cref="Assembly"/> that adds these prefabs to the game. If null, will resolve to whichever Assembly this method was called by.</param>
    public static PrefabInfo RegisterCookedFish(CreatureAsset creature, string name, string description, EdibleData cookedData, VFXFabricatingData vfxFabricatingSettings = default, Assembly ownerAssembly = null)
    {
        var creatureItemAssembly = ownerAssembly != null ? ownerAssembly : Assembly.GetCallingAssembly();

        if (creature.Template == null)
        {
            ECCPlugin.logger.LogError("Attempting to call RegisterCookedFish with a creature that has not been registered yet.");
            return default;
        }
        var info = PrefabInfo.WithTechType($"Cooked{creature.ClassID}", name, description, "English", false, creatureItemAssembly);
        RegisterEdibleVariant(
            info,
            creature.Template.Model,
            cookedData,
            new RecipeData(new Ingredient(creature.TechType, 1)),
            new string[] { "Survival", "CookedFood" },
#if SUBNAUTICA
            TechCategory.CookedFood,
#elif BELOWZERO
            TechCategory.FoodAndDrinks,
#endif
            vfxFabricatingSettings
        );
        CraftDataHandler.SetCookedVariant(creature.TechType, info.TechType);
        return info;
    }

    /// <summary>
    /// Simple method to create a cured variant of the given creature.
    /// </summary>
    /// <param name="creature">The original uncooked creature. MUST have been registered already.</param>
    /// <param name="name">Name of the cured fish item.</param>
    /// <param name="description">Description of the cured fish item.</param>
    /// <param name="foodValue">Food value of the food.</param>
    /// <param name="vfxFabricatingSettings">Fabricator settings. If not assigned, automatic values will be determined. Do NOT rely on those being perfect.</param>
    /// <param name="ownerAssembly">The <see cref="Assembly"/> that adds these prefabs to the game. If null, will resolve to whichever Assembly this method was called by.</param>
    public static PrefabInfo RegisterCuredFish(CreatureAsset creature, string name, string description, float foodValue, VFXFabricatingData vfxFabricatingSettings = default, Assembly ownerAssembly = null)
    {
        var creatureItemAssembly = ownerAssembly != null ? ownerAssembly : Assembly.GetCallingAssembly();

        if (creature.Template == null)
        {
            ECCPlugin.logger.LogError("Attempting to call RegisterCuredFish with a creature that has not been registered yet.");
            return default;
        }
        var info = PrefabInfo.WithTechType($"Cured{creature.ClassID}", name, description, "English", false, creatureItemAssembly);
        RegisterEdibleVariant(
            info,
            creature.Template.Model,
            new EdibleData(foodValue, -2, false),
            new RecipeData(new Ingredient(creature.TechType, 1), new Ingredient(TechType.Salt, 1)),
            new string[] { "Survival", "CuredFood" },
#if SUBNAUTICA
            TechCategory.CuredFood,
#elif BELOWZERO
            TechCategory.FoodAndDrinks,
#endif
            vfxFabricatingSettings
        );
        return info;
    }

    private class CookedCreatureTemplate : PrefabTemplate
    {
        private readonly GameObject _model;
        private readonly EdibleData _edible;
        private readonly VFXFabricatingData _vfxFabricatingData;

        public CookedCreatureTemplate(PrefabInfo info, GameObject creatureModel, EdibleData edibleData, VFXFabricatingData vfxFabricatingData) : base(info)
        {
            _model = creatureModel;
            _edible = edibleData;
            _vfxFabricatingData = vfxFabricatingData;
        }

        public override IEnumerator GetPrefabAsync(TaskResult<GameObject> gameObject)
        {
            GameObject prefab = Object.Instantiate(_model);
            prefab.SetActive(false);
            PrefabUtils.AddBasicComponents(prefab, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Near);
            CreaturePrefabUtils.AddEatable(prefab, _edible);
            CreaturePrefabUtils.AddVFXFabricating(prefab, _vfxFabricatingData);
            prefab.EnsureComponent<Pickupable>();
            var rb = prefab.EnsureComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.useGravity = false;
            rb.mass = 3;
            prefab.EnsureComponent<WorldForces>().useRigidbody = rb;
            prefab.EnsureComponent<SkyApplier>();
            prefab.EnsureComponent<EcoTarget>().type = EcoTargetType.DeadMeat;
            MaterialUtils.ApplySNShaders(prefab);
            var animator = prefab.GetComponentInChildren<Animator>();
            if (animator) animator.enabled = false;
            gameObject.Set(prefab);
            yield break;
        }
    }
}