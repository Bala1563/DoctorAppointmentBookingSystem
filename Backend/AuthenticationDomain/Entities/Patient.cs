using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationDomain.Entities
{
    [Table("Patients")]
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; } // Male, Female, Other

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(20)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(10)]
        public string BloodGroup { get; set; }

        [MaxLength(200)]
        public string EmergencyContactName { get; set; }

        [MaxLength(20)]
        public string EmergencyContactPhone { get; set; }

        public string MedicalHistory { get; set; }

        public string Allergies { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual User User { get; set; }
    }
}
