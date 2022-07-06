﻿using Core.Domain.ApplicationModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Transversal.Extensions;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users,Privileges,string,IdentityUserClaim<string>, UsersPrivileges,IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly)
                .SetPropertyDefaultSqlValue("CreateDate", "getdate()")
                .SetPropertyDefaultValue<bool>("Active", true)
                .SetPropertyQueryFilter("Active", true);
        }

        public override int SaveChanges()
        {
            SetUpdateDateOnModifiedEntries();
            CheckDeleteRoleBase();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUpdateDateOnModifiedEntries();
            CheckDeleteRoleBase();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUpdateDateOnModifiedEntries()
        {
            var modifiedEntries = ChangeTracker
                .Entries()
                .Where(e => e.Metadata.FindProperty("UpdateDate") != null &&
                            e.State == EntityState.Modified);

            foreach (var modifiedEntry in modifiedEntries)
            {
                modifiedEntry.Property("UpdateDate").CurrentValue = DateTime.Now;
            }
        }

        private void CheckDeleteRoleBase()
        {
            var modifiedEntries = ChangeTracker
                .Entries()
                .Where(e => e.Metadata.FindProperty("NormalizedName") != null &&
                            e.Entity is Privileges &&
                            e.State == EntityState.Deleted);

            foreach (var modifiedEntry in modifiedEntries)
            {
                var role = modifiedEntry.Property("NormalizedName")?.CurrentValue?.ToString() ?? null;
                if (role == "ADMINISTRADOR" || role == "USER")
                {
                    throw new Exception($"No se puede eliminar el rol {role} porque es un rol base del sistema.");
                }
            }
        }
    }
}
