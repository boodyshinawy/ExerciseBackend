namespace Project.Api;

public static class DataSeeder
{
    public const string SeedFirmCode = "10";
    public const string AdminRoleId = "1";
    public const string DefaultAdminUserName = "admin1";
    public const string DefaultAdminPassword = "Admin@123";

    public static async Task SeedAsync(ApplicationDbContext context, IUserService userService)
    {
        await SeedTenUnitsAsync(context);
        await SeedRolesAsync(context);
        await SeedPagesAsync(context);
        await SeedAdditionalPagesAsync(context);
        await UpdateGiftPagePathsToSeizureAsync(context);
        await SeedAdminUserAsync(context, userService);
        await SeedRanksAsync(context);
        await SeedStatusesAsync(context);
        await SeedUnitsOfMeasureAsync(context);
        await SeedSaleEntitiesAsync(context);
        await SeedPageRolesAsync(context);
    }


    /// <summary>
    /// Updates already-seeded menu routes from /gift-* to /seizure-* without disrupting other page data.
    /// </summary>
    public static async Task UpdateGiftPagePathsToSeizureAsync(ApplicationDbContext context)
    {
        var pathMap = new Dictionary<string, string>
        {
            ["/gift-request"] = "/seizure-request",
            ["/gift-sub-committee"] = "/seizure-sub-committee",
            ["/gift-permanent-committee"] = "/seizure-permanent-committee",
            ["/gift-sales-entity"] = "/seizure-sales-entity",
            ["/gift-finance-authority"] = "/seizure-finance-authority",
            ["/gift-completed"] = "/seizure-completed",
            ["/gift-detail"] = "/seizure-detail"
        };

        var pages = await context.Set<Page>()
            .Where(p => p.To != null && p.To.StartsWith("/gift-"))
            .ToListAsync();

        if (pages.Count == 0)
            return;

        foreach (var page in pages)
        {
            if (pathMap.TryGetValue(page.To!, out var newPath))
                page.To = newPath;
            else if (page.To!.StartsWith("/gift-", StringComparison.Ordinal))
                page.To = "/seizure-" + page.To.Substring("/gift-".Length);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedStatusesAsync(ApplicationDbContext context)
    {
        var statuses = new (int Id, string Name)[]
        {
            ((int)SeizureStatus.Registration, "قيد التسجيل"),
            ((int)SeizureStatus.SubCommittee, "اللجنة الفرعية"),
            ((int)SeizureStatus.PermanentCommittee, "اللجنة الدائمة"),
            ((int)SeizureStatus.SalesEntity, "جهة البيع"),
            ((int)SeizureStatus.ServicesAgency, "جهاز الخدمات العامة"),
            ((int)SeizureStatus.PartiallyAllocated, "مخصص جزئياً"),
            ((int)SeizureStatus.FullyAllocated, "مخصص بالكامل"),
            ((int)SeizureStatus.FinanceAuthority, "هيئة شئون مالية"),
            ((int)SeizureStatus.Completed, "مكتمل")
        };

        foreach (var (id, name) in statuses)
        {
            var exists = await context.Set<Status>().AnyAsync(s => s.Id == id);
            if (!exists)
            {
                await context.Database.ExecuteSqlInterpolatedAsync($@"
                    INSERT INTO status (id, name, is_deleted, time_stamp)
                    OVERRIDING SYSTEM VALUE
                    VALUES ({id}, {name}, false, NOW())");
            }
            else
            {
                var existing = await context.Set<Status>().FirstOrDefaultAsync(s => s.Id == id);
                if (existing != null && existing.Name != name)
                {
                    existing.Name = name;
                }
            }
        }
        await context.SaveChangesAsync();

        await context.Database.ExecuteSqlRawAsync(@"
            SELECT setval(
                pg_get_serial_sequence('status', 'id'),
                COALESCE((SELECT MAX(id) FROM status), 1))");
    }

    private static async Task SeedTenUnitsAsync(ApplicationDbContext context)
    {
        var units = new (string Code, string Name, short Order)[]
        {
            ("1", "رئاسة مكاتب مخابرات وامن حرس الحدود الغربي (مطروح)", 1),
            ("2", "رئاسة مكاتب مخابرات وامن حرس الحدود الجنوبي (أسوان)", 2),
            ("3", "رئاسة مكاتب مخابرات وامن حرس الحدود البحر الأحمر (الغردقة)", 3),
            ("4", "رئاسة مكاتب مخابرات وامن حرس الحدود السويس وجنوب سيناء (السويس)", 4),
            ("5", "رئاسة مكاتب مخابرات وامن حرس الحدود الإسماعيلية وشمال سيناء (الإسماعيلية)", 5),
            ("6", "رئاسة مكاتب مخابرات وامن حرس الحدود الشمالي (الاسكندرية)", 6),
            ("7", "اللجنة الدائمة", 7),
            ("8", "جهاز الخدمات العامة", 8),
            ("9", "هيئة الشئون المالية", 9),
            ("10", "قيادة قوات حرس الحدود (Admin)", 10)
        };

        foreach (var unit in units)
        {
            var firm = await context.Set<Firm>().FirstOrDefaultAsync(f => f.Id == unit.Code);
            if (firm == null)
            {
                context.Set<Firm>().Add(new Firm
                {
                    Id = unit.Code,
                    Name = unit.Name,
                });
            }
            else
            {
                firm.Name = unit.Name;
            }
        }
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdditionalPagesAsync(ApplicationDbContext context)
    {
        // Remove direct sale page if exists, as it is merged into جهة البيع
        var directSalePage = await context.Set<Page>().FirstOrDefaultAsync(p => p.To == "/seizure-direct-sale");
        if (directSalePage != null)
        {
            context.Set<Page>().Remove(directSalePage);
            await context.SaveChangesAsync();
        }

        var additionalPages = new List<Page>
        {
            new()
            {
                Label = "جهاز الخدمات العامة",
                To = "/public-services",
                Icon = "pi pi-building",
                SortNumber = 12,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            }
        };

        foreach (var page in additionalPages)
        {
            var exists = await context.Set<Page>().AnyAsync(p => p.To == page.To);
            if (!exists)
            {
                context.Set<Page>().Add(page);
            }
        }
        await context.SaveChangesAsync();
    }

    private static async Task SeedSaleEntitiesAsync(ApplicationDbContext context)
    {
        var entities = new[]
        {
            new { Name = "جهاز الخدمات العامة", IsPublicService = true },
            new { Name = "لجنة البيع المباشر", IsPublicService = false }
        };

        foreach (var item in entities)
        {
            var existing = await context.Set<SaleEntity>().FirstOrDefaultAsync(s => s.Name == item.Name);
            if (existing == null)
            {
                context.Set<SaleEntity>().Add(new SaleEntity { Name = item.Name, IsPublicService = item.IsPublicService });
            }
            else if (existing.IsPublicService != item.IsPublicService)
            {
                existing.IsPublicService = item.IsPublicService;
            }
        }
        await context.SaveChangesAsync();
    }

    private static async Task SeedRanksAsync(ApplicationDbContext context)
    {
        if (await context.Set<Rank>().AnyAsync())
            return;

        var ranks = new (int Id, string Name)[]
        {
            (1, "جندي"),
            (2, "عريف"),
            (3, "رقيب"),
            (4, "رقيب أول"),
            (5, "مساعد"),
            (6, "مساعد أول"),
            (7, "ملازم"),
            (8, "ملازم أول"),
            (9, "نقيب"),
            (10, "رائد"),
            (11, "مقدم"),
            (12, "عقيد"),
            (13, "عميد"),
            (14, "لواء"),
            (15, "فريق"),
            (16, "فريق أول"),
            (17, "مشير")
        };

        foreach (var (id, name) in ranks)
        {
            await context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO rank (id, name, is_deleted, time_stamp)
                OVERRIDING SYSTEM VALUE
                VALUES ({id}, {name}, false, NOW())");
        }

        await context.Database.ExecuteSqlRawAsync(@"
            SELECT setval(
                pg_get_serial_sequence('rank', 'id'),
                COALESCE((SELECT MAX(id) FROM rank), 1))");
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new (string Id, string Name, string NameAr, string Color, RoleMember Member)[]
        {
            (AdminRoleId, "Admin", "مدير", "#3B82F6", RoleMember.Admin),
            ("2", "BorderGuardOffice", "مكتب حرس حدود", "#10B981", RoleMember.BorderGuardOffice),
            ("3", "SubCommittee", "لجنة فرعية", "#10B981", RoleMember.SubCommittee),
            ("4", "PermanentCommittee", "لجنة دائمة", "#F59E0B", RoleMember.PermanentCommittee),
            ("5", "ServicesAgency", "جهاز الخدمات العامة", "#F59E0B", RoleMember.ServicesAgency),
            ("6", "FinanceAuthority", "هيئة الشئون المالية", "#F59E0B", RoleMember.FinanceAuthority),
        };

        foreach (var role in roles)
        {
            var existing = await context.Roles.FirstOrDefaultAsync(r => r.Id == role.Id || r.Name == role.Name);
            if (existing == null)
            {
                context.Roles.Add(new ApplicationRole
                {
                    Id = role.Id,
                    Name = role.Name,
                    NameAr = role.NameAr,
                    Color = role.Color,
                    RoleMember = role.Member.ToString()
                });
            }
            else
            {
                if (string.IsNullOrEmpty(existing.NameAr))
                {
                    existing.NameAr = role.NameAr;
                }
            }
        }
        await context.SaveChangesAsync();
    }

    private static async Task SeedPagesAsync(ApplicationDbContext context)
    {
        var rootPages = new List<Page>
        {
            new()
            {
                Label = "لوحة البيانات",
                To = "/",
                Icon = "pi pi-fw pi-home",
                SortNumber = 1,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "الضبطيات",
                To = "/seizure-request",
                Icon = "pi pi-inbox",
                SortNumber = 2,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "اللجنة الفرعية",
                To = "/seizure-sub-committee",
                Icon = "pi pi-users",
                SortNumber = 3,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "اللجنة الدائمة",
                To = "/seizure-permanent-committee",
                Icon = "pi pi-users",
                SortNumber = 4,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "جهة البيع",
                To = "/seizure-sales-entity",
                Icon = "pi pi-shopping-cart",
                SortNumber = 5,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "هيئة شئون مالية",
                To = "/seizure-finance-authority",
                Icon = "pi pi-wallet",
                SortNumber = 6,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "مكتمل",
                To = "/seizure-completed",
                Icon = "pi pi-check-circle",
                SortNumber = 7,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "تصنيفات المضبوطات",
                To = "/seizure-detail",
                Icon = "pi pi-fw pi-bars",
                SortNumber = 8,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "إدارة المستخدمين و الصلاحيات",
                To = "/users-roles",
                Icon = "pi pi-fw pi-user",
                SortNumber = 9,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
            new()
            {
                Label = "الأستعلامات",
                To = "/approvels",
                Icon = "pi pi-file-pdf",
                SortNumber = 10,
                IsSearchable = true,
                IsNavigable = true,
                IsSidebarItem = true
            },
        };

        foreach (var p in rootPages)
        {
            var exists = await context.Set<Page>().AnyAsync(page => page.To == p.To && page.ParentId == null);
            if (!exists)
            {
                context.Set<Page>().Add(p);
            }
        }
        await context.SaveChangesAsync();

        var seizureRequestPage = await context.Set<Page>().FirstOrDefaultAsync(p => p.To == "/seizure-request" && p.ParentId == null);
        if (seizureRequestPage != null)
        {
            var detailExists = await context.Set<Page>().AnyAsync(p => p.To == "/:id" && p.ParentId == seizureRequestPage.Id);
            if (!detailExists)
            {
                context.Set<Page>().Add(new Page
                {
                    Label = "البيانات التفصيلية للطلب",
                    To = "/:id",
                    Icon = "pi pi-fw pi-file",
                    ParentId = seizureRequestPage.Id,
                    SortNumber = 1,
                    IsSearchable = false,
                    IsNavigable = true,
                    IsSidebarItem = false
                });
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext context, IUserService userService)
    {
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == DefaultAdminUserName);
        if (adminUser == null)
        {
            var registerModel = new RegisterModel
            {
                Username = DefaultAdminUserName,
                Name = "System Administrator",
                Password = DefaultAdminPassword,
                ConfirmPassword = DefaultAdminPassword,
                Status = "Active",
                FirmId = SeedFirmCode,
                FirmName = "قيادة قوات حرس الحدود (Admin)",
                Roles = new List<ApplicationRole>
                {
                    new() { Id = AdminRoleId }
                }
            };

            var user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Username,
                Name = registerModel.Name,
                FirmId = registerModel.FirmId,
                Status = registerModel.Status
            };

            var result = await userService.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to seed admin user via register flow.");

            foreach (var role in registerModel.Roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Id == role.Id))
                    continue;

                context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
            await context.SaveChangesAsync();
        }
        else
        {
            if (adminUser.FirmId != SeedFirmCode)
            {
                adminUser.FirmId = SeedFirmCode;
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedPageRolesAsync(ApplicationDbContext context)
    {
        var pages = await context.Set<Page>().ToListAsync();
        var pageMap = pages.ToDictionary(p => (p.To ?? "").Trim(), p => p.Id, StringComparer.OrdinalIgnoreCase);

        var rolePermissions = new Dictionary<string, string[]>
        {
            [AdminRoleId] = new[]
            {
                "/", "/seizure-request", "/:id", "/seizure-sub-committee", "/seizure-permanent-committee",
                "/seizure-sales-entity", "/seizure-finance-authority", "/seizure-completed", "/seizure-detail",
                "/users-roles", "/approvels", "/public-services"
            },
            ["2"] = new[] // BorderGuardOffice
            {
                "/", "/seizure-request", "/:id", "/seizure-detail", "/approvels", "/seizure-completed"
            },
            ["3"] = new[] // SubCommittee
            {
                "/", "/seizure-sub-committee", "/seizure-request", "/:id", "/seizure-sales-entity",
                "/seizure-detail", "/approvels", "/seizure-completed"
            },
            ["4"] = new[] // PermanentCommittee
            {
                "/", "/seizure-permanent-committee", "/seizure-request", "/:id", "/seizure-sales-entity",
                "/seizure-detail", "/approvels", "/seizure-completed"
            },
            ["5"] = new[] // ServicesAgency
            {
                "/", "/public-services", "/seizure-sales-entity", "/seizure-request", "/:id",
                "/approvels", "/seizure-completed"
            },
            ["6"] = new[] // FinanceAuthority
            {
                "/", "/seizure-finance-authority", "/seizure-sales-entity", "/seizure-request", "/:id",
                "/approvels", "/seizure-completed"
            }
        };

        foreach (var (roleId, pagePaths) in rolePermissions)
        {
            foreach (var path in pagePaths)
            {
                if (!pageMap.TryGetValue(path, out var pageId))
                    continue;

                var existing = await context.Set<PageRole>()
                    .FirstOrDefaultAsync(pr => pr.RoleId == roleId && pr.PageId == pageId);

                if (existing == null)
                {
                    context.Set<PageRole>().Add(new PageRole
                    {
                        RoleId = roleId,
                        PageId = pageId,
                        CreateAction = true,
                        ReadAction = true,
                        UpdateAction = true,
                        DeleteAction = true
                    });
                }
                else
                {
                    existing.CreateAction = true;
                    existing.ReadAction = true;
                    existing.UpdateAction = true;
                    existing.DeleteAction = true;
                }
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedUnitsOfMeasureAsync(ApplicationDbContext context)
    {
        var units = new (int Id, string Name)[]
        {
            (1, "كجم"),
            (2, "جرام"),
            (3, "طن"),
            (4, "لتر"),
            (5, "مليلتر"),
            (6, "متر"),
            (7, "سم"),
            (8, "قطعة"),
            (9, "كرتونة"),
            (10, "صندوق"),
            (11, "شيكارة"),
            (12, "باكو"),
            (13, "عبوة"),
            (14, "طرد"),
            (15, "برميل"),
            (16, "جركن"),
            (17, "لفة"),
            (18, "زوج"),
            (19, "رأس"),
            (20, "اسطوانة")
        };

        foreach (var (id, name) in units)
        {
            var exists = await context.Set<UnitOfMeasure>().AnyAsync(u => u.Id == id);
            if (!exists)
            {
                await context.Database.ExecuteSqlInterpolatedAsync($@"
                    INSERT INTO unit_of_measure (id, name, is_deleted, time_stamp)
                    OVERRIDING SYSTEM VALUE
                    VALUES ({id}, {name}, false, NOW())");
            }
            else
            {
                var existing = await context.Set<UnitOfMeasure>().FirstOrDefaultAsync(u => u.Id == id);
                if (existing != null && existing.Name != name)
                {
                    existing.Name = name;
                }
            }
        }
        await context.SaveChangesAsync();

        await context.Database.ExecuteSqlRawAsync(@"
            SELECT setval(
                pg_get_serial_sequence('unit_of_measure', 'id'),
                COALESCE((SELECT MAX(id) FROM unit_of_measure), 1))");
    }
}
