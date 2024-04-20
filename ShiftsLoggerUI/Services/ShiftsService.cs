﻿using ShiftsLoggerUI.Helpers;
using ShiftsLoggerUI.Models;
using Spectre.Console;
using System.Text;
using System.Text.Json;

namespace ShiftsLoggerUI.Services;

internal static class ShiftsService
{
  internal static async Task<List<Shift>?> GetShifts(HttpClient client)
  {
    List<Shift>? shifts;
    Stream stream = await client.GetStreamAsync("https://localhost:7106/api/shifts");
    shifts = JsonSerializer.Deserialize<List<Shift>>(stream);

    if (shifts == null)
    {
      return [.. shifts];
    }

    return shifts;
  }

  internal static async Task CreateShift(HttpClient client)
  {
    string employeeName = UserInput.GetName();
    if (employeeName == "0") return;

    string startDateStr = UserInput.GetStartDate(employeeName);
    if (startDateStr == "0") return;

    string endDateStr = UserInput.GetEndDate(startDateStr, employeeName);
    if (endDateStr == "0") return;

    DateTime startDate = DateTimeParser.Parse(startDateStr);
    DateTime endDate = DateTimeParser.Parse(endDateStr);

    ShiftRequest shift = new(employeeName, startDate, endDate);
    var json = JsonSerializer.Serialize(shift);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    HttpRequestMessage req = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new Uri("https://localhost:7106/api/shifts"),
      Content = content,
    };

    HttpResponseMessage res = await client.SendAsync(req);

    if (!res.IsSuccessStatusCode) AnsiConsole.Markup("\n[red]Couldn't connect to ShiftLoggerAPI server. [/]\n");
    else AnsiConsole.Markup("\n[green]Shift added successfully![/]\n");

    return;
  }
}