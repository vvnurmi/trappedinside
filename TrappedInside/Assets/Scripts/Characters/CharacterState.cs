﻿using UnityEngine;

/// <summary>
/// Cross-component state of a game character.
/// </summary>
public class CharacterState : MonoBehaviour
{
    public bool isDead;

    /// <summary>
    /// Is the character's horizontal movement governed by an attack.
    /// </summary>
    public bool isInHorizontalAttackMove;

    /// <summary>
    /// Is the character's vertical movement governed by an attack.
    /// </summary>
    public bool isInVerticalAttackMove;

    public bool canClimb;
    public bool isClimbing;
    public bool isStuck;

    public CollisionInfo collisions = new CollisionInfo();

    public int collectedBusinessCards;
    public int collectedArcadeTokens;

    public bool CanMoveHorizontally => !isDead && !isStuck && !isInHorizontalAttackMove;
    public bool CanJump => !isDead && !isStuck && !isInVerticalAttackMove && (collisions.below || isClimbing);
    public bool CanChangeDirection => !isDead;
    public bool CanInflictDamage => !isDead;
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
