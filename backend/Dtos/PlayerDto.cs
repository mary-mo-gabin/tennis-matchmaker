namespace TennisMatchmaker.Dtos;

public record PlayerDto(
    int Id,
    string Name,
    string Gender,
    double SkillLevel,
    bool IsActive
);