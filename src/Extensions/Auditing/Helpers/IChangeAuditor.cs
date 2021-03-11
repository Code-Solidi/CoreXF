using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;

namespace Auditing.Helpers
{
    /// <summary>
    /// The change auditor.
    /// </summary>
    public interface IChangeAuditor
    {
        /// <summary>
        /// Gets the save changes handler.
        /// </summary>
        EventHandler<SavingChangesEventArgs> SavingChanges { get; }

        /// <summary>
        /// Gets or sets the change tracker.
        /// </summary>
        ChangeTracker ChangeTracker { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        string User { set; }
    }
}