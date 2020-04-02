using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMqUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationProcessor
{
    public class ReservationListener : RabbitListener
    {
        private readonly ILogger<ReservationListener> Logger;
        private readonly ReservationHttpService HttpService;
        public ReservationListener(ReservationHttpService httpService, ILogger<ReservationListener> logger, IOptionsMonitor<RabbitOptions> options) : base(options)
        {
            Logger = logger;
            base.QueueName = "reservations";
            base.ExchangeName = "";
            HttpService = httpService;
        }
        public async override Task<bool> Process(string message)
        {
            // read the message -
            // 1. Deserialize the JSON into a class.
            Logger.LogInformation($"Got the JSON from the Message Queue: {message}");
            var request = JsonSerializer.Deserialize<ReservationModel>(message);
            // 2. Apply business rules to processing the reservation
            if (request.Books.Split(',').Length > 3)
            {
                //    - Any reservation > 3 books gets cancelled. Otherwise, approved.
                return await HttpService.MarkReservationCancelled(request);
            }
            else
            {
                // 3. Post to the appropriate route on our API.
                return await HttpService.MarkReservationApproved(request);
            }
            return true; // if you did some error handling, and you had an error, return false. It will leave it on the Queue
        }
    }
}
