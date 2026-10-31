using System.Reflection;
using ECCLibrary.Data;

namespace ECCLibrary.Examples;

internal static class ExamplePatcher
{
    private static AssetBundle _assetBundle;
    private const string AssetBundleName = "ecc_example_bundle";
    
    public static void PatchExampleCreatures()
    {
        var exampleCreature = new ExampleCreature(PrefabInfo.WithTechType("ExampleCreature", "Example Creature", "An example of a prey fish."));
        exampleCreature.PrefabInfo.WithIcon(SpriteManager.Get(TechType.LavaBoomerang));
        exampleCreature.Register();
        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(exampleCreature, "Lifeforms/Fauna/Herbivores", "Example", "Cool lol", 6, null, null);

        var exampleClonedCreature = new ExampleClonedCreature(PrefabInfo.WithTechType("ExampleClonedCreature", "Example Cloned Creature", "Example Cloned Creature that makes me go yes."));
        exampleClonedCreature.Register();

        var exampleLeviathan = new ExampleLeviathan(PrefabInfo.WithTechType("ExampleLeviathan", "Example Leviathan", "An example of a Leviathan."));
        exampleLeviathan.Register();

        var assetBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), AssetBundleName);
        if (File.Exists(assetBundlePath))
        {
            _assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        }

        if (_assetBundle == null)
        {
            return;
        }
        
        var exampleShark = new SealedCreatureAsset(
            PrefabInfo.WithTechType("ExampleShark", "Example Shark", "An example of a Shark."),
            new CreatureTemplate(_assetBundle.LoadAsset<GameObject>("SharkPrefab"), BehaviourType.Shark, EcoTargetType.Shark, 200f)
            {
                CellLevel = LargeWorldEntity.CellLevel.Medium,
                SwimRandomData = new SwimRandomData(0.2f, 6f, new Vector3(20, 20, 20)),
                StayAtLeashData = new StayAtLeashData(0.6f, 6f, 14f),
                AvoidObstaclesData = new AvoidObstaclesData(0.5f, 6f, false, 5f, 5f),
                LocomotionData = new LocomotionData(10, 0.3f),
                BioReactorCharge = 600,
                Mass = 100,
                SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
                AnimateByVelocityData = new AnimateByVelocityData(6f),
                FleeOnDamageData = new FleeOnDamageData(0.8f, 8f),
                AggressiveWhenSeeTargetList = new List<AggressiveWhenSeeTargetData> { new(EcoTargetType.Shark, 1, 20, 1), new(EcoTargetType.SmallFish, 1, 20, 1) },
                AttackLastTargetData = new AttackLastTargetData(0.7f, 8f, 0.5f, 7f)
            }.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 1f, 1f, 1f, false, true, "ExampleShark", "ExampleShark")),
            ModifyExampleShark
        );
        if (ECCPlugin.config.EnableExampleShark)
        {
            exampleShark.CustomPrefab.SetSpawns(new SpawnLocation(new Vector3(5, -800, 500)));
        }
        exampleShark.Register();
    }

    private static IEnumerator ModifyExampleShark(GameObject obj, CreatureComponents components)
    {
        CreaturePrefabUtils.AddMeleeAttack<MeleeAttack>(obj, components,
            obj.transform.Find("MouthDamageTrigger").gameObject, true, 20);
        
        yield break;
    }
}
