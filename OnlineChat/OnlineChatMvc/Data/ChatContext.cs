﻿using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace OnlineChatMvc.Data
{
    public class ChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {

        }

    }

}
