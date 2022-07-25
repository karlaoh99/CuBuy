using CuBuy.Models;
using System;

namespace CuBuy.Models
{
    public enum NotificationType { Success, Warning, Error }
    
    public class Notification
    {
        public Guid id {get; set;}
        public AppUser User {get; set;}
        public string Message {get; set;}
        public string Link {get; set;}
        public NotificationType Type {get; set;}
        public DateTime DateTime {get; set;}
    }
}