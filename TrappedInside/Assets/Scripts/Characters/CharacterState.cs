/// <summary>
/// Cross-component state of a game character.
/// </summary>
public class CharacterState
{
    public bool isInMelee;

    public bool CanMoveHorizontally => !isInMelee;
    public bool CanJump => !isInMelee;
}
