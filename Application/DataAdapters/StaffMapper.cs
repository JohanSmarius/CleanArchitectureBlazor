using System.Collections.Generic;

namespace Application.DataAdapters
{
    public static class StaffMapper
    {
        public static StaffDTO ToDTO(this Entities.Staff entityStaff)
        {
            return new StaffDTO
            {
                Id = entityStaff.Id,
                FirstName = entityStaff.FirstName,
                LastName = entityStaff.LastName,
                Email = entityStaff.Email,
                Phone = entityStaff.Phone,
                Role = (StaffRoleDTO)entityStaff.Role,
                IsActive = entityStaff.IsActive,
                CertificationLevel = entityStaff.CertificationLevel,
                CertificationExpiry = entityStaff.CertificationExpiry,
                CreatedAt = entityStaff.CreatedAt,
                UpdatedAt = entityStaff.UpdatedAt,
                Birthday = entityStaff.Birthday,
                StaffAssignments = entityStaff.StaffAssignments.ToDTOList()
            };
        }

        public static Entities.Staff ToEntity(this StaffDTO dtoStaff)
        {
            return new Entities.Staff
            {
                Id = dtoStaff.Id,
                FirstName = dtoStaff.FirstName,
                LastName = dtoStaff.LastName,
                Email = dtoStaff.Email,
                Phone = dtoStaff.Phone,
                Role = (Entities.StaffRole)dtoStaff.Role,
                IsActive = dtoStaff.IsActive,
                CertificationLevel = dtoStaff.CertificationLevel,
                CertificationExpiry = dtoStaff.CertificationExpiry,
                CreatedAt = dtoStaff.CreatedAt,
                UpdatedAt = dtoStaff.UpdatedAt,
                Birthday = dtoStaff.Birthday,
                StaffAssignments = dtoStaff.StaffAssignments.ToEntityList()
            };
        }

        public static List<StaffDTO> ToDTOList(this List<Entities.Staff> entityStaff)
        {
            var dtoStaff = new List<StaffDTO>();
            foreach (var staff in entityStaff)
            {
                dtoStaff.Add(staff.ToDTO());
            }
            return dtoStaff;
        }

        public static List<Entities.Staff> ToEntityList(this List<StaffDTO> dtoStaff)
        {
            var entityStaff = new List<Entities.Staff>();
            foreach (var staff in dtoStaff)
            {
                entityStaff.Add(staff.ToEntity());
            }
            return entityStaff;
        }
    }
}
