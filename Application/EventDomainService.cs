using System;
using System.Collections.Generic;
using System.Text;
using Entities;

namespace Application
{
    /// <summary>
    /// Pure domain service that contains business rules for Events.
    /// It decides which notifications should be sent and applies incoming changes
    /// without touching infrastructure (EF, email, logging).
    /// </summary>
    public sealed class EventDomainService
    {
        /// <summary>
        /// Applies incoming values to the existing event aggregate and returns a decision
        /// describing which side-effects (emails) the infrastructure layer should attempt.
        /// Database persistence and side-effects are intentionally left outside this method.
        /// </summary>
        /// <param name="existing">Tracked entity loaded from the database.</param>
        /// <param name="updated">DTO/DETACHED instance containing the new values from the caller.</param>
        /// <returns>Decision information for the repository/service layer.</returns>
        public EventStatusChangeDecision ApplyChanges(Event existing, Event updated)
        {
            if (existing.Id != updated.Id)
                throw new InvalidOperationException("Mismatched Event ids.");

            // Capture original values needed for rules
            var originalStatus = existing.Status;
            var originalNotificationSent = existing.NotificationSent;

            // Copy over mutable fields (avoid overwriting identity / audit)
            existing.Name = updated.Name;
            existing.StartDate = updated.StartDate;
            existing.EndDate = updated.EndDate;
            existing.Location = updated.Location;
            existing.Description = updated.Description;
            existing.Status = updated.Status; // may be further changed after notification
            existing.ContactPerson = updated.ContactPerson;
            existing.ContactPhone = updated.ContactPhone;
            existing.ContactEmail = updated.ContactEmail;
            existing.UpdatedAt = DateTime.UtcNow;

            bool canContact = !string.IsNullOrWhiteSpace(existing.ContactEmail);

            bool shouldSendPlanned =
                originalStatus != EventStatus.Planned &&
                updated.Status == EventStatus.Planned &&
                !originalNotificationSent &&
                canContact;

            bool shouldSendInvoice =
                originalStatus != EventStatus.SendInvoice &&
                updated.Status == EventStatus.SendInvoice &&
                canContact;

            return new EventStatusChangeDecision(
                ShouldSendPlannedNotification: shouldSendPlanned,
                PromoteToConfirmedAfterPlanned: shouldSendPlanned, // business rule: auto confirm after planned email
                ShouldSendInvoiceNotification: shouldSendInvoice
            );
        }
    }

    /// <summary>
    /// Value object describing side-effects the infrastructure layer should perform
    /// after domain state changes were applied.
    /// </summary>
    public readonly record struct EventStatusChangeDecision(
        bool ShouldSendPlannedNotification,
        bool PromoteToConfirmedAfterPlanned,
        bool ShouldSendInvoiceNotification
    );
}
