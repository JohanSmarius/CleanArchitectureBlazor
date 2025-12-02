namespace UITests
{
    [TestClass]
    public class Test1 : PageTest
    {
        [TestMethod]
        public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingToTheIntroPage()
        {
            await Page.GotoAsync("https://playwright.dev");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

            // create a locator
            var getStarted = Page.Locator("text=Get Started");

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

            // Click the get started link.
            await getStarted.ClickAsync();

            // Expects the URL to contain intro.
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }

        [TestMethod]
        public async Task HomepageHasTitleAndButtonToViewDashboard()
        {
            await Page.GotoAsync("https://localhost:7096");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("Medical First Aid Manager"));

            // create a locator
            var viewDashboard = Page.Locator("text=View Dashboard");

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(viewDashboard).ToHaveAttributeAsync("href", "/dashboard");

            // Click the get started link.
            await viewDashboard.ClickAsync();

            // Expects the URL to contain intro.
            await Expect(Page).ToHaveURLAsync(new Regex(".*dashboard"));
        }
    }
}
