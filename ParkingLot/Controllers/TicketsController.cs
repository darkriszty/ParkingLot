﻿using Microsoft.AspNetCore.Mvc;
using ParkingLot.Dal;
using ParkingLot.Dtos;
using ParkingLot.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ParkingLot.Bll;

namespace ParkingLot.Controllers
{
    [ApiController]
    [Route("api/tickets")]
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
        [Route("")]
        public async Task<IActionResult> NewTicket()
        {
            var response = MapGetTicketResponse(await _ticketsRepository.TryGenerateNewTicketAsync(Timeout));
            return !response.Success 
                ? StatusCode((int)HttpStatusCode.Forbidden, response) 
                : Ok(response);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketsRepository.GetCurrentTicketsAsync(Timeout);
            return Ok(ApiResponse<GetTicketResponse[]>.SuccessResult(
                tickets.Select(_ => new GetTicketResponse(_)).ToArray())
            );
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTicketPrice(string id)
        {
            if (!Guid.TryParse(id, out var parsedTickedId))
                return BadRequest(ApiResponse<PriceResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, Timeout);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PriceResponse>.FailResponse($"Ticket with id {id} was not found."));

            int price = _ticketPriceCalculator.GetPriceFor(ticket);
            return Ok(ApiResponse<PriceResponse>.SuccessResult(new PriceResponse(price)));
        }

        [HttpPost]
        [Route("{id}/payments")]
        public async Task<IActionResult> Pay(string id, [FromBody]PaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse("Invalid payment request."));
            if (string.IsNullOrWhiteSpace(paymentRequest.PaymentMethod))
                return BadRequest(ApiResponse<PaymentResponse>.FailResponse("Missing payment method."));

            if (!Guid.TryParse(id, out var parsedTickedId))
                return NotFound(ApiResponse<PaymentResponse>.FailResponse($"Invalid parking ticket ID: {id}"));

            Ticket ticket = await _ticketsRepository.GetTicketByIdAsync(parsedTickedId, Timeout);
            if (ticket == Ticket.None)
                return NotFound(ApiResponse<PaymentResponse>.FailResponse($"Ticket with id {id} was not found."));

            Ticket payedTicket = await _ticketPaymentService.PayTicketAsync(ticket, paymentRequest, Timeout);
            return Ok(ApiResponse<PaymentResponse>.SuccessResult(new PaymentResponse(payedTicket)));
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

        private CancellationToken Timeout => new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
    }
}
