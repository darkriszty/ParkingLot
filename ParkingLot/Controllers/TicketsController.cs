using Microsoft.AspNetCore.Mvc;
using ParkingLot.Dal;
using ParkingLot.Dtos;
using ParkingLot.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsRepository _ticketsRepository;

        public TicketsController(ITicketsRepository ticketsRepository)
        {
            _ticketsRepository = ticketsRepository;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EnterParkingLot()
        {
            var response = MapTicketResponse(await _ticketsRepository.TryGenerateNewTicketAsync(Timeout));
            return !response.Success 
                ? StatusCode((int)HttpStatusCode.Forbidden, response) 
                : Ok(response);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _ticketsRepository.GetCurrentTicketsAsync(Timeout));
        }

        private ApiResponse<TicketViewModel> MapTicketResponse(Ticket ticket)
        {
            bool ticketCreated = ticket != Ticket.None;
            return new ApiResponse<TicketViewModel>
            {
                Success = ticketCreated,
                ErrorMessage = !ticketCreated ? "The parking lot is full." : null,
                Response = new TicketViewModel(ticket)
            };
        }

        private CancellationToken Timeout => new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
    }
}
