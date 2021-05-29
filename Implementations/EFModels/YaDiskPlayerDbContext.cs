﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations.EFModels
{
    public class YaDiskPlayerDbContext: DbContext
    {
        public YaDiskPlayerDbContext(DbContextOptions<YaDiskPlayerDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}