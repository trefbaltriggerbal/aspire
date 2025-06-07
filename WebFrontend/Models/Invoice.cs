namespace Projects.WebFrontend.Models;

public record Invoice(int Id, int PolicyId, decimal Amount, bool Paid);
