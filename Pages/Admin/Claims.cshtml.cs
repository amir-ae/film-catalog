namespace FilmCatalog.Pages.Admin
{
    public class ClaimsModel : AdminPageModel
    {
        public ClaimsModel(UserManager<IdentityUser> mgr)
            => UserManager = mgr;

        public UserManager<IdentityUser> UserManager { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }

        public IEnumerable<Claim>? Claims { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("Selectuser",
                    new { Label = "Manage Claims", Callback = "Claims" });
            }
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            Claims = await UserManager.GetClaimsAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Required] string task,
            [Required] string type, [Required] string value, string? oldValue)
        {
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            if (user != null)
            {
                Claims = await UserManager.GetClaimsAsync(user);
                if (ModelState.IsValid)
                {
                    Claim claim = new Claim(type, value);
                    IdentityResult result = IdentityResult.Success;
                    switch (task)
                    {
                        case "add":
                            result = await UserManager.AddClaimAsync(user, claim);
                            break;
                        case "delete":
                            result = await UserManager.RemoveClaimAsync(user, claim);
                            break;
                        case "change":
                            if (!string.IsNullOrEmpty(oldValue))
                            {
                                result = await UserManager.ReplaceClaimAsync(user,
                                    new Claim(type, oldValue), claim);
                            }
                            break;
                    };
                    if (result.Process(ModelState))
                    {
                        return RedirectToPage(new { id = Id });
                    }
                }
            }
            return Page();
        }
    }
}
