﻿using FlightService.Domain.Entities;
using FlightService.Infrastructure.Persistence;
using MassTransit;
using Saga.Core.Concrete.Brokers;
using Saga.Core.Constants;
using Saga.Core.MessageBrokers.Concrete;
using Saga.Shared.Consumers.Abstract;
using Saga.Shared.Consumers.Models.Flight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightService.Consumer.Consumer
{
    public class CreateFlightBookingConsumer : IConsumer<ICreateFlightBookingEvent>
    {
        private readonly FlightDbContext _dbContext;

        public CreateFlightBookingConsumer(FlightDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ICreateFlightBookingEvent> context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffffff")}: Let's create Flight Booking for Booking ID - " + context.Message.BookingId);

            Flight flight = new Flight
            {
                Created = DateTime.Now,
                CreatedBy = "TEst",
                Price = 200,
                From = DateTime.Now.ToString(),
                To = DateTime.Now.AddDays(4).ToString(),
                Type = "Tip"
            };

            _dbContext.Flights.Add(flight);
            _dbContext.SaveChanges();

            await context.Publish<IHotelBookingFailedEvent>(new
            {
                CreatedDate = DateTime.Now,
                BookingId = context.Message.BookingId,
               CorrelationId = context.Message.CorrelationId
        });

            //await context.Publish<IHotelBookingFailedEvent>(new
            //{
            //    CreatedDate = DateTime.Now,
            //    BookingId = context.Message.BookingId
            //});

            //await context.RespondAsync<IHotelBookingFailedEvent>(new
            //{
            //    context.Message.BookingId,
            //    context.Message.CreatedDate
            //});

            //if (flight.Price == 200)
            //{
            //    await context.Publish<IHotelBookingFailedEventModel>(new
            //    {
            //        CreatedDate = DateTime.Now,
            //        BookingId = context.Message.BookingId
            //    });
            //}

            //try
            //{
            //    throw new Exception("Greska");
            //}
            //catch(Exception e)
            //{
            //    //Console.WriteLine("Create Flight Booking went wrong with ID " + context.Message.BookingId);

            //    await context.Publish<IHotelBookingFailedEventModel>(new
            //    {
            //        CreatedDate = DateTime.Now,
            //        BookingId = context.Message.BookingId+1,
            //        FlightId = context.Message.FlightId
            //    });
            //}

        }
    }
}
