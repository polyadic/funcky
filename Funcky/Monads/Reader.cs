using System;
using System.Runtime.CompilerServices;

namespace Funcky.Monads
{
    public static class Reader
    {
        // Used to extract some value from a context
        public static Reader<TService> GetService<TService>() =>
            Reader<TService>.Read<IServiceProvider>(serviceProvider => (TService)serviceProvider.GetService(typeof(TService)));
    }

    [AsyncMethodBuilder(typeof(ReaderTaskMethodBuilder<>))]
    public class Reader<T> : INotifyCompletion, IReader
    {
        // Used by ReaderTaskMethodBuilder in a compiler generated code
        internal Reader()
        {
        }

        // Used to extract some value from a context
        public static Reader<T> Read<TContext>(Func<TContext, T> extractor) => new Reader<T>(context => Extract(context, extractor));

        private Reader(Func<object, T> exec) => _extractor = exec;

        public bool IsCompleted { get; private set; }

        private readonly Func<object, T> _extractor;

        private object _context;

        private Action _continuation;

        private T _result;

        private Exception _exception;

        private IReader _child;

        public Reader<T> GetAwaiter() => this;

        public Reader<T> Apply(object context)
        {
            if (_context != null)
            {
                throw new Exception("Another context is already applied to the reader");
            }

            SetContext(context);
            return this;
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                continuation();
            }
            else
            {
                if (_continuation != null)
                {
                    throw new Exception("Only a single async continuation is allowed");
                }

                _continuation = continuation;
            }
        }

        public T GetResult()
        {
            if (_exception != null)
            {
                // ExceptionDispatchInfo.Throw(_exception);
            }

            if (!IsCompleted)
            {
                throw new Exception("Not Completed");
            }

            return _result;
        }

        internal void SetResult(T result)
        {
            _result = result;
            IsCompleted = true;
            _continuation?.Invoke();
        }

        internal void SetException(Exception exception)
        {
            _exception = exception;
            IsCompleted = true;
            _continuation?.Invoke();
        }

        internal void SetChild(IReader reader)
        {
            _child = reader;
            if (_context != null)
            {
                _child.SetContext(_context);
            }
        }

        public void SetContext(object context)
        {
            _context = context;
            if (_context != null)
            {
                _child?.SetContext(_context);

                if (_extractor != null)
                {
                    SetResult(_extractor(_context));
                }
            }
        }

        private static T Extract<TContext>(object context, Func<TContext, T> extractor)
        {
            if (extractor == null)
            {
                throw new Exception("Some extracting function should be defined");
            }

            if (context is TContext tContext)
            {
                return extractor(tContext);
            }

            throw new Exception($"Could not cast the passed context to type '{typeof(TContext).Name}'");
        }
    }

    public class ReaderTaskMethodBuilder<T>
    {
        public ReaderTaskMethodBuilder() => Task = new Reader<T>();

        public static ReaderTaskMethodBuilder<T> Create() => new ReaderTaskMethodBuilder<T>();

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
            => stateMachine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetException(Exception exception) => this.Task.SetException(exception);

        public void SetResult(T result) => this.Task.SetResult(result);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            GenericAwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            GenericAwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void GenericAwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is IReader reader)
            {
                Task.SetChild(reader);
            }

            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public Reader<T> Task { get; }
    }
}
