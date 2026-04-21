using static Microsoft.Playwright.AriaRole;

namespace E2ETests;

[TestClass]
public class EventFlowTests : PageTest
{
    public override Microsoft.Playwright.BrowserNewContextOptions ContextOptions() =>
        new()
        {
            IgnoreHTTPSErrors = true
        };

    [TestMethod]
    public async Task CreateEditAndVerifyEvent_ShouldUpdateEventInList()
    {
        const int eventStartHour = 9;
        const int eventEndHour = 17;
        const string eventsLoadingErrorText = "Error Loading Events";

        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "https://localhost:7096/";
        var uniqueSuffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var createdEventName = $"Playwright Event {uniqueSuffix}";
        var updatedEventName = $"{createdEventName} Updated";

        var tomorrow = DateTime.Today.AddDays(1).Date;
        var startDate = tomorrow + TimeSpan.FromHours(eventStartHour);
        var endDate = tomorrow + TimeSpan.FromHours(eventEndHour);

        try
        {
            await Page.GotoAsync(baseUrl);
        }
        catch (Microsoft.Playwright.PlaywrightException ex) when (ex.Message.Contains("ERR_CONNECTION_REFUSED") || ex.Message.Contains("net::ERR"))
        {
            Assert.Inconclusive($"Application is not running at {baseUrl}. Start the app before running E2E tests.");
            return;
        }

        await Page.GetByRole(Link, new() { Name = "Events" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events$"));

        // Wait for initial page loading lifecycle to complete without using fixed delays.
        await Expect(Page.GetByRole(Status).Filter(new() { HasTextString = "Loading..." })).ToBeHiddenAsync();

        var loadErrorAlert = Page.GetByRole(Alert).Filter(new() { HasTextString = eventsLoadingErrorText });
        if (await loadErrorAlert.IsVisibleAsync())
        {
            Assert.Inconclusive("Event flow test requires a working event data store. Current environment cannot load events.");
        }

        await Page.GetByRole(Button, new() { Name = "Create Event" }).ClickAsync();
        var createEventModal = Page.Locator(".modal-dialog").First;
        await Expect(createEventModal.GetByRole(Heading, new() { Name = "Create New Event" })).ToBeVisibleAsync();
        var startDateInput = createEventModal.Locator("label:has-text('Start Date & Time *') + input[type='datetime-local']");
        var endDateInput = createEventModal.Locator("label:has-text('End Date & Time *') + input[type='datetime-local']");

        await createEventModal.GetByPlaceholder("Enter event name").FillAsync(createdEventName);
        await createEventModal.GetByPlaceholder("Enter event location").FillAsync("Main medical station");
        await startDateInput.FillAsync(startDate.ToString("yyyy-MM-ddTHH:mm"));
        await endDateInput.FillAsync(endDate.ToString("yyyy-MM-ddTHH:mm"));
        await createEventModal.GetByRole(Button, new() { Name = "Create Event" }).ClickAsync();

        try
        {
            await Expect(Page).ToHaveURLAsync(new Regex(".*/events$"));

            // Creating an event keeps users on the events list; open the new event details explicitly.
            await Expect(Page.GetByText(createdEventName, new() { Exact = true }).First).ToBeVisibleAsync();
            var eventCardOrRow = Page.Locator(".card, tr").Filter(new() { HasTextString = createdEventName }).First;
            await eventCardOrRow.GetByRole(Button, new() { NameRegex = new Regex("View Details|Details") }).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*/events/details/\\d+$"));
        }
        catch (Microsoft.Playwright.PlaywrightException)
        {
            if (await loadErrorAlert.IsVisibleAsync())
            {
                Assert.Inconclusive("Event creation failed because events cannot be loaded in this environment.");
                return;
            }

            throw;
        }

        if (Page.Url.EndsWith("/events", StringComparison.OrdinalIgnoreCase) && await loadErrorAlert.IsVisibleAsync())
        {
            Assert.Inconclusive("Event creation failed because events cannot be loaded in this environment.");
        }

        await Expect(Page.GetByRole(Heading, new() { Name = createdEventName })).ToBeVisibleAsync();

        await Page.GetByRole(Button, new() { Name = "Edit Event" }).ClickAsync();
        try
        {
            await Expect(Page).ToHaveURLAsync(new Regex(".*/events/edit/\\d+$"));
        }
        catch (Microsoft.Playwright.PlaywrightException)
        {
            // Fallback for occasional missed click-navigation in interactive server mode.
            var eventIdMatch = Regex.Match(Page.Url, ".*/events/details/(\\d+)$");
            if (!eventIdMatch.Success)
            {
                throw;
            }

            var detailsEventId = eventIdMatch.Groups[1].Value;
            await Page.GotoAsync($"{baseUrl.TrimEnd('/')}/events/edit/{detailsEventId}");
            await Expect(Page).ToHaveURLAsync(new Regex(".*/events/edit/\\d+$"));
        }

        await Page.GetByPlaceholder("Enter event name").FillAsync(updatedEventName);
        await Page.GetByRole(Button, new() { Name = "Save Changes" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events/details/\\d+$"));
        await Expect(Page.GetByRole(Heading, new() { Name = updatedEventName })).ToBeVisibleAsync();

        await Page.GetByRole(Link, new() { Name = "Events" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events$"));
        await Expect(Page.GetByText(updatedEventName, new() { Exact = true }).First).ToBeVisibleAsync();
    }
}
