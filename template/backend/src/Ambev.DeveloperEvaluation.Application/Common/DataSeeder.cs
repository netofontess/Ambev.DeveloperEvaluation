using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Common;

public class DataSeeder : IDataSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<DataSeeder> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        try
        {
            var adminConfig = _configuration.GetSection("AdminUser");
            if (!adminConfig.Exists())
            {
                _logger.LogWarning("Admin user configuration not found");
                return;
            }

            var adminEmail = adminConfig["Email"];
            var existingAdmin = await _userRepository.GetByEmailAsync(adminEmail, cancellationToken);

            if (existingAdmin == null)
            {
                _logger.LogInformation("Creating admin user...");

                var adminUser = new User
                {
                    Username = adminConfig["Username"],
                    Email = adminEmail,
                    Phone = adminConfig["Phone"],
                    Password = _passwordHasher.HashPassword(adminConfig["Password"]),
                    Role = Enum.Parse<UserRole>(adminConfig["Role"]),
                    Status = Enum.Parse<UserStatus>(adminConfig["Status"])
                };

                await _userRepository.CreateAsync(adminUser, cancellationToken);
                _logger.LogInformation("Admin user created successfully");
            }
            else
            {
                _logger.LogInformation("Admin user already exists");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating admin user");
            throw;
        }
    }
} 