namespace TipsAndSteps.Content.Domain.Enums;

/// <summary>The 11 content sections of the Tips &amp; Steps platform (خطوات ونصائح)</summary>
public enum ContentSection
{
    [System.Runtime.Serialization.EnumMember(Value = "behavioral")]
    Behavioral = 1,
    
    [System.Runtime.Serialization.EnumMember(Value = "psychological")]
    Psychological = 2,
    
    [System.Runtime.Serialization.EnumMember(Value = "nutrition")]
    Nutrition = 3,
    
    [System.Runtime.Serialization.EnumMember(Value = "sexual-education")]
    SexualEducation = 4,
    
    [System.Runtime.Serialization.EnumMember(Value = "educational-games")]
    EducationalGames = 5,
    
    [System.Runtime.Serialization.EnumMember(Value = "hospitals")]
    Hospitals = 6,
    
    [System.Runtime.Serialization.EnumMember(Value = "health-units")]
    HealthUnits = 7,
    
    [System.Runtime.Serialization.EnumMember(Value = "emergency")]
    Emergency = 8,
    
    [System.Runtime.Serialization.EnumMember(Value = "vaccines")]
    Vaccines = 9,
    
    [System.Runtime.Serialization.EnumMember(Value = "questionnaires")]
    Questionnaires = 10,
    
    [System.Runtime.Serialization.EnumMember(Value = "faqs")]
    Faqs = 11
}
