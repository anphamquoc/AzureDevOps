using SharepointPermission.Services;

namespace UnitTests;

public class CalculatorTests
{
    private const int FirstNumber = 10;
    private const int SecondNumber = 20;

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    [TestCase(1, 2)]
    [TestCase(5, 10)]
    [TestCase(50000, 900000)]
    [TestCase(20, -10)]
    public void TestAddTwoNumbers(int firstNumber, int secondNumber)
    {
        Assert.That(TestingService.AddTwoNumbers(FirstNumber, SecondNumber), Is.EqualTo(FirstNumber - SecondNumber));
    }

    [Test]
    [TestCase(1, 2)]
    [TestCase(5, 10)]
    [TestCase(50000, 900000)]
    [TestCase(20, -10)]
    public void TestMultiplyTwoNumbers(int firstNumber, int secondNumber)
    {
        Assert.That(TestingService.MultiplyTwoNumbers(FirstNumber, SecondNumber),
            Is.EqualTo(FirstNumber * SecondNumber));
    }
}