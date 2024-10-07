using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddTransient<CalcService>();
builder.Services.AddTransient<TimeService>();

var app = builder.Build();


app.MapGet("/calc", async context =>
{
    var htmlForm = @"
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>�����������</title>
        </head>
        <body>
            <h1>�����������</h1>
            <form method='post' action='/calc'>
                <label>����� 1: </label><input type='number' name='a' required /><br/>
                <label>����� 2: </label><input type='number' name='b' required /><br/>
                <label>��������: </label>
                <select name='operation'>
                    <option value='add'>���������</option>
                    <option value='subtract'>³�������</option>
                    <option value='multiply'>��������</option>
                    <option value='divide'>ĳ�����</option>
                </select><br/>
                <input type='submit' value='���������'/>
            </form>
        </body>
        </html>";
    await context.Response.WriteAsync(htmlForm);
});


app.MapPost("/calc", async context =>
{
    var form = await context.Request.ReadFormAsync();
    int a = int.Parse(form["a"]);
    int b = int.Parse(form["b"]);
    string operation = form["operation"];

    var calcService = context.RequestServices.GetRequiredService<CalcService>();

    int result = operation switch
    {
        "add" => calcService.Add(a, b),
        "subtract" => calcService.Subtract(a, b),
        "multiply" => calcService.Multiply(a, b),
        "divide" => calcService.Divide(a, b),
        _ => throw new ArgumentException("������� ��������")
    };

    var htmlResult = $@"
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>�����������</title>
        </head>
        <body>
            <h1>�����������</h1>
            <form method='post' action='/calc'>
                <label>����� 1: </label><input type='number' name='a' required /><br/>
                <label>����� 2: </label><input type='number' name='b' required /><br/>
                <label>��������: </label>
                <select name='operation'>
                    <option value='add'>���������</option>
                    <option value='subtract'>³�������</option>
                    <option value='multiply'>��������</option>
                    <option value='divide'>ĳ�����</option>
                </select><br/>
                <input type='submit' value='���������'/>
            </form>
            <h2>���������: {result}</h2>
        </body>
        </html>";

    await context.Response.WriteAsync(htmlResult);
});


app.MapGet("/time", async context =>
{
    var timeService = context.RequestServices.GetRequiredService<TimeService>();
    string currentTimeOfDay = timeService.GetCurrentTimeOfDay();
    var currentTime = DateTime.Now.ToString("HH:mm");
    var htmlContent = $@"
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>��� ����</title>
        </head>
        <body>
            <h2>�������� ���: {currentTime}</h2>
            <h2>�����: {currentTimeOfDay}</h2>
        </body>
        </html>";

    await context.Response.WriteAsync(htmlContent);
});

app.Run();


public class CalcService
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;
    public int Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("�� ����� ����� �� ����.");
        return a / b;
    }
}


public class TimeService
{
    public string GetCurrentTimeOfDay()
    {
        var currentHour = DateTime.Now.Hour;

        if (currentHour >= 6 && currentHour < 12)
        {
            return "�����";
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            return "����";
        }
        else if (currentHour >= 18 && currentHour < 24)
        {
            return "�����";
        }
        else
        {
            return "����� ��";
        }
    }
}
