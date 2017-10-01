using System;

namespace BreadPlayer.StateTriggers
{
    /// <summary>
    /// Implement this interface to be able to observe triggers or use them
    /// in a <see cref="CompositeStateTrigger"/>.
    /// </summary>
    public interface ITriggerValue
    {
        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        /// Occurs when the <see cref="IsActive"/> property has changed.
        /// </summary>
        event EventHandler IsActiveChanged;
    }
}