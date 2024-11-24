













using System;
using System.Linq;
using System.Linq.Expressions;

class Program
{
    static void Main()
    {
        // Mock LINQ query
        var query = from EMP in tdv.EMPL_Data
                    join IDS in tdc.EMPL_AccountsIDs on EMP.Emp_PK equals IDS.FK_Emp_PK into joined
                    from IDS in joined.DefaultIfEmpty() // Simulate left outer join
                    where IDS.LoginID == objSysID.LoginID
                    select new { EMP.DisplayNameLFM, IDS?.LoginID };

        // Parse and generate both the left and right outer join
        ParseAndGenerateFluentWithJoins(query);
    }

    static void ParseAndGenerateFluentWithJoins(IQueryable query)
    {
        // Extract query expression
        var queryExpression = query.Expression;

        // Generate fluent queries
        string fluentQuery = GenerateFluentQuery(queryExpression);
        string rightOuterJoinQuery = ConvertToRightOuterJoin(fluentQuery);

        // Display results
        Console.WriteLine("Generated Fluent Query (Left Join):");
        Console.WriteLine(fluentQuery);

        Console.WriteLine("\nGenerated Fluent Query (Right Join):");
        Console.WriteLine(rightOuterJoinQuery);
    }

    static string GenerateFluentQuery(Expression expression)
    {
        string fluentQuery = "";
        if (expression is MethodCallExpression methodCall)
        {
            if (methodCall.Method.Name == "GroupJoin")
            {
                var outerTable = GetMemberName(methodCall.Arguments[0]);
                var innerTable = GetMemberName(methodCall.Arguments[1]);
                var outerKeySelector = ExtractLambdaBody(methodCall.Arguments[2]);
                var innerKeySelector = ExtractLambdaBody(methodCall.Arguments[3]);

                fluentQuery += $"{outerTable}.GroupJoin(\n    {innerTable},\n    {outerKeySelector},\n    {innerKeySelector},\n    (EMP, IDS) => new {{ EMP, IDS }}\n)";
            }
            else if (methodCall.Method.Name == "SelectMany")
            {
                fluentQuery += ".SelectMany(\n    joined => joined.IDS.DefaultIfEmpty(),\n    (EMP, IDS) => new { EMP, IDS }\n)";
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

            // Recursive processing for further arguments
            foreach (var arg in methodCall.Arguments)
            {
                fluentQuery += GenerateFluentQuery(arg);
            }
        }
        return fluentQuery;
    }

    static string ConvertToRightOuterJoin(string fluentQuery)
    {
        // Simulate right outer join by swapping outer and inner collections
        if (fluentQuery.Contains(".GroupJoin("))
        {
            fluentQuery = fluentQuery.Replace(".GroupJoin(", ".GroupJoin(\n    SWAPPED_");
            fluentQuery = fluentQuery.Replace("(EMP, IDS)", "(IDS, EMP)");
        }
        if (fluentQuery.Contains(".SelectMany("))
        {
            fluentQuery = fluentQuery.Replace("(EMP, IDS)", "(IDS, EMP)");
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
