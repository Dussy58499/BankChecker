﻿using Microsoft.EntityFrameworkCore;
using Repository.Models.Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FullExchangeRate> FullExchangeRates { get; set; }
}
