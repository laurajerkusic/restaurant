using AbySalto.Junior.Application.Orders;
using AbySalto.Junior.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Junior.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _svc;
    public OrdersController(IOrderService svc) => _svc = svc;

    // LISTA (opcionalno sortiranje preko ?sort=total_desc / total_asc)
    [HttpGet]
    public Task<List<OrderReadDto>> GetAll(
        [FromQuery] string? sort = null,
        CancellationToken ct = default)
        => _svc.GetAllAsync(sort, ct);

    // LISTA (paginirano) + opcionalno sortiranje
    [HttpGet("paged")]
    public Task<PagedResult<OrderReadDto>> GetAllPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null,
        CancellationToken ct = default)
        => _svc.GetAllPagedAsync(page, pageSize, sort, ct);

    // DETALJ
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderReadDto>> GetById(int id, CancellationToken ct)
        => (await _svc.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

    // KREIRAJ
    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PROMJENA STATUSA
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderUpdateStatusDto dto, CancellationToken ct)
        => await _svc.UpdateStatusAsync(id, dto.Status, ct) ? NoContent() : NotFound();

    // BRISANJE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await _svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
