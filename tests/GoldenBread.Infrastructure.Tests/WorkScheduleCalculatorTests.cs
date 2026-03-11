//using GoldenBread.Domain.Services;
//using Xunit;

//namespace GoldenBread.Infrastructure.Tests;

//public class WorkScheduleCalculatorTests
//{
//    private readonly WorkScheduleCalculator _calculator;

//    public WorkScheduleCalculatorTests()
//    {
//        _calculator = new WorkScheduleCalculator();
//    }

//    [Fact]
//    public void GetWorkStart_Returns9AM()
//    {
//        var date = new DateTime(2024, 3, 15, 14, 30, 0, DateTimeKind.Utc);

//        var result = _calculator.GetWorkStart(date);

//        Assert.Equal(new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void GetWorkEnd_Returns6PM()
//    {
//        var date = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc);

//        var result = _calculator.GetWorkEnd(date);

//        Assert.Equal(new DateTime(2024, 3, 15, 18, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Theory]
//    [InlineData(DayOfWeek.Monday, true)]
//    [InlineData(DayOfWeek.Tuesday, true)]
//    [InlineData(DayOfWeek.Wednesday, true)]
//    [InlineData(DayOfWeek.Thursday, true)]
//    [InlineData(DayOfWeek.Friday, true)]
//    [InlineData(DayOfWeek.Saturday, false)]
//    [InlineData(DayOfWeek.Sunday, false)]
//    public void IsWorkDay_ReturnsCorrectValue(DayOfWeek dayOfWeek, bool expected)
//    {
//        // 11 марта 2024 - понедельник
//        var baseDate = new DateTime(2024, 3, 11, 0, 0, 0, DateTimeKind.Utc);
//        var daysToAdd = ((int)dayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
//        var date = baseDate.AddDays(daysToAdd);

//        Assert.Equal(dayOfWeek, date.DayOfWeek);

//        var result = _calculator.IsWorkDay(date);

//        Assert.Equal(expected, result);
//    }

//    [Fact]
//    public void AddWorkDays_SkipsWeekends()
//    {
//        var start = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc); // Friday

//        var result = _calculator.AddWorkDays(start, 3);

//        // Friday + 3 work days = Wednesday (skip Sat/Sun)
//        // Friday → Monday (1), Tuesday (2), Wednesday (3)
//        Assert.Equal(new DateTime(2024, 3, 20, 9, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void AddWorkMinutes_SameDay_BeforeLunch()
//    {
//        var start = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc); // 9:00

//        var result = _calculator.AddWorkMinutes(start, 120); // +2 hours

//        Assert.Equal(new DateTime(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void AddWorkMinutes_CrossesLunch()
//    {
//        var start = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc); // 12:00 (1 hour before lunch)

//        var result = _calculator.AddWorkMinutes(start, 180); // +3 hours

//        // 12:00-13:00 = 60 min, lunch 13:00-14:00 skipped, 14:00-16:00 = 120 min → 16:00
//        Assert.Equal(new DateTime(2024, 3, 15, 16, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void AddWorkMinutes_SpansMultipleDays()
//    {
//        var start = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc); // Friday 9:00

//        var result = _calculator.AddWorkMinutes(start, 600); // +10 hours

//        // Friday: 9:00-18:00 = 8 hours (480 min) with lunch, need 120 more
//        // Monday: 9:00 + 120 min = 11:00
//        Assert.Equal(new DateTime(2024, 3, 18, 11, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void AddWorkMinutes_SkipsWeekend()
//    {
//        var start = new DateTime(2024, 3, 15, 16, 0, 0, DateTimeKind.Utc); // Friday 16:00

//        var result = _calculator.AddWorkMinutes(start, 180); // +3 hours

//        // Friday 16:00-18:00 = 2 hours, need 1 more → Monday 10:00
//        Assert.Equal(new DateTime(2024, 3, 18, 10, 0, 0, DateTimeKind.Utc), result);
//    }

//    [Fact]
//    public void CalculateWorkMinutes_SameDay()
//    {
//        var start = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc);
//        var end = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc);

//        var result = _calculator.CalculateWorkMinutes(start, end);

//        Assert.Equal(180, result); // 3 hours
//    }

//    [Fact]
//    public void CalculateWorkMinutes_CrossesLunch()
//    {
//        var start = new DateTime(2024, 3, 15, 11, 0, 0, DateTimeKind.Utc); // 11:00
//        var end = new DateTime(2024, 3, 15, 15, 0, 0, DateTimeKind.Utc);   // 15:00

//        var result = _calculator.CalculateWorkMinutes(start, end);

//        // 11:00-13:00 = 120 min, lunch skipped, 14:00-15:00 = 60 min → 180 min
//        Assert.Equal(180, result);
//    }

//    [Fact]
//    public void CalculateWorkMinutes_SpansMultipleDays()
//    {
//        var start = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc); // Friday
//        var end = new DateTime(2024, 3, 18, 12, 0, 0, DateTimeKind.Utc);  // Monday

//        var result = _calculator.CalculateWorkMinutes(start, end);

//        // Friday: 9:00-18:00 = 480 min, weekend skipped, Monday: 9:00-12:00 = 180 min → 660 min
//        Assert.Equal(660, result);
//    }
//}


