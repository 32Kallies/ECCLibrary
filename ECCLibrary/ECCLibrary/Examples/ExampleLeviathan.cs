using ECCLibrary.Data;

namespace ECCLibrary.Examples;

internal class ExampleLeviathan : CreatureAsset
{
    public ExampleLeviathan(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(GetModel, BehaviourType.Leviathan, EcoTargetType.Leviathan, 600f)
        {
            CellLevel = LargeWorldEntity.CellLevel.Far,
            SwimRandomData = new SwimRandomData(0.2f, 10, new Vector3(30, 10, 30), 4f, 1f, true),
            StayAtLeashData = new StayAtLeashData(0.6f, 10f, 60f),
            AvoidTerrainData = new AvoidTerrainData(0.7f, 10f, 30f, 30f),
            AcidImmune = true,
            BioReactorCharge = 4000,
            Mass = 2000,
            EyeFOV = -0.75f,
            LocomotionData = new LocomotionData(15, 0.3f),
            SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
            AnimateByVelocityData = new AnimateByVelocityData(15f),
            AttackLastTargetData = new AttackLastTargetData(0.8f, 12f, 0.5f, 10f),
            AttackCyclopsData = new AttackCyclopsData(1f, 15f, 100f, 0.4f, 3f, 0.01f)
        };

        template.AddAggressiveWhenSeeTargetData(new AggressiveWhenSeeTargetData(EcoTargetType.Shark, 2, 150, 3));

        template.BehaviourLODData = new BehaviourLODData(50, 250, 500);

        return template;
    }

    public static GameObject GetModel()
    {
        var model = new GameObject("CreatureModel");
        model.SetActive(false);

        var worldModel = new GameObject("Model");

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = worldModel.transform;
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = worldModel.transform;
        sphere.transform.localPosition = Vector3.forward * 0.5f;

        worldModel.transform.parent = model.transform;
        worldModel.AddComponent<Animator>();
        worldModel.transform.localScale = Vector3.one * 10f;

        foreach (var col in model.GetComponentsInChildren<Collider>(true))
        {
            Object.DestroyImmediate(col);
        }

        model.gameObject.AddComponent<BoxCollider>().size = Vector3.one * 10;

        GameObject tailRoot = new GameObject("Tail");
        tailRoot.transform.parent = worldModel.transform;
        tailRoot.transform.localScale = Vector3.one;

        Transform parent = tailRoot.transform;

        for (int i = 0; i < 20; i++)
        {
            var tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tail.name = "TailSegment_phys";
            Object.DestroyImmediate(tail.GetComponent<Collider>());
            tail.transform.parent = parent;
            tail.transform.localPosition = Vector3.forward * (-0.4f * (Mathf.Log(i + 2)));
            tail.transform.localScale = Vector3.one * 0.9f;
            parent = tail.transform;
        }

        GameObject mouth = new GameObject("Mouth");
        mouth.transform.parent = sphere.transform;
        mouth.transform.ZeroTransform();
        mouth.AddComponent<SphereCollider>().isTrigger = true;
        
        Object.DontDestroyOnLoad(model);

        return model;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        var trailManagerBuilder = new TrailManagerBuilder(components, prefab.transform.SearchChild("Tail"));
        trailManagerBuilder.SegmentSnapSpeed = 4;
        trailManagerBuilder.SetTrailArrayToPhysBoneChildren();
        trailManagerBuilder.AllowDisableOnScreen = false;
        trailManagerBuilder.Apply();

        var mouth = prefab.SearchChild("Mouth");

        CreaturePrefabUtils.AddMeleeAttack<LeviathanMeleeAttack>(prefab, components, mouth, true, 40f);

        yield break;
    }
}