using Microsoft.EntityFrameworkCore;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Menus.AnyAsync()) return;

        // Categories
        var bebidas    = new Category { Name = "Bebidas",    DisplayOrder = 1 };
        var cervezas   = new Category { Name = "Cervezas",   DisplayOrder = 2 };
        var tragos     = new Category { Name = "Tragos",     DisplayOrder = 3 };
        var picadas    = new Category { Name = "Picadas",    DisplayOrder = 4 };
        var sandwiches = new Category { Name = "Sándwiches", DisplayOrder = 5 };
        db.Categories.AddRange(bebidas, cervezas, tragos, picadas, sandwiches);

        // Product master catalog
        var agua       = new Product { Name = "Agua mineral" };
        var aguaGas    = new Product { Name = "Agua con gas" };
        var gaseosa    = new Product { Name = "Gaseosa 350ml",            Description = "Coca-Cola, Sprite o Fanta" };
        var jugo       = new Product { Name = "Jugo de naranja natural" };
        var cerPorron  = new Product { Name = "Cerveza porrón",           Description = "330ml" };
        var cerLitron  = new Product { Name = "Cerveza litrón",           Description = "1 litro" };
        var cerRubia   = new Product { Name = "Cerveza artesanal rubia",  Description = "500ml" };
        var cerNegra   = new Product { Name = "Cerveza artesanal negra",  Description = "500ml" };
        var fernet     = new Product { Name = "Fernet con Coca" };
        var ginTonic   = new Product { Name = "Gin tónic",                Description = "Gin + tónica + rodaja de limón" };
        var aperol     = new Product { Name = "Aperol Spritz" };
        var campari    = new Product { Name = "Campari orange" };
        var mojito     = new Product { Name = "Mojito" };
        var picChica   = new Product { Name = "Picada chica",             Description = "Fiambres, quesos y aceitunas" };
        var picGrande  = new Product { Name = "Picada grande",            Description = "Fiambres, quesos, aceitunas y tostadas" };
        var papas      = new Product { Name = "Papas fritas" };
        var tostadas   = new Product { Name = "Tostadas con queso" };
        var lomito     = new Product { Name = "Lomito clásico",           Description = "Lomo, lechuga, tomate y mayonesa" };
        var lomitoComp = new Product { Name = "Lomito completo",          Description = "Lomo, jamón, queso, huevo, lechuga y tomate" };
        var hambSimple = new Product { Name = "Hamburguesa simple",       Description = "Medallón, lechuga, tomate y ketchup" };
        var hambDoble  = new Product { Name = "Hamburguesa doble",        Description = "Doble medallón, cheddar, panceta y bbq" };
        db.Products.AddRange(agua, aguaGas, gaseosa, jugo,
            cerPorron, cerLitron, cerRubia, cerNegra,
            fernet, ginTonic, aperol, campari, mojito,
            picChica, picGrande, papas, tostadas,
            lomito, lomitoComp, hambSimple, hambDoble);

        // Modifier groups
        var coccionGroup = new ModifierGroup
        {
            Name = "Cocción", MinSelections = 1, MaxSelections = 1, IsRequired = true,
            Product = hambSimple
        };
        var coccionGroupDoble = new ModifierGroup
        {
            Name = "Cocción", MinSelections = 1, MaxSelections = 1, IsRequired = true,
            Product = hambDoble
        };
        var extrasGroup = new ModifierGroup
        {
            Name = "Extras", MinSelections = 0, MaxSelections = 3, IsRequired = false,
            Category = sandwiches
        };
        db.ModifierGroups.AddRange(coccionGroup, coccionGroupDoble, extrasGroup);

        var coccionOpts = new[]
        {
            new Modifier { Name = "A punto",        ModifierGroup = coccionGroup },
            new Modifier { Name = "Bien cocida",    ModifierGroup = coccionGroup },
            new Modifier { Name = "Jugosa",         ModifierGroup = coccionGroup },
            new Modifier { Name = "A punto",        ModifierGroup = coccionGroupDoble },
            new Modifier { Name = "Bien cocida",    ModifierGroup = coccionGroupDoble },
            new Modifier { Name = "Jugosa",         ModifierGroup = coccionGroupDoble },
        };
        var queso  = new Modifier { Name = "Queso extra", ModifierGroup = extrasGroup };
        var huevo  = new Modifier { Name = "Huevo frito", ModifierGroup = extrasGroup };
        var panceta = new Modifier { Name = "Panceta",   ModifierGroup = extrasGroup };
        var cheddar = new Modifier { Name = "Cheddar",   ModifierGroup = extrasGroup };
        db.Modifiers.AddRange(coccionOpts);
        db.Modifiers.AddRange(queso, huevo, panceta, cheddar);

        // Menus
        var salon  = new Menu { Name = "Salón",  IsActive = true };
        var vereda = new Menu { Name = "Vereda", IsActive = true };
        db.Menus.AddRange(salon, vereda);

        // MenuProducts — Salón: full menu (sandwiches named for MenuModifier references)
        var mpLomito     = new MenuProduct { Menu = salon, Product = lomito,     Category = sandwiches, Price = 2800 };
        var mpLomitoComp = new MenuProduct { Menu = salon, Product = lomitoComp, Category = sandwiches, Price = 3500 };
        var mpHambSimple = new MenuProduct { Menu = salon, Product = hambSimple, Category = sandwiches, Price = 2500 };
        var mpHambDoble  = new MenuProduct { Menu = salon, Product = hambDoble,  Category = sandwiches, Price = 3800 };

        db.MenuProducts.AddRange(
            new MenuProduct { Menu = salon, Product = agua,      Category = bebidas,  Price = 500 },
            new MenuProduct { Menu = salon, Product = aguaGas,   Category = bebidas,  Price = 500 },
            new MenuProduct { Menu = salon, Product = gaseosa,   Category = bebidas,  Price = 700 },
            new MenuProduct { Menu = salon, Product = jugo,      Category = bebidas,  Price = 900 },
            new MenuProduct { Menu = salon, Product = cerPorron, Category = cervezas, Price = 1200 },
            new MenuProduct { Menu = salon, Product = cerLitron, Category = cervezas, Price = 2200 },
            new MenuProduct { Menu = salon, Product = cerRubia,  Category = cervezas, Price = 1800 },
            new MenuProduct { Menu = salon, Product = cerNegra,  Category = cervezas, Price = 1900 },
            new MenuProduct { Menu = salon, Product = fernet,    Category = tragos,   Price = 1500 },
            new MenuProduct { Menu = salon, Product = ginTonic,  Category = tragos,   Price = 1800 },
            new MenuProduct { Menu = salon, Product = aperol,    Category = tragos,   Price = 2000 },
            new MenuProduct { Menu = salon, Product = campari,   Category = tragos,   Price = 1700 },
            new MenuProduct { Menu = salon, Product = mojito,    Category = tragos,   Price = 2100 },
            new MenuProduct { Menu = salon, Product = picChica,  Category = picadas,  Price = 3500 },
            new MenuProduct { Menu = salon, Product = picGrande, Category = picadas,  Price = 5500 },
            new MenuProduct { Menu = salon, Product = papas,     Category = picadas,  Price = 1200 },
            new MenuProduct { Menu = salon, Product = tostadas,  Category = picadas,  Price = 900 },
            mpLomito, mpLomitoComp, mpHambSimple, mpHambDoble
        );

        // MenuProducts — Vereda: only drinks and snacks, slightly higher price
        db.MenuProducts.AddRange(
            new MenuProduct { Menu = vereda, Product = agua,      Category = bebidas,  Price = 550 },
            new MenuProduct { Menu = vereda, Product = aguaGas,   Category = bebidas,  Price = 550 },
            new MenuProduct { Menu = vereda, Product = gaseosa,   Category = bebidas,  Price = 750 },
            new MenuProduct { Menu = vereda, Product = cerPorron, Category = cervezas, Price = 1300 },
            new MenuProduct { Menu = vereda, Product = cerLitron, Category = cervezas, Price = 2300 },
            new MenuProduct { Menu = vereda, Product = fernet,    Category = tragos,   Price = 1600 },
            new MenuProduct { Menu = vereda, Product = aperol,    Category = tragos,   Price = 2100 },
            new MenuProduct { Menu = vereda, Product = papas,     Category = picadas,  Price = 1300 }
        );

        // MenuModifiers — cocción (hambSimple y hambDoble, sin costo extra)
        var (apt1, bien1, jug1, apt2, bien2, jug2) = (
            coccionOpts[0], coccionOpts[1], coccionOpts[2],
            coccionOpts[3], coccionOpts[4], coccionOpts[5]);

        db.MenuModifiers.AddRange(
            new MenuModifier { MenuProduct = mpHambSimple, Modifier = apt1,  ExtraPrice = 0 },
            new MenuModifier { MenuProduct = mpHambSimple, Modifier = bien1, ExtraPrice = 0 },
            new MenuModifier { MenuProduct = mpHambSimple, Modifier = jug1,  ExtraPrice = 0 },
            new MenuModifier { MenuProduct = mpHambDoble,  Modifier = apt2,  ExtraPrice = 0 },
            new MenuModifier { MenuProduct = mpHambDoble,  Modifier = bien2, ExtraPrice = 0 },
            new MenuModifier { MenuProduct = mpHambDoble,  Modifier = jug2,  ExtraPrice = 0 }
        );

        // MenuModifiers — extras (todos los sándwiches, con precio)
        foreach (var mp in new[] { mpLomito, mpLomitoComp, mpHambSimple, mpHambDoble })
        {
            db.MenuModifiers.AddRange(
                new MenuModifier { MenuProduct = mp, Modifier = queso,  ExtraPrice = 300 },
                new MenuModifier { MenuProduct = mp, Modifier = huevo,  ExtraPrice = 400 },
                new MenuModifier { MenuProduct = mp, Modifier = panceta, ExtraPrice = 500 },
                new MenuModifier { MenuProduct = mp, Modifier = cheddar, ExtraPrice = 350 }
            );
        }

        // Tables — 1-5 Salón, 6-10 Vereda
        if (!await db.Tables.AnyAsync())
        {
            db.Tables.AddRange(
                Enumerable.Range(1, 5).Select(i => new Table { Number = i.ToString(), IsActive = true, Menu = salon })
            );
            db.Tables.AddRange(
                Enumerable.Range(6, 5).Select(i => new Table { Number = i.ToString(), IsActive = true, Menu = vereda })
            );
        }

        await db.SaveChangesAsync();
    }
}
