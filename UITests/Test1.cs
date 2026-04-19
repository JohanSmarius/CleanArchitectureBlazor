namespace UITests;

[TestClass]
public class EventFlowTests : PageTest
{
    [TestMethod]
    public async Task CanCreateAndEditEventAndSeeItInEventsList()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "https://localhost:7096";
        var uniqueSuffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var createdEventName = $"Playwright Event {uniqueSuffix}";
        var updatedEventName = $"{createdEventName} Updated";

        var tomorrow = DateTime.Today.AddDays(1);
        var startDate = tomorrow.AddHours(9);
        var endDate = tomorrow.AddHours(17);

        await Page.GotoAsync(baseUrl);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "Events" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events$"));

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Create Event" }).ClickAsync();
        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "Create New Event" })).ToBeVisibleAsync();

        await Page.GetByPlaceholder("Enter event name").FillAsync(createdEventName);
        await Page.GetByPlaceholder("Enter event location").FillAsync("Main medical station");
        await Page.Locator("input[type='datetime-local']").Nth(0).FillAsync(startDate.ToString("yyyy-MM-ddTHH:mm"));
        await Page.Locator("input[type='datetime-local']").Nth(1).FillAsync(endDate.ToString("yyyy-MM-ddTHH:mm"));
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Create Event" }).Nth(1).ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*/events/details/\\d+$"));
        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = createdEventName })).ToBeVisibleAsync();

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Edit Event" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events/edit/\\d+$"));

        await Page.GetByPlaceholder("Enter event name").FillAsync(updatedEventName);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events/details/\\d+$"));
        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = updatedEventName })).ToBeVisibleAsync();

        await Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "Events" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*/events$"));
        await Expect(Page.GetByText(updatedEventName)).ToBeVisibleAsync();
    }
}
