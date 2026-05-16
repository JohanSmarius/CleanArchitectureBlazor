namespace ClientApplication.DataAdapters
{
    public static class StaffAssignmentMapper
    {
        public static StaffAssignmentDTO ToDTO(this Entities.StaffAssignment entityAssignment)
        {
            return new StaffAssignmentDTO
            {
                Id = entityAssignment.Id,
                ShiftId = entityAssignment.ShiftId,
                StaffId = entityAssignment.StaffId,
                Status = entityAssignment.Status,
                CheckInTime = entityAssignment.CheckInTime,
                CheckOutTime = entityAssignment.CheckOutTime,
                Notes = entityAssignment.Notes
            };
        }

        public static Entities.StaffAssignment ToEntity(this StaffAssignmentDTO dtoAssignment)
        {
            return new Entities.StaffAssignment
            {
                Id = dtoAssignment.Id,
                ShiftId = dtoAssignment.ShiftId,
                StaffId = dtoAssignment.StaffId,
                Status = dtoAssignment.Status,
                CheckInTime = dtoAssignment.CheckInTime,
                CheckOutTime = dtoAssignment.CheckOutTime,
                Notes = dtoAssignment.Notes
            };
        }

        public static List<StaffAssignmentDTO> ToDTOList(this List<Entities.StaffAssignment> entityAssignments)
        {
            var dtoAssignments = new List<StaffAssignmentDTO>();
            foreach (var assignment in entityAssignments)
            {
                dtoAssignments.Add(assignment.ToDTO());
            }
            return dtoAssignments;
        }

        public static List<Entities.StaffAssignment> ToEntityList(this List<StaffAssignmentDTO> dtoAssignments)
        {
            var entityAssignments = new List<Entities.StaffAssignment>();
            foreach (var assignment in dtoAssignments)
            {
                entityAssignments.Add(assignment.ToEntity());
            }
            return entityAssignments;
        }
    }
}
