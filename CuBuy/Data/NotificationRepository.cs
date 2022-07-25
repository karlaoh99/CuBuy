using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CuBuy.Models;
using System;
using CuBuy.Data;

namespace CuBuy.Data
{
    public interface INotificationRepository
    {
        void AddNotification(Notification n);
        IEnumerable<Notification> GetByUser(string userId);

        void Remove(Notification n);
    }


    public class NotificationRepositoryEF : INotificationRepository
    {
        private AppDbContext context;

        public NotificationRepositoryEF(AppDbContext context) => this.context = context;

        public void AddNotification(Notification n)
        {
            context.Notifications.Add(n);
            context.SaveChanges();
        }

        public void Remove(Notification n){
            context.Notifications.Remove(n);
        }

        public IEnumerable<Notification> GetByUser(string userId)
        {
            return context.Notifications.Where(n => n.User.Id == userId);
        }
    }
}