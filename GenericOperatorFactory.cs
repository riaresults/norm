
using System;
using System.Reflection.Emit;
using System.Reflection;

namespace D10.Norm
{
    internal static class GenericOperatorFactory<TLeft, TRight, TResult, TOwner>
    {
        private static Func<TLeft, TRight, TResult> _add;
        private static Func<TLeft, TRight, TResult> _subtract;
        private static Func<TLeft, TRight, TResult> _multiply;
        private static Func<TLeft, TRight, TResult> _divide;
        private static Func<TLeft, TRight, TResult> _and;
        private static Func<TLeft, TRight, TResult> _or;
        private static Func<TLeft, TRight, TResult> _xor;
        private static Func<TLeft, TRight, TResult> _not;

        private static Func<TLeft, TRight, TResult> CreateBinaryExpression(string name, Action<ILGenerator> something)
        {
            DynamicMethod method = new DynamicMethod(
                name + ":" + typeof(TLeft) + ":" + typeof(TRight) + ":" + typeof(TResult) + ":" + typeof(TOwner),
                typeof(TLeft),
                new Type[] { typeof(TLeft), typeof(TRight) },
                typeof(TOwner)
            );

            ILGenerator generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);

            something(generator);

            generator.Emit(OpCodes.Ret);

            return (Func<TLeft, TRight, TResult>)method.CreateDelegate(typeof(Func<TLeft, TRight, TResult>));
        }

        public static Func<TLeft, TRight, TResult> Add
        {
            get
            {
                if (_add == null)
                {
                    _add = CreateBinaryExpression("op_Addition",
                      delegate(ILGenerator generator)
                      {
                          if (typeof(TLeft).IsPrimitive)
                          {
                              generator.Emit(OpCodes.Add);
                          }
                          else
                          {
                              MethodInfo info = typeof(TLeft).GetMethod(
                                "op_Addition",
                                new Type[] { typeof(TLeft), typeof(TRight) },
                                null
                                );

                              generator.EmitCall(OpCodes.Call, info, null);
                          }
                      });
                }

                return _add;
            }
        }

        public static Func<TLeft, TRight, TResult> Subtract
        {
            get
            {
                if (_subtract == null)
                {
                    _subtract = CreateBinaryExpression("op_Subtraction",
                      delegate(ILGenerator generator)
                      {
                          if (typeof(TLeft).IsPrimitive)
                          {
                              generator.Emit(OpCodes.Sub);
                          }
                          else
                          {
                              MethodInfo info = typeof(TLeft).GetMethod(
                                  "op_Subtraction",
                                  new Type[] { typeof(TLeft), typeof(TRight) },
                                  null
                              );

                              generator.EmitCall(OpCodes.Call, info, null);
                          }
                      });
                }

                return _subtract;
            }
        }

        public static Func<TLeft, TRight, TResult> Multiply
        {
            get
            {
                if (_multiply == null)
                {
                    _multiply = CreateBinaryExpression("op_Multiply",
                      delegate(ILGenerator generator)
                      {
                          if (typeof(TLeft).IsPrimitive)
                          {
                              generator.Emit(OpCodes.Mul);
                          }
                          else
                          {
                              MethodInfo info = typeof(TLeft).GetMethod(
                                "op_Multiply",
                                new Type[] { typeof(TLeft), typeof(TRight) },
                                null
                                );

                              generator.EmitCall(OpCodes.Call, info, null);
                          }
                      });
                }

                return _multiply;
            }
        }

        public static Func<TLeft, TRight, TResult> Divide
        {
            get
            {
                if (_divide == null)
                {
                    _divide = CreateBinaryExpression("op_Devision",
                      delegate(ILGenerator generator)
                      {
                          if (typeof(TLeft).IsPrimitive)
                          {
                              generator.Emit(OpCodes.Div);
                          }
                          else
                          {
                              MethodInfo info = typeof(TLeft).GetMethod(
                                  "op_Devision",
                                  new Type[] { typeof(TLeft), typeof(TRight) },
                                  null
                              );

                              generator.EmitCall(OpCodes.Call, info, null);
                          }
                      });
                }

                return _divide;
            }
        }

        public static Func<TLeft, TRight, TResult> And
        {
            get
            {
                if (_and == null)
                    _and = CreateBinaryExpression("And", delegate(ILGenerator generator) { generator.Emit(OpCodes.And); });

                return _and;
            }
        }

        public static Func<TLeft, TRight, TResult> Or
        {
            get
            {
                if (_or == null)
                    _or = CreateBinaryExpression("Or", delegate(ILGenerator generator) { generator.Emit(OpCodes.Or); });

                return _or;
            }
        }

        public static Func<TLeft, TRight, TResult> Xor
        {
            get
            {
                if (_xor == null)
                    _xor = CreateBinaryExpression("Xor", delegate(ILGenerator generator) { generator.Emit(OpCodes.Xor); });

                return _xor;
            }
        }

        public static Func<TLeft, TRight, TResult> Not
        {
            get
            {
                if (_not == null)
                    _not = CreateBinaryExpression("Not", delegate(ILGenerator generator) { generator.Emit(OpCodes.Not); });

                return _not;
            }
        }
    }
}