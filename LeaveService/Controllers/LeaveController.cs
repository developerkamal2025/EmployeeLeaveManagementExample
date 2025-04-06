using BuildingBlock.Shared.Models;
using LeaveService.Entity;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LeaveService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private static readonly List<Leave> _leaveRequests = new List<Leave>();
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public LeaveController()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Change if using Docker
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "data_exchange", type: ExchangeType.Fanout);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequest request)
        {
            int id = _leaveRequests.LastOrDefault()?.Id??1;
            _leaveRequests.Add(new Leave()
            {
                Id = id,
                EmployeeEmail = request.EmployeeEmail,
                StartDate = request.StartDate,
                EndDate = request.EndDate,  
                Status = "Pending"
            });

            try
            {
                var json = JsonSerializer.Serialize(new LeaveEvent()
                {
                    Id = id,
                    status = "Pending"
                });

                await PublishData(json);
            }
            catch (Exception ex)
            {

            }

            return Ok(new { Message = "Leave request submitted." });
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var leave = _leaveRequests.FirstOrDefault(x=> x.Id == id);
            if (leave == null) return NotFound("Leave request not found.");

            leave.Status = "Approved";

            var json = JsonSerializer.Serialize(new LeaveEvent()
            {
                Id = id,
                status = "Approved"
            });

            await PublishData(json);

            return Ok(new { Message = "Leave Approved" });
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectLeave(int id)
        {
            var leave = _leaveRequests.FirstOrDefault(x => x.Id == id);
            if (leave == null) return NotFound("Leave request not found.");

            leave.Status = "Rejected";

            var json = JsonSerializer.Serialize(new LeaveEvent()
            {
                Id = id,
                status = "Rejected"
            });

            await PublishData(json);

            return Ok(new { Message = "Leave Rejected" });
        }

        private async Task PublishData(string data)
        {
            var body = Encoding.UTF8.GetBytes(data);

            _channel.BasicPublish(
                exchange: "data_exchange",
                routingKey: "",
                basicProperties: null,
                body: body
            );
        }
    }
}
