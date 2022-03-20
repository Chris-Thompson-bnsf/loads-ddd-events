using Imports.Data;
using Microsoft.AspNetCore.Mvc;
using TestApps.DDD.DomainEvents.WebAPI.Requests;

namespace TestApps.DDD.DomainEvents.WebAPI.Controllers
{
    [ApiController]
    [Route("loads")]
    public class LoadsController : Controller
    {
        private readonly ImportsDbContext _context;

        public LoadsController(ImportsDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Import(ImportLoadRequest model, CancellationToken cancellationToken)
        {
            var efModel = new SavedLoadImportedEvent()
            {
                BolNumber = model.BolNumber,
                CustomerCode = model.Customer,
                DetailsJson = string.Empty,
            };

            await _context.LoadImportedEvents.AddAsync(efModel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var domainEvent = new Imports.Events.LoadImportedEvent()
            {
                BolNumber = model.BolNumber,
                Customer = model.Customer,
            };

            // TODO: Publish event

            return Ok();
        }
    }
}
