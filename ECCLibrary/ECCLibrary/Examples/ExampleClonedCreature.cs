using ECCLibrary.Data;

namespace ECCLibrary.Examples;

internal class ExampleClonedCreature : CreatureAsset
{
    public ExampleClonedCreature(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureTemplate(TechType.Boomerang, 420);
        CreatureTemplateUtils.SetPreyEssentials(template, 4.20f,
            new PickupableFishData(TechType.Boomerang, null, null),
            new EdibleData(6f, 9f, false));

        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>())
        {
            foreach (var material in renderer.materials)
            {
                material.color = Color.red;
                material.SetColor("_SpecColor", Color.red);
            }
        }
        
        yield break;
    }
}