namespace ECCLibrary.Data;

/// <summary>
/// Contains data pertaining to picking up and/or holding fish in your hands.
/// </summary>
public class PickupableFishData
{
    /// <summary>
    /// Can this fish be held in the hotbar?
    /// </summary>
    public bool CanBeHeld;

    /// <summary>
    /// If can be held: The TechType that is used to find the holding animation.
    /// </summary>
    public TechType ReferenceHoldingAnimation;

    /// <summary>
    /// If can be held: The name of the model used for the World View, which must be a child of the object.
    /// </summary>
    public string WorldModelName;

    /// <summary>
    /// If can be held: The name of the model used for the First Person View, which must be a child of the object.
    /// </summary>
    public string ViewModelName;

    /// <summary>
    /// If <c>true</c>, this pickupable creature will be allowed to be grabbed up by the Grav Trap. 
    /// </summary>
    public bool AffectedByGravTrap { get; set; } = true;

    /// <summary>
    /// This constructor overload creates a <see cref="PickupableFishData"/> instance for creatures that can be picked up but NOT held.
    /// </summary>
    public PickupableFishData() { }

    /// <summary>
    /// This constructor overload creates a <see cref="PickupableFishData"/> instance for creatures that can be picked up AND held.
    /// </summary>
    /// <param name="referenceHoldingAnimation">If can be held: The TechType that is used to find the holding animation.</param>
    /// <param name="worldModelName">If can be held: The name of the model used for the World View, which must be a child of the object.</param>
    /// <param name="viewModelName">If can be held: The name of the model used for the First Person View, which must be a child of the object.</param>
    public PickupableFishData(TechType referenceHoldingAnimation, string worldModelName, string viewModelName)
    {
        CanBeHeld = true;
        ReferenceHoldingAnimation = referenceHoldingAnimation;
        WorldModelName = worldModelName;
        ViewModelName = viewModelName;
    }
}