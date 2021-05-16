
using System;

public interface ITIInputContext : IDisposable
{
    /// <summary>
    /// Returns the current input state. By calling this method exactly once
    /// every update you'll get correct values for the event flags in
    /// <see cref="TIInputState"/> so that even keypresses that started and
    /// stopped between two consecutive updates will be there.
    /// </summary>
    TIInputState GetStateAndResetEventFlags();
}
