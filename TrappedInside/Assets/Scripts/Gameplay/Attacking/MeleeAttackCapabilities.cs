using System;

[Flags]
public enum MeleeAttackCapabilities
{
    None = 0b000,
    HasShieldBlock = 0b001,
    HasShieldThrow = 0b010,
    HasShieldBash = 0b100,
}
