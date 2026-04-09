using RVM.MenuNaMao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RVM.MenuNaMao.Tests.Helpers;

public static class TestDbContext
{
    public static MenuNaMaoDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MenuNaMaoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new MenuNaMaoDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }
}
