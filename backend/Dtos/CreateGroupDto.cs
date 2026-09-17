namespace TennisMatchmaker.Dtos;

public record CreateGroupDto(
    string Name,
    int LeaderId
);