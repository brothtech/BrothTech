using BrothTech.Sql.Database.Model;

namespace BrothTech.Sql.Database.Builders;

public class PrimaryKeyColumnBuilder(
    TableBuilder tableBuilder, 
    string name,
    bool isClustered) :
    ColumnBuilder<PrimaryKeyColumn, PrimaryKeyColumn>(tableBuilder),
    IRequireType<PrimaryKeyColumn, PrimaryKeyColumn>
{
    private readonly string _name = name;
    private readonly bool _isClustered = isClustered;
    
    protected override PrimaryKeyColumn CreateImplementation(
        ISqlType type)
    {
        return new PrimaryKeyColumn
        {
            Name = _name,
            IsClustered = _isClustered,
            Type = type
        };
    }
}