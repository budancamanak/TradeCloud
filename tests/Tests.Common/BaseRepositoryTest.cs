using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Tests.Common.Database;

namespace Tests.Common;

[TestFixture]
public abstract class BaseRepositoryTest<T, TFixture, TRepo, TValidator>(string dbName) : AbstractLoggableTest
    where T : DbContext
    where TFixture : DatabaseFixture<T>
    where TValidator : IValidator
{
    protected TFixture Fixture;
    protected T DbContext;
    protected TRepo Repository;
    protected ILogger<TRepo> Logger;


    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        Logger = _loggerFactory.CreateLogger<TRepo>();
        var validator = (TValidator)Activator.CreateInstance(typeof(TValidator));
        Fixture = Activator.CreateInstance(typeof(TFixture), dbName) as TFixture;
        Fixture.SeedData();
        DbContext = Fixture.DbContext;
        Repository = (TRepo)Activator.CreateInstance(typeof(TRepo), DbContext, validator,Logger);
    }

    [TearDown]
    public virtual void Dispose()
    {
        Fixture.Dispose();
    }
}