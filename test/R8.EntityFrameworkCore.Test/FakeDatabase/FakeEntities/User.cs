﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.ComponentModel.DataAnnotations;
using R8.EntityFrameworkCore.EntityBases;

namespace R8.EntityFrameworkCore.Test.FakeDatabase.FakeEntities
{
    public class User : EntityBase<User>
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public Guid? RoleId { get; set; }
        public virtual Role Role { get; set; }

        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId);
        }
    }
}