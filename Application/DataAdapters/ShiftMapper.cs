using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataAdapters
{
    public static class ShiftMapper
    {
        public static ShiftDTO ToDTO(this Entities.Shift entityShift)
        {
            return new ShiftDTO
            {
                Id = entityShift.Id,
                EventId = entityShift.EventId,
                Name = entityShift.Name,
                StartTime = entityShift.StartTime,
                EndTime = entityShift.EndTime,
                RequiredStaff = entityShift.RequiredStaff,
                Description = entityShift.Description,
                Status = (ShiftStatusDTO)entityShift.Status,
                CreatedAt = entityShift.CreatedAt,
                UpdatedAt = entityShift.UpdatedAt
            };
        }

        public static Entities.Shift ToEntity(this ShiftDTO dtoShift)
        {
            return new Entities.Shift
            {
                Id = dtoShift.Id,
                EventId = dtoShift.EventId,
                Name = dtoShift.Name,
                StartTime = dtoShift.StartTime,
                EndTime = dtoShift.EndTime,
                RequiredStaff = dtoShift.RequiredStaff,
                Description = dtoShift.Description,
                Status = (Entities.ShiftStatus)dtoShift.Status,
                CreatedAt = dtoShift.CreatedAt,
                UpdatedAt = dtoShift.UpdatedAt
            };
        }

        public static List<ShiftDTO> ToDTOList(this List<Entities.Shift> entityShifts)
        {
            var dtoShifts = new List<ShiftDTO>();
            foreach (var entityShift in entityShifts)
            {
                dtoShifts.Add(entityShift.ToDTO());
            }
            return dtoShifts;
        }

        public static List<Entities.Shift> ToEntityList(this List<ShiftDTO> dtoShifts)
        {
            var entityShifts = new List<Entities.Shift>();
            foreach (var dtoShift in dtoShifts)
            {
                entityShifts.Add(dtoShift.ToEntity());
            }
            return entityShifts;
        }
    }
}
