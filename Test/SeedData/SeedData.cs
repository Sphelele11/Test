using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.SeedData
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Seed roles
            string[] roles = { "Admin", "Agent", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            if (await userManager.FindByEmailAsync("admin@itsm.com") == null)
            {
                var admin = new IdentityUser { UserName = "admin@itsm.com", Email = "admin@itsm.com", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed agent user
            if (await userManager.FindByEmailAsync("agent@itsm.com") == null)
            {
                var agent = new IdentityUser { UserName = "agent@itsm.com", Email = "agent@itsm.com", EmailConfirmed = true };
                await userManager.CreateAsync(agent, "Agent@123");
                await userManager.AddToRoleAsync(agent, "Agent");
            }

            // Seed sample data
            if (!context.Incidents.Any())
            {
                var incidents = new List<Incident>
            {
                new() { TicketNumber = "INC-0001", Title = "Email server not responding", Description = "Users unable to access email since 8AM.", Priority = Priority.Critical, Status = IncidentStatus.InProgress, Category = Category.Software, AssignedTo = "agent@itsm.com", ReportedBy = "user@company.com", AffectedSystem = "Exchange Server", CreatedAt = DateTime.UtcNow.AddDays(-2), UpdatedAt = DateTime.UtcNow.AddDays(-1), SLADueDate = DateTime.UtcNow.AddHours(2) },
                new() { TicketNumber = "INC-0002", Title = "Printer offline in Marketing dept", Description = "The HP LaserJet in marketing is showing offline.", Priority = Priority.Medium, Status = IncidentStatus.Open, Category = Category.Hardware, AssignedTo = "agent@itsm.com", ReportedBy = "marketing@company.com", AffectedSystem = "HP LaserJet 4015", CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow.AddDays(-1), SLADueDate = DateTime.UtcNow.AddHours(8) },
                new() { TicketNumber = "INC-0003", Title = "VPN connectivity issues", Description = "Remote workers unable to connect to VPN.", Priority = Priority.High, Status = IncidentStatus.InProgress, Category = Category.Network, AssignedTo = "agent@itsm.com", ReportedBy = "remote@company.com", CreatedAt = DateTime.UtcNow.AddHours(-5), UpdatedAt = DateTime.UtcNow.AddHours(-2), SLADueDate = DateTime.UtcNow.AddHours(1) },
                new() { TicketNumber = "INC-0004", Title = "Password reset request", Description = "User locked out after multiple failed attempts.", Priority = Priority.Low, Status = IncidentStatus.Resolved, Category = Category.Access, ReportedBy = "user2@company.com", CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-2), ResolvedAt = DateTime.UtcNow.AddDays(-2), SLADueDate = DateTime.UtcNow.AddDays(-1), ResolutionNotes = "Password reset completed." },
                new() { TicketNumber = "INC-0005", Title = "Database performance degradation", Description = "Production database showing high query times.", Priority = Priority.Critical, Status = IncidentStatus.Open, Category = Category.Software, AssignedTo = "agent@itsm.com", ReportedBy = "dba@company.com", AffectedSystem = "SQL Server 2019", CreatedAt = DateTime.UtcNow.AddHours(-1), UpdatedAt = DateTime.UtcNow.AddHours(-1), SLADueDate = DateTime.UtcNow.AddHours(1) },
            };
                context.Incidents.AddRange(incidents);
            }

            if (!context.Problems.Any())
            {
                context.Problems.AddRange(
                    new Problem { TicketNumber = "PRB-0001", Title = "Recurring email outages", Description = "Email system has experienced 3 outages in the past month.", Priority = Priority.High, Status = ProblemStatus.UnderInvestigation, Category = Category.Software, AssignedTo = "agent@itsm.com", Workaround = "Restart Exchange services.", CreatedAt = DateTime.UtcNow.AddDays(-10), UpdatedAt = DateTime.UtcNow.AddDays(-1) },
                    new Problem { TicketNumber = "PRB-0002", Title = "VPN instability", Description = "VPN drops connections for remote users.", Priority = Priority.High, Status = ProblemStatus.KnownError, Category = Category.Network, AssignedTo = "agent@itsm.com", RootCause = "Firmware bug in VPN concentrator.", Workaround = "Reconnect after 30 minutes.", CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
                );
            }

            if (!context.ChangeRequests.Any())
            {
                context.ChangeRequests.AddRange(
                    new ChangeRequest { TicketNumber = "CHG-0001", Title = "Exchange Server upgrade to 2022", Description = "Upgrade production Exchange from 2019 to 2022.", Type = ChangeType.Normal, Status = ChangeStatus.Approved, Risk = ChangeRisk.High, Priority = Priority.High, RequestedBy = "admin@itsm.com", AssignedTo = "agent@itsm.com", Justification = "End of support for 2019.", ImplementationPlan = "Step 1: Backup. Step 2: Install. Step 3: Migrate.", BackoutPlan = "Restore from backup.", PlannedStartDate = DateTime.UtcNow.AddDays(5), PlannedEndDate = DateTime.UtcNow.AddDays(6), CreatedAt = DateTime.UtcNow.AddDays(-7), UpdatedAt = DateTime.UtcNow.AddDays(-2) },
                    new ChangeRequest { TicketNumber = "CHG-0002", Title = "VPN firmware update", Description = "Apply vendor patch to fix connection drops.", Type = ChangeType.Emergency, Status = ChangeStatus.Submitted, Risk = ChangeRisk.Medium, Priority = Priority.Critical, RequestedBy = "agent@itsm.com", Justification = "Fixes critical bug.", ImplementationPlan = "Apply patch during maintenance window.", BackoutPlan = "Rollback to previous firmware.", PlannedStartDate = DateTime.UtcNow.AddDays(1), PlannedEndDate = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow }
                );
            }

            if (!context.ServiceRequests.Any())
            {
                context.ServiceRequests.AddRange(
                    new ServiceRequest { TicketNumber = "SRQ-0001", Title = "New laptop for onboarding employee", Description = "Need a laptop configured for new hire starting Monday.", Status = ServiceRequestStatus.InFulfillment, Category = ServiceRequestCategory.NewEquipment, Priority = Priority.Medium, RequestedBy = "hr@company.com", AssignedTo = "agent@itsm.com", Department = "HR", CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-1), SLADueDate = DateTime.UtcNow.AddDays(1) },
                    new ServiceRequest { TicketNumber = "SRQ-0002", Title = "Adobe Creative Suite license", Description = "Request for Adobe CC license for design team.", Status = ServiceRequestStatus.Submitted, Category = ServiceRequestCategory.Software, Priority = Priority.Low, RequestedBy = "design@company.com", Department = "Design", CreatedAt = DateTime.UtcNow.AddHours(-4), UpdatedAt = DateTime.UtcNow.AddHours(-4), SLADueDate = DateTime.UtcNow.AddDays(5) }
                );
            }

            if (!context.Assets.Any())
            {
                context.Assets.AddRange(
                    new Asset { AssetTag = "AST-001", Name = "Exchange Server 01", Type = AssetType.Server, Status = AssetStatus.InUse, Manufacturer = "Dell", Model = "PowerEdge R750", SerialNumber = "SN1234567", Location = "DC-Rack-A1", IpAddress = "10.0.0.10", OperatingSystem = "Windows Server 2019", PurchaseCost = 15000, PurchaseDate = new DateTime(2022, 3, 15), WarrantyExpiry = new DateTime(2025, 3, 15) },
                    new Asset { AssetTag = "AST-002", Name = "Web Server 01", Type = AssetType.Server, Status = AssetStatus.InUse, Manufacturer = "HP", Model = "ProLiant DL380", SerialNumber = "SN7654321", Location = "DC-Rack-B2", IpAddress = "10.0.0.20", OperatingSystem = "Ubuntu 22.04", PurchaseCost = 12000, PurchaseDate = new DateTime(2022, 6, 1), WarrantyExpiry = new DateTime(2025, 6, 1) },
                    new Asset { AssetTag = "AST-003", Name = "John Doe Laptop", Type = AssetType.Laptop, Status = AssetStatus.InUse, Manufacturer = "Lenovo", Model = "ThinkPad T14", SerialNumber = "LP9012345", AssignedTo = "john.doe@company.com", Department = "IT", OperatingSystem = "Windows 11", PurchaseCost = 1800, PurchaseDate = new DateTime(2023, 1, 10), WarrantyExpiry = new DateTime(2026, 1, 10) },
                    new Asset { AssetTag = "AST-004", Name = "Network Switch Core", Type = AssetType.Network, Status = AssetStatus.InUse, Manufacturer = "Cisco", Model = "Catalyst 9300", SerialNumber = "SW5678901", Location = "DC-Rack-C1", IpAddress = "10.0.0.1", PurchaseCost = 8500, PurchaseDate = new DateTime(2021, 9, 20), WarrantyExpiry = new DateTime(2024, 9, 20) },
                    new Asset { AssetTag = "AST-005", Name = "Old Server 2012", Type = AssetType.Server, Status = AssetStatus.Retired, Manufacturer = "HP", Model = "ProLiant DL360 G8", SerialNumber = "OLD123456", Location = "DC-Rack-D4", OperatingSystem = "Windows Server 2012", PurchaseCost = 6000, PurchaseDate = new DateTime(2015, 4, 5) }
                );
            }

            if (!context.KnowledgeArticles.Any())
            {
                context.KnowledgeArticles.AddRange(
                    new KnowledgeArticle { ArticleNumber = "KB-001", Title = "How to reset your password", Summary = "Step-by-step guide for password reset.", Content = "## Password Reset Guide\n\n1. Go to the login page\n2. Click 'Forgot Password'\n3. Enter your email address\n4. Check your email for the reset link\n5. Click the link and enter a new password\n\n**Note:** Password must be at least 8 characters.", Status = ArticleStatus.Published, Category = Category.Access, Tags = "password,reset,account", Author = "admin@itsm.com", Views = 245, HelpfulVotes = 42, CreatedAt = DateTime.UtcNow.AddDays(-30), PublishedAt = DateTime.UtcNow.AddDays(-28) },
                    new KnowledgeArticle { ArticleNumber = "KB-002", Title = "VPN Setup Guide for Remote Workers", Summary = "Configure VPN on Windows and Mac.", Content = "## VPN Setup\n\n### Windows\n1. Download the VPN client from IT portal\n2. Run the installer\n3. Enter server: vpn.company.com\n4. Login with your AD credentials\n\n### Mac\n1. Go to System Preferences > Network\n2. Click '+' to add new VPN\n3. Configure with provided settings", Status = ArticleStatus.Published, Category = Category.Network, Tags = "vpn,remote,connectivity", Author = "agent@itsm.com", Views = 389, HelpfulVotes = 67, CreatedAt = DateTime.UtcNow.AddDays(-20), PublishedAt = DateTime.UtcNow.AddDays(-19) },
                    new KnowledgeArticle { ArticleNumber = "KB-003", Title = "Email Troubleshooting Steps", Summary = "Common email issues and fixes.", Content = "## Email Troubleshooting\n\n### Cannot Send Email\n- Check internet connection\n- Verify SMTP settings\n- Check outbox for stuck messages\n\n### Cannot Receive Email\n- Check spam/junk folder\n- Verify mailbox quota\n- Contact IT if quota is full", Status = ArticleStatus.Published, Category = Category.Software, Tags = "email,outlook,troubleshooting", Author = "admin@itsm.com", Views = 512, HelpfulVotes = 89, CreatedAt = DateTime.UtcNow.AddDays(-15), PublishedAt = DateTime.UtcNow.AddDays(-14) }
                );
            }

            await context.SaveChangesAsync();

        }
    }

}
