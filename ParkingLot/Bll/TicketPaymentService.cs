using ParkingLot.Dal;
using ParkingLot.Dtos;
using ParkingLot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Bll
{
    public class TicketPaymentService
    {
        private readonly TicketPriceCalculator _ticketPriceCalculator;
        private readonly ITicketsRepository _ticketsRepository;

        public TicketPaymentService(TicketPriceCalculator ticketPriceCalculator, ITicketsRepository ticketsRepository)
        {
            _ticketPriceCalculator = ticketPriceCalculator;
            _ticketsRepository = ticketsRepository;
        }

        public async Task<Ticket> PayTicketAsync(Ticket ticket, PaymentRequest paymentRequest, CancellationToken cancellationToken)
        {
            int price = _ticketPriceCalculator.GetPriceFor(ticket);
            if (price > 0)
            {
                ticket.PayedAmount += price;
                ticket.PaymentMethod = paymentRequest.PaymentMethod;
                ticket.PayedAt = DateTimeOffset.UtcNow;
                await _ticketsRepository.UpdateTicketAsync(ticket, cancellationToken);
            }

            return ticket;
        }
    }
}
