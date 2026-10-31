using ECCLibrary.Data;

namespace ECCLibrary.Examples;

// This file shows one method of creature creation, which involves making a new class that inherits from the CreatureAsset class
internal class ExampleCreature : CreatureAsset
{
    public ExampleCreature(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    // This is the most essential part of creating your creature. This is where you set all required and optional properties.
    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(GetModel, BehaviourType.SmallFish, EcoTargetType.SmallFish, 160f);
        CreatureTemplateUtils.SetCreatureDataEssentials(template, LargeWorldEntity.CellLevel.Medium, 50f);
        CreatureTemplateUtils.SetCreatureMotionEssentials(template, new SwimRandomData(0.2f, 3f, new Vector3(20, 5, 20)), new StayAtLeashData(0.6f, 6f, 14f));
        CreatureTemplateUtils.SetPreyEssentials(template, 5f, new PickupableFishData(TechType.Peeper, "WM", "VM"), new EdibleData(69f, 1337f, false, 2f));
        template.SetCreatureComponentType<ExampleCreatureComponent>();
        template.AvoidObstaclesData = new AvoidObstaclesData(1f, 3f, false, 5f, 5f);
        template.SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f));
        template.AnimateByVelocityData = new AnimateByVelocityData(6f);
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));
        template.SwimInSchoolData = new SwimInSchoolData(0.5f, 3f, 2f, 0.5f, 1f, 0.1f, 25f);
        return template;
    }

    // You can ignore the entire GetModel function if you're using a custom creature model.
    // All this part of the code does is generate a model out of basic Unity shapes for the sake of the tutorial.
    // For actual mods I would instead recommend using a GameObject loaded directly from an Asset Bundle, and applying any changes in ModifyPrefab.
    public static GameObject GetModel()
    {
        var model = new GameObject("CreatureModel");
        model.SetActive(false);

        var worldModel = new GameObject("WM");

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = worldModel.transform;
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = worldModel.transform;
        sphere.transform.localPosition = Vector3.forward * 0.5f;

        var viewModel = Object.Instantiate(worldModel);
        viewModel.name = "VM";

        worldModel.transform.parent = model.transform;
        viewModel.transform.parent = model.transform;

        viewModel.SetActive(false);
        viewModel.transform.localScale = Vector3.one * 0.2f;

        worldModel.AddComponent<Animator>();
        viewModel.AddComponent<Animator>();

        foreach (var col in model.GetComponentsInChildren<Collider>(true))
        {
            Object.DestroyImmediate(col);
        }

        model.gameObject.AddComponent<SphereCollider>();

        Object.DontDestroyOnLoad(model);

        return model;
    }

    // This override is optional, but allows you to apply custom behavior onto your creature *object*. In this example I make it immune to heat damage. 
    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Heat, 0f);
        yield break;
    }

    // This override is useful for registering custom spawns. I have them commented out as to not disrupt gameplay for users
    protected override void PostRegister()
    {
        /* LootDistributionHandler.AddLootDistributionData(PrefabInfo.ClassID,
            new LootDistributionData.BiomeData()
            {
                biome = BiomeType.SafeShallows_Grass,
                count = 2,
                probability = 0.1f
            },
            new LootDistributionData.BiomeData()
            {
                biome = BiomeType.JellyshroomCaves_CaveSand,
                count = 6,
                probability = 0.4f
            }
            );
        */
    }
}

// It's optional but oftentimes nice to make a new class for your creature instances.
internal class ExampleCreatureComponent : Creature
{
    public override void Start()
    {
        base.Start();
        ErrorMessage.AddMessage("I'm an example creature!");
    }
}