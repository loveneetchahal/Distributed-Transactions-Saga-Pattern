﻿using Saga.Core.MessageBrokers.Models;
using Saga.Shared.Consumers.Abstract;
using System;

namespace Saga.Shared.Consumers.Models.Sagas
{
    public class SendBookingRequestEvent : EventModel, ISendBookingRequestEvent
    {
        public int BookingId { get; set; }
        public string Email { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}