﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACS.EFMODEL.DataModels
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ACSEntities : DbContext
    {
        public ACSEntities()
            : base("name=ACSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ACS_APPLICATION> ACS_APPLICATION { get; set; }
        public DbSet<ACS_APPLICATION_ROLE> ACS_APPLICATION_ROLE { get; set; }
        public DbSet<ACS_CONTROL> ACS_CONTROL { get; set; }
        public DbSet<ACS_CONTROL_ROLE> ACS_CONTROL_ROLE { get; set; }
        public DbSet<ACS_CREDENTIAL_DATA> ACS_CREDENTIAL_DATA { get; set; }
        public DbSet<ACS_MODULE> ACS_MODULE { get; set; }
        public DbSet<ACS_MODULE_GROUP> ACS_MODULE_GROUP { get; set; }
        public DbSet<ACS_MODULE_ROLE> ACS_MODULE_ROLE { get; set; }
        public DbSet<ACS_OTP> ACS_OTP { get; set; }
        public DbSet<ACS_ROLE> ACS_ROLE { get; set; }
        public DbSet<ACS_ROLE_BASE> ACS_ROLE_BASE { get; set; }
        public DbSet<ACS_ROLE_USER> ACS_ROLE_USER { get; set; }
        public DbSet<ACS_TOKEN> ACS_TOKEN { get; set; }
        public DbSet<ACS_USER> ACS_USER { get; set; }
        public DbSet<V_ACS_APPLICATION_ROLE> V_ACS_APPLICATION_ROLE { get; set; }
        public DbSet<V_ACS_CONTROL_ROLE> V_ACS_CONTROL_ROLE { get; set; }
        public DbSet<V_ACS_MODULE> V_ACS_MODULE { get; set; }
        public DbSet<V_ACS_MODULE_ROLE> V_ACS_MODULE_ROLE { get; set; }
        public DbSet<V_ACS_ROLE_BASE> V_ACS_ROLE_BASE { get; set; }
        public DbSet<V_ACS_ROLE_USER> V_ACS_ROLE_USER { get; set; }
    }
}
