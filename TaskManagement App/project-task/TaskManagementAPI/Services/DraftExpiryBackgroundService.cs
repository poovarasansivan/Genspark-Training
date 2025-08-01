using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Data;

namespace TaskManagementAPI.Services
{
    public class DraftExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DraftExpiryBackgroundService> _logger;

        public DraftExpiryBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<DraftExpiryBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Draft expiry background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var taskRepo = scope.ServiceProvider.GetRequiredService<ITaskItemRepository>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();

                    var expiredDrafts = await taskRepo.GetDraftsOlderThanAsync(DateTime.UtcNow.AddDays(-3));

                    if (expiredDrafts.Any())
                    {
                        foreach (var draft in expiredDrafts)
                        {
                            draft.IsDeleted = true;
                            draft.UpdatedAt = DateTime.UtcNow;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Expired {expiredDrafts.Count()} draft tasks.");
                    }
                    else
                    {
                        _logger.LogInformation("No expired draft tasks found.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while expiring drafts.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
