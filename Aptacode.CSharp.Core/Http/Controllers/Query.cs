using System;
using System.Linq.Expressions;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    public delegate Expression<Func<T, bool>> Query<T>();
}