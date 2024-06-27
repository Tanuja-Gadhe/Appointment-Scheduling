using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class AppointmentSchedulingContext : DbContext
{
    public AppointmentSchedulingContext()
    {
    }

    public AppointmentSchedulingContext(DbContextOptions<AppointmentSchedulingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAppointment> TblAppointments { get; set; }

    public virtual DbSet<TblAvailability> TblAvailabilities { get; set; }

    public virtual DbSet<TblChatroom> TblChatrooms { get; set; }

    public virtual DbSet<TblDoctorSpecialization> TblDoctorSpecializations { get; set; }

    public virtual DbSet<TblDoctorVerification> TblDoctorVerifications { get; set; }

    public virtual DbSet<TblDocument> TblDocuments { get; set; }

    public virtual DbSet<TblMessage> TblMessages { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSpecialization> TblSpecializations { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblWaitlist> TblWaitlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=AppointmentScheduling;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__tbl_appo__A50828FCA0AF9F38");

            entity.ToTable("tbl_appointment");

            entity.Property(e => e.AppointmentId)
                .ValueGeneratedNever()
                .HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDatetime)
                .HasColumnType("datetime")
                .HasColumnName("appointment_datetime");
            entity.Property(e => e.AppointmentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("appointment_status");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TblAppointmentDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_appoi__docto__4222D4EF");

            entity.HasOne(d => d.Patient).WithMany(p => p.TblAppointmentPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_appoi__patie__4316F928");
        });

        modelBuilder.Entity<TblAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId).HasName("PK__tbl_avai__86E3A80146BB8CFD");

            entity.ToTable("tbl_availability");

            entity.Property(e => e.AvailabilityId)
                .ValueGeneratedNever()
                .HasColumnName("availability_id");
            entity.Property(e => e.AppointmentCount).HasColumnName("appointment_count");
            entity.Property(e => e.AvailabilityEndDatetime)
                .HasColumnType("datetime")
                .HasColumnName("availability_end_datetime");
            entity.Property(e => e.AvailabilityStartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("availability_start_datetime");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TblAvailabilities)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_avail__docto__3F466844");
        });

        modelBuilder.Entity<TblChatroom>(entity =>
        {
            entity.HasKey(e => e.ChatroomId).HasName("PK__tbl_chat__74F85A66EE689DA9");

            entity.ToTable("tbl_chatroom");

            entity.Property(e => e.ChatroomId)
                .ValueGeneratedNever()
                .HasColumnName("chatroom_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TblChatroomDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_chatr__docto__45F365D3");

            entity.HasOne(d => d.Patient).WithMany(p => p.TblChatroomPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_chatr__patie__46E78A0C");
        });

        modelBuilder.Entity<TblDoctorSpecialization>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_doctor_specialization");

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");

            entity.HasOne(d => d.Doctor).WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_docto__docto__628FA481");

            entity.HasOne(d => d.Specialization).WithMany()
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_docto__speci__6383C8BA");
        });

        modelBuilder.Entity<TblDoctorVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_doct__3214EC078F428CD8");

            entity.ToTable("tbl_doctor_verification");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.VerificationDate).HasColumnType("datetime");
            entity.Property(e => e.VerifiedBy).HasMaxLength(128);

            entity.HasOne(d => d.Doctor).WithMany(p => p.TblDoctorVerifications)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__tbl_docto__Docto__6FE99F9F");
        });

        modelBuilder.Entity<TblDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__tbl_docu__9666E8AC8CEE8C53");

            entity.ToTable("tbl_document");

            entity.Property(e => e.DocumentId)
                .ValueGeneratedNever()
                .HasColumnName("document_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("document_type");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TblDocuments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_docum__docto__4CA06362");
        });

        modelBuilder.Entity<TblMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__tbl_mess__0BBF6EE60DA37BE6");

            entity.ToTable("tbl_message");

            entity.Property(e => e.MessageId)
                .ValueGeneratedNever()
                .HasColumnName("message_id");
            entity.Property(e => e.ChatroomId).HasColumnName("chatroom_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Reciever)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reciever");
            entity.Property(e => e.RecieverId).HasColumnName("reciever_id");
            entity.Property(e => e.Sender)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sender");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Chatroom).WithMany(p => p.TblMessages)
                .HasForeignKey(d => d.ChatroomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_messa__chatr__49C3F6B7");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tbl_role__760965CCEEB4ABAE");

            entity.ToTable("tbl_role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TblSpecialization>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("PK__tbl_spec__0E5BB6507D10F992");

            entity.ToTable("tbl_specialization");

            entity.Property(e => e.SpecializationId)
                .ValueGeneratedNever()
                .HasColumnName("specialization_id");
            entity.Property(e => e.DegreeName).HasColumnName("degree_name");
            entity.Property(e => e.SpecializationName)
                .HasMaxLength(50)
                .HasColumnName("specialization_name");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tbl_user__B9BE370FF3044C97");

            entity.ToTable("tbl_user");

            entity.HasIndex(e => e.UserName, "UQ_user_name").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserImage).HasColumnName("user_image");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_user__role_i__3B75D760");
        });

        modelBuilder.Entity<TblWaitlist>(entity =>
        {
            entity.HasKey(e => e.WaitlistId).HasName("PK__tbl_wait__620FAF914EA3D79C");

            entity.ToTable("tbl_waitlist");

            entity.Property(e => e.WaitlistId)
                .ValueGeneratedNever()
                .HasColumnName("waitlist_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.WaitlistDate).HasColumnName("waitlist_date");

            entity.HasOne(d => d.Appointment).WithMany(p => p.TblWaitlists)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_waitl__appoi__5070F446");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
