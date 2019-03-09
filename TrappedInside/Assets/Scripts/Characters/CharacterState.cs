/// <summary>
/// Cross-component state of a game character.
/// </summary>
public struct CharacterState
{
    public bool isInMelee;

    public bool CanMoveHorizontally => !isInMelee;
    public bool CanJump => !isInMelee;
}
