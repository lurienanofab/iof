using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IOF
{
    public interface IExcelService
    {
        IExcelExport CreateExport();
    }

    public interface IExcelExport
    {
        IWorksheet<T> AddWorkSheet<T>(IEnumerable<T> items, string name);
        byte[] GetBytes();
    }

    public interface IWorksheet<T>
    {
        void Sum<TProperty>(Expression<Func<T, TProperty>> expression);
        void Format<TProperty>(Expression<Func<T, TProperty>> expression, string format);
        void FormatCurrency<TProperty>(Expression<Func<T, TProperty>> expression);
    }
}
