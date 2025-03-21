using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Common;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
} 