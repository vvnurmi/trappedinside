using UnityEngine;

/// <summary>
/// Cross-component state of a game character.
/// </summary>
public class CharacterState
{
    public bool isInMelee;

    public CollisionInfo collisions = new CollisionInfo();

    public bool CanMoveHorizontally => !isInMelee;
    public bool CanJump => !isInMelee;
}

/// <summary>
/// State of collisions of a game character.
/// </summary>
public class CollisionInfo
{
    public bool above;
    public bool below;
    public bool left;
    public bool right;

    public Vector2 moveAmountOld;
    /// <summary>
    /// 1 = right, -1 = left
    /// </summary>
    public int faceDir = 1;

    public bool HasHorizontalCollisions => left || right;
    public bool HasVerticalCollisions => above || below;

    public void Reset()
    {
        above = false;
        below = false;
        left = false;
        right = false;
    }
}
