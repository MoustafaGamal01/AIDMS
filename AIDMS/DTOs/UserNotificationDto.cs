namespace AIDMS.DTOs
{
    public class UserNotificationDto
    {
        public byte[]? userPicture { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string Message { get; set; }
        public string CreatedAt { get; set; }
    }
}
