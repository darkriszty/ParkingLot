using Microsoft.EntityFrameworkCore;
using ParkingLot.Models;

namespace ParkingLot.Dal
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
