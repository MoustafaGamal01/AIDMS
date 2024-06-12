namespace AIDMS.DTOs
{
    public class StudentNotificationDto
    {
        public byte[]? employeePicture { get; set; }
        public string employeeFirstName { get; set; }
        public string employeeLastName { get; set; }
        public string Message { get; set; }
        public string CreatedAt { get; set; }
    }
}
