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
                var payedTicket = PayTicket(ticket, price, paymentRequest.PaymentMethod);
                await _ticketsRepository.UpdateTicketAsync(payedTicket, cancellationToken);
                return payedTicket;
            }

            return ticket;
        }

        private static Ticket PayTicket(Ticket ticket, int price, string paymentMethod)
            => ticket with
               {
                   PayedAmount = ticket.PayedAmount + price,
                   PaymentMethod = paymentMethod,
                   PayedAt = DateTimeOffset.UtcNow
               };
    }
}
