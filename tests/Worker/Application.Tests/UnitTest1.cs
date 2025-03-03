using Common.Plugin.Models;
using FluentAssertions;

namespace Application.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        // var param = new RangeValueParameter<Int32>(1, 1, 5, "test", 0);
        var param = NumericParameter<int>.IntParameter("Test parameter", 1);
        param.Value.Should().Be(1);
        param.Name.Should().Be("Test parameter");
    }

    [Test]
    public void TestRangeParameter()
    {
        int min = 1, max = 5, inc = 1, val = min;
        var param = NumericParameter<int>.IntParameter("Test", val, min, max, inc);
        using var enumerator = param.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Should().Be(val);
            val += param.Increment;
        }
    }

    [Test]
    public void TestListStringParameter()
    {
        var list = new List<string> { "A1", "A2", "A3" };
        var param = ListParameter<string>.StringParameter("Test String parameter", 0, "A1", "A2", "A3");
        using var enumerator = param.GetEnumerator();
        int index = 0;
        while (enumerator.MoveNext())
        {
            enumerator.Current.Should().Be(list[index]);
            index++;
        }
    }

    [Test]
    public void TestListIntegerParameter()
    {
        var list = new List<int> { 11, 12, 13, 14, 15 };
        var param = ListParameter<int>.IntParameter("Test String parameter", 0, 11, 12, 13, 14, 15);
        using var enumerator = param.GetEnumerator();
        int index = 0;
        while (enumerator.MoveNext())
        {
            enumerator.Current.Should().Be(list[index]);
            index++;
        }
    }

    [Test]
    public void TestGenerateParametersForPlugin()
    {
        /***
         * We will have a json of object.
         * This json will have a bunch of parameters in it.
         * Json object type will be IPluginParamset
         * IPluginParamset will have parameters that are required for plugin to run.
         *
         * json will be turned to ipluginparamset
         * using each parameter -> cartesian product will be computed.
         * for each cartesian product, plugin parameters will be generated.
         */

        Assert.Pass();
    }
}