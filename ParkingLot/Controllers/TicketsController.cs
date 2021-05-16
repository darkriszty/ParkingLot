using Microsoft.AspNetCore.Mvc;
using ParkingLot.Dal;
using ParkingLot.Dtos;
using ParkingLot.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ParkingLot.Bll;

namespace ParkingLot.Controllers
{
    [ApiController]
    [Route("api")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly TicketPriceCalculator _ticketPriceCalculator;
        private readonly TicketPaymentService _ticketPaymentService;

        public TicketsController(ITicketsRepository ticketsRepository, TicketPriceCalculator ticketPriceCalculator, TicketPaymentService ticketPaymentService)
        {
            _ticketsRepository = ticketsRepository;
            _ticketPriceCalculator = ticketPriceCalculator;
            _ticketPaymentService = ticketPaymentService;
        }

        [HttpPost]
        [Route("tickets")]
        public async Task<IActionResult> NewTicket()
        {
            using var timeout = StandardTimeoutCts;
            var response = MapGetTicketResponse(await _ticketsRepository.TryGenerateNewTicketAsync(timeout.Token));
            return !response.Success 
                ? StatusCode((int)HttpStatusCode.Forbidden, response) 
                : Ok(response);
        }

        [HttpGet]
        [Route("tickets")]
        public async Task<IActionResult> GetTicketsWithCars()
        {
            using var timeout = StandardTimeoutCts;
            var tickets = await _ticketsRepository.GetCurrentTicketsAsync(timeout.Token);
            return Ok(ApiResponse<Ticket[]>.SuccessResult(tickets));
        }

        [HttpGet]
        [Route("tickets/{id}")]
        public async Task<IActionResult> GetTicketPrice(string id)
        {
            using var timeout = StandardTimeoutCts;
            if (!Guid.TryParse(id, out var parsedTickedId))
                return BadRequest(ApiResponse<PriceResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, timeout.Token);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PriceResponse>.FailResponse($"Ticket with id {id} was not found."));

            int price = _ticketPriceCalculator.GetPriceFor(ticket);
            return Ok(ApiResponse<PriceResponse>.SuccessResult(new PriceResponse(price)));
        }

        [HttpPost]
        [Route("tickets/{id}/payments")]
        public async Task<IActionResult> Pay(string id, [FromBody]PaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse("Invalid payment request."));
            if (string.IsNullOrWhiteSpace(paymentRequest.PaymentMethod))
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse("Missing payment method."));

            if (!Guid.TryParse(id, out var parsedTickedId))
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            using var getTicketTimeout = StandardTimeoutCts;
            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, getTicketTimeout.Token);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PaymentResponse>.FailResponse($"Ticket with id {id} was not found."));

            using var payTicketTimeout = StandardTimeoutCts;
            Ticket payedTicket = await _ticketPaymentService.PayTicketAsync(ticket, paymentRequest, payTicketTimeout.Token);
            return Ok(ApiResponse<PaymentResponse>.SuccessResult(new PaymentResponse(payedTicket)));
        }

        [HttpGet]
        [Route("tickets/{id}/state")]
        public async Task<IActionResult> State(string id)
        {
            if (!Guid.TryParse(id, out var parsedTickedId))
                return BadRequest(ApiResponse<PaymentStateResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            using var timeout = StandardTimeoutCts;
            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, timeout.Token);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PaymentStateResponse>.FailResponse($"Ticket with id {id} was not found."));

            int price = _ticketPriceCalculator.GetPriceFor(ticket);
            return Ok(ApiResponse<PaymentStateResponse>.SuccessResult(response: new PaymentStateResponse(price)));
        }

        [HttpPost]
        [Route("tickets/{id}/leave")]
        public async Task<IActionResult> Leave(string id)
        {
            if (!Guid.TryParse(id, out var parsedTickedId))
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            using var getTicketsTimeout = StandardTimeoutCts;
            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, getTicketsTimeout.Token);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PaymentResponse>.FailResponse($"Ticket with id {id} was not found."));

            int price = _ticketPriceCalculator.GetPriceFor(ticket);
            if (price > 0)
            {
                return Unauthorized(ApiResponse<LeaveParkingResponse>.FailResponse($"Not possible to leave. For {id} an amount of ${price} must be paid."));
            }

            using var leaveParkingTimeout = StandardTimeoutCts;
            await _ticketsRepository.MarkLeaveParking(ticket, leaveParkingTimeout.Token);

            return Ok(ApiResponse<LeaveParkingResponse>.SuccessResult(new LeaveParkingResponse(ticket)));
        }

        [HttpGet]
        [Route("free-spaces")]
        public async Task<IActionResult> NumberOfFreeSpaces()
        {
            using var timeout = StandardTimeoutCts;
            int freeSpaces = await _ticketsRepository.GetFreeSpacesAsync(timeout.Token);
            return Ok(ApiResponse<FreeSpacesResponse>.SuccessResult(response: new FreeSpacesResponse(freeSpaces)));
        }

        private ApiResponse<GetTicketResponse> MapGetTicketResponse(Ticket ticket)
        {
            bool ticketCreated = ticket != Ticket.None;
            return new ApiResponse<GetTicketResponse>
            {
                Success = ticketCreated,
                ErrorMessage = !ticketCreated ? "The parking lot is full." : null,
                Response = new GetTicketResponse(ticket)
            };
        }

        private CancellationTokenSource StandardTimeoutCts => new CancellationTokenSource(TimeSpan.FromSeconds(30));
    }
}
