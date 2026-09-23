namespace TennisMatchmaker.Dtos;

public record UpdatePlayerDto(
    string Name, 
    string Gender, 
    double SkillLevel
);