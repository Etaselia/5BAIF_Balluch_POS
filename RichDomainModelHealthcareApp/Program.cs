using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcareApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the DbContext
builder.Services.AddDbContext<HealthcareContext>(options =>
    options.UseSqlite("Data Source=/home/eta/RiderProjects/5BAIF_Balluch_POS/RichDomainModelHealthcare/HealthcareDatabase.db"));

// Register the IdentityContext
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlite("Data Source=identity.db"));

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Seed the database with roles and an admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    
    await SeedRolesAndAdminUser(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("/Clinicians/GetClinicianCountForPatient", async (HttpContext context, [FromServices] HealthcareContext dbContext) =>
{
    var patientId = Guid.Parse(context.Request.Query["patientId"]);
    var clinicianCount = await dbContext.Appointments
        .Where(a => a.PatientId == patientId)
        .Select(a => a.ClinicianId)
        .Distinct()
        .CountAsync();

    return Results.Json(new { clinicianCount });
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

async Task SeedRolesAndAdminUser(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    var adminUser = new IdentityUser { UserName = "admin", Email = "admin@example.com" };
    var user = await userManager.FindByNameAsync(adminUser.UserName);
    if (user == null)
    {
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
