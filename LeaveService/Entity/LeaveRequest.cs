namespace LeaveService.Entity
{
    public class Leave
    {
        public int Id { get; set; }
        public string EmployeeEmail { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
    public class LeaveRequest
    {
        public string EmployeeEmail { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
