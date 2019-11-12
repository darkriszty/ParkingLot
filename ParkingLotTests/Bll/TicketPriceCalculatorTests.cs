using System;
using NUnit.Framework;
using ParkingLot.Bll;
using ParkingLot.Models;

namespace ParkingLotTests.Bll
{
    public class Tests
    {
        [Test]
        public void GetPriceFor_OneHourTicket_Costs2()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddHours(-1)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(2));
        }

        [Test]
        public void GetPriceFor_TwoHourTicket_Costs4()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddHours(-2)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(4));
        }


        [Test]
        public void GetPriceFor_45MinTicket_Costs2()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddMinutes(-45)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(2));
        }

        [Test]
        public void GetPriceFor_1MinTicket_Costs2()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddMinutes(-1)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(2));
        }

        [Test]
        public void GetPriceFor_OneAndAHalfHourTicket_Costs4()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddMinutes(-90)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(4));
        }

        [Test]
        public void GetPriceFor_OneDayTicket_Costs48()
        {
            var ticket = new Ticket
            {
                IssueDate = DateTimeOffset.UtcNow.AddDays(-1)
            };
            var sut = CreateSut();

            int price = sut.GetPriceFor(ticket);

            Assert.That(price, Is.EqualTo(48));
        }

        [Test]
        public void GetPriceFor_EmptyTicket_DoesNotThrow()
        {
            var ticket = Ticket.None;
            var sut = CreateSut();

            sut.GetPriceFor(ticket);

            Assert.Pass();
        }

        private TicketPriceCalculator CreateSut() => new TicketPriceCalculator();
    }
}