namespace TrackManagement.Application.DTOs;

public record DspDto(int Id, string Name);

public record TrackDistributionDto(
    int Id,
    int DspId,
    string DspName,
    DateTime SubmittedAt,
    string Status);
