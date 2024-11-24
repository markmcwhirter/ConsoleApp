using System;
using System.Linq;
using System.Linq.Expressions;

class Program
{
    static void Main()
    {
        // Mock LINQ query
        var query = from EMP in tdv.EMPL_Data
                    join IDS in tdc.EMPL_AccountsIDs on EMP.Emp_PK equals IDS.FK_Emp_PK
                    where IDS.LoginID == objSysID.LoginID
                    select new { EMP.DisplayNameLFM };

        // Parse the query and generate the fluent version
        ParseAndGenerateFluent(query);
    }

    static void ParseAndGenerateFluent(IQueryable query)
    {
        // Extract query provider details
        var queryExpression = query.Expression;

        // Extract details and build the fluent query
        string fluentQuery = GenerateFluentQuery(queryExpression);

        // Display the generated fluent query
        Console.WriteLine("Generated Fluent Query:");
        Console.WriteLine(fluentQuery);
    }

    static string GenerateFluentQuery(Expression expression)
    {
        string fluentQuery = "";
        if (expression is MethodCallExpression methodCall)
        {
            // Process Join, Where, Select, etc.
            if (methodCall.Method.Name == "Join")
            {
                var outerTable = GetMemberName(methodCall.Arguments[0]);
                var innerTable = GetMemberName(methodCall.Arguments[1]);
                var outerKeySelector = ExtractLambdaBody(methodCall.Arguments[2]);
                var innerKeySelector = ExtractLambdaBody(methodCall.Arguments[3]);

                fluentQuery += $"{outerTable}.Join(\n    {innerTable},\n    {outerKeySelector},\n    {innerKeySelector},\n    (EMP, IDS) => new {{ EMP, IDS }}\n)";
            }
            else if (methodCall.Method.Name == "Where")
            {
                var condition = ExtractLambdaBody(methodCall.Arguments[1]);
                fluentQuery += $".Where({condition})";
            }
            else if (methodCall.Method.Name == "Select")
            {
                var projection = ExtractLambdaBody(methodCall.Arguments[1]);
                fluentQuery += $".Select({projection})";
            }

            // Recursive call for further processing
            foreach (var arg in methodCall.Arguments)
            {
                fluentQuery += GenerateFluentQuery(arg);
            }
        }
        return fluentQuery;
    }

    static string GetMemberName(Expression expression)
    {
        if (expression is MemberExpression member)
        {
            return member.Member.Name;
        }
        return "Unknown";
    }

    static string ExtractLambdaBody(Expression expression)
    {
        if (expression is LambdaExpression lambda)
        {
            return lambda.Body.ToString();
        }
        return "Unknown";
    }
}
