namespace TennisMatchmaker.Dtos;

public record CreatePlayerDto(
    string Name, 
    string Gender, 
    double SkillLevel, 
    List<int> GroupIds
);