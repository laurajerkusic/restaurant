using AbySalto.Junior.Application.Orders;
using AbySalto.Junior.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Junior.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _svc;
    public OrdersController(IOrderService svc)
    {
        _svc = svc;
    }


    // LISTA (opcionalno sortiranje preko ?sort=total_desc / total_asc)
    [HttpGet]
    public async Task<List<OrderReadDto>> GetAll(
     [FromQuery] string? sort = null,
     CancellationToken ct = default)
    {
        var result = await _svc.GetAllAsync(sort, ct);
        return result;
    }


    // LISTA (paginirano) + opcionalno sortiranje
    [HttpGet("paged")]
    public async Task<PagedResult<OrderReadDto>> GetAllPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null,
    CancellationToken ct = default)
    {
        var result = await _svc.GetAllPagedAsync(page, pageSize, sort, ct);
        return result;
    }


    // DETALJ
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderReadDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);

        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }


    // KREIRAJ
    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
    {
      
        var created = await _svc.CreateAsync(dto, ct);

       
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }


    // PROMJENA STATUSA

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderUpdateStatusDto dto, CancellationToken ct)
    {
        var updated = await _svc.UpdateStatusAsync(id, dto.Status, ct);

        if (updated)
        {
            return NoContent();
        }

        return NotFound();
    }


    // BRISANJE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _svc.DeleteAsync(id, ct);

        if (deleted)
        {
            return NoContent();
        }

        return NotFound();
    }

}
