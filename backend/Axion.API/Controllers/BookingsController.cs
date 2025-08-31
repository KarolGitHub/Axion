using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
  private readonly AxionDbContext _context;

  public BookingsController(AxionDbContext context)
  {
    _context = context;
  }

  // GET: api/bookings
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
  {
    return await _context.Bookings
        .Include(b => b.Resource)
        .Include(b => b.User)
        .ToListAsync();
  }

  // GET: api/bookings/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Booking>> GetBooking(string id)
  {
    var booking = await _context.Bookings
        .Include(b => b.Resource)
        .Include(b => b.User)
        .FirstOrDefaultAsync(b => b.Id == id);

    if (booking == null)
    {
      return NotFound();
    }

    return booking;
  }

  // GET: api/bookings/resource/5
  [HttpGet("resource/{resourceId}")]
  public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByResource(string resourceId)
  {
    return await _context.Bookings
        .Include(b => b.Resource)
        .Include(b => b.User)
        .Where(b => b.ResourceId == resourceId)
        .OrderBy(b => b.StartTime)
        .ToListAsync();
  }

  // GET: api/bookings/user/5
  [HttpGet("user/{userId}")]
  public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUser(string userId)
  {
    return await _context.Bookings
        .Include(b => b.Resource)
        .Include(b => b.User)
        .Where(b => b.UserId == userId)
        .OrderBy(b => b.StartTime)
        .ToListAsync();
  }

  // POST: api/bookings
  [HttpPost]
  public async Task<ActionResult<Booking>> CreateBooking([FromBody] CreateBookingRequest request)
  {
    // Check for booking conflicts
    var hasConflict = await _context.Bookings
        .AnyAsync(b => b.ResourceId == request.ResourceId &&
                      ((request.StartTime >= b.StartTime && request.StartTime < b.EndTime) ||
                       (request.EndTime > b.StartTime && request.EndTime <= b.EndTime) ||
                       (request.StartTime <= b.StartTime && request.EndTime >= b.EndTime)));

    if (hasConflict)
    {
      return BadRequest(new { message = "Booking conflicts with existing reservation" });
    }

    // Check if resource is available
    var resource = await _context.Resources.FindAsync(request.ResourceId);
    if (resource == null)
    {
      return NotFound(new { message = "Resource not found" });
    }

    if (!resource.IsAvailable)
    {
      return BadRequest(new { message = "Resource is not available for booking" });
    }

    var booking = new Booking
    {
      ResourceId = request.ResourceId,
      UserId = request.UserId,
      StartTime = request.StartTime,
      EndTime = request.EndTime,
      Purpose = request.Purpose
    };

    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
  }

  // PUT: api/bookings/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateBooking(string id, [FromBody] UpdateBookingRequest request)
  {
    var booking = await _context.Bookings.FindAsync(id);
    if (booking == null)
    {
      return NotFound();
    }

    // Check for booking conflicts (excluding current booking)
    var hasConflict = await _context.Bookings
        .AnyAsync(b => b.Id != id &&
                      b.ResourceId == (request.ResourceId ?? booking.ResourceId) &&
                      ((request.StartTime >= b.StartTime && request.StartTime < b.EndTime) ||
                       (request.EndTime > b.StartTime && request.EndTime <= b.EndTime) ||
                       (request.StartTime <= b.StartTime && request.EndTime >= b.EndTime)));

    if (hasConflict)
    {
      return BadRequest(new { message = "Booking conflicts with existing reservation" });
    }

    booking.ResourceId = request.ResourceId ?? booking.ResourceId;
    booking.UserId = request.UserId ?? booking.UserId;
    booking.StartTime = request.StartTime ?? booking.StartTime;
    booking.EndTime = request.EndTime ?? booking.EndTime;
    booking.Purpose = request.Purpose ?? booking.Purpose;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!BookingExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  // DELETE: api/bookings/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteBooking(string id)
  {
    var booking = await _context.Bookings.FindAsync(id);
    if (booking == null)
    {
      return NotFound();
    }

    _context.Bookings.Remove(booking);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  // GET: api/bookings/check-availability
  [HttpGet("check-availability")]
  public async Task<ActionResult<object>> CheckAvailability([FromQuery] string resourceId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
  {
    var hasConflict = await _context.Bookings
        .AnyAsync(b => b.ResourceId == resourceId &&
                      ((startTime >= b.StartTime && startTime < b.EndTime) ||
                       (endTime > b.StartTime && endTime <= b.EndTime) ||
                       (startTime <= b.StartTime && endTime >= b.EndTime)));

    var resource = await _context.Resources.FindAsync(resourceId);
    var isAvailable = resource?.IsAvailable == true && !hasConflict;

    return new { isAvailable, hasConflict };
  }

  private bool BookingExists(string id)
  {
    return _context.Bookings.Any(e => e.Id == id);
  }
}

// DTOs for Booking requests
public class CreateBookingRequest
{
  public string ResourceId { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public string Purpose { get; set; } = string.Empty;
}

public class UpdateBookingRequest
{
  public string? ResourceId { get; set; }
  public string? UserId { get; set; }
  public DateTime? StartTime { get; set; }
  public DateTime? EndTime { get; set; }
  public string? Purpose { get; set; }
}
