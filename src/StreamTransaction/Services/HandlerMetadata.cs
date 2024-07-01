using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wrench.StreamTransaction.Services;

internal class HandlerMetadata
{
    protected const string HandleAsyncMethodName = "HandleAsync";
    internal Type MessageType { get; set; }
    internal Type HandlerType { get; set; }

    public HandlerMetadata(Type messageType, Type handlerType)
    {
        MessageType = messageType;
        HandlerType = handlerType;
    }

    internal Delegate CreateDelegate(object instance)
    {
        _ = instance ?? throw new ArgumentNullException(nameof(instance));
        var methodInfo = HandlerType.GetMethod(HandleAsyncMethodName);

        if (methodInfo is null)
            throw new MissingMethodException(HandleAsyncMethodName);

        var parametersInfo = methodInfo.GetParameters();
        Expression[] exArgs = new Expression[parametersInfo.Length];

        List<ParameterExpression> parametersExpressions = new();

        for (int index = 0; index < exArgs.Length; index++)
        {
            exArgs[index] = Expression.Parameter(
                parametersInfo[index].ParameterType,
                parametersInfo[index].Name
            );
            parametersExpressions.Add((ParameterExpression)exArgs[index]);
        }

        MethodCallExpression callExpression = Expression.Call(
            Expression.Constant(instance),
            methodInfo,
            exArgs
        );

        LambdaExpression lambdaExpression = Expression.Lambda(
            callExpression,
            parametersExpressions
        );

        return lambdaExpression.Compile();
    }
}