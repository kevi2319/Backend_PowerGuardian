using Backend_PowerGuardian.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend_PowerGuardian.Seed;   // ⬅️ carpeta + nombre del proyecto
                                        //    (VS lo autocompleta si la carpeta se llama igual)

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "User" };

        // Crear roles si no existen
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Crear usuario administrador si no existe
        const string adminEmail = "admin@powerguardian.com";
        const string adminPass = "Passw0rd!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                Nombres = "Administrador",
                ApellidoPaterno = "Sistema",
                ApellidoMaterno = "Central",             // Usa "" si es nullable
                FechaNacimiento = new DateTime(1990, 1, 1),
                Pais = "México",
                PhoneNumber = "0000000000"
            };

            await userManager.CreateAsync(admin, adminPass);
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
