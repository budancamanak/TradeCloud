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