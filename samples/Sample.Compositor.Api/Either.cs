namespace Sample.Compositor.Api;

public record Error(string Source, string Message);

public record Either<TLeft, TRight>
{
    private readonly TLeft _left;
    private readonly TRight _right;
    private bool _isLeft;

    public Either(TLeft left)
    {
        _left = left;
        _isLeft = true;
    }

    public Either(TRight right)
    {
        _right = right;
        _isLeft = false;
    }

    public void When(Action<TLeft> success, Action<TRight> error)
    {
        if (_isLeft)
            success(_left);
        else
            error(_right);
    }

    public T When<T>(Func<TLeft, T> success, Func<TRight, T> error)
    {
        return _isLeft ? success(_left) : error(_right);
    }

    public void IfSuccess(Action<TLeft> action)
    {
        if (_isLeft)
            action(_left);
    }

    public void IfError(Action<TRight> action)
    {
        if (!_isLeft)
            action(_right);
    }

    public static Either<TLeft, TRight> Success(TLeft data)
    {
        return new Either<TLeft, TRight>(data);
    }

    public static Either<TLeft, TRight> Fail(TRight data)
    {
        return new Either<TLeft, TRight>(data);
    }
}