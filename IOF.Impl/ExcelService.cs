using System.Drawing;
using System.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;
using System.Linq.Expressions;

namespace IOF.Impl
{
    public class ExcelService : IExcelService
    {
        public IExcelExport CreateExport()
        {
            return new ExcelExport();
        }
    }

    public class ExcelExport : IExcelExport
    {
        ExcelPackage _pack;

        internal ExcelExport()
        {
            _pack = new ExcelPackage();
        }

        public IWorksheet<T> AddWorkSheet<T>(IEnumerable<T> items, string name)
        {
            var ws = _pack.Workbook.Worksheets.Add(name);
            return new Worksheet<T>(ws, items);
        }

        public byte[] GetBytes()
        {
            return _pack.GetAsByteArray();
        }
    }

    public class Worksheet<T> : IWorksheet<T>
    {
        private readonly ExcelWorksheet _ws;
        private readonly IEnumerable<T> _items;
        private readonly System.Reflection.PropertyInfo[] _props;

        private int _cols;
        private int _rows;

        private string _header;

        internal Worksheet(ExcelWorksheet ws, IEnumerable<T> items)
        {
            _ws = ws;
            _items = items;

            _props = typeof(T).GetProperties();

            _cols = 1;
            _rows = 1;

            foreach (var p in _props)
            {
                var cell = ws.Cells[_rows, _cols];
                cell.Value = p.Name;
                _cols++;
            }

            _header = ExcelCellBase.GetAddress(1, 1, 1, _cols - 1);
            _ws.Cells[_header].Style.Fill.PatternType = ExcelFillStyle.Solid;
            _ws.Cells[_header].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5078b3"));
            _ws.Cells[_header].Style.Font.Color.SetColor(Color.White);
            _ws.Cells[_header].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            _ws.Cells[_header].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            _ws.Cells[_header].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            _ws.Cells[_header].Style.Border.Left.Color.SetColor(Color.Black);
            _ws.Cells[_header].Style.Font.Bold = true;

            _rows = 2;

            foreach (var i in _items)
            {
                for (int c = 0; c < _cols - 1; c++)
                {
                    var prop = _props[c];
                    var cell = ws.Cells[_rows, c + 1];
                    var value = prop.GetValue(i);
                    cell.Value = value;
                }

                if (_rows % 2 != 0)
                {
                    _ws.Cells[_rows, 1, _rows, _cols - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _ws.Cells[_rows, 1, _rows, _cols - 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e0e8ee"));
                }

                _ws.Cells[_rows, 1, _rows, _cols - 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, ColorTranslator.FromHtml("#808080"));
                _ws.Cells[_rows, 1, _rows, _cols - 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                _ws.Cells[_rows, 1, _rows, _cols - 1].Style.Border.Left.Color.SetColor(ColorTranslator.FromHtml("#808080"));

                _rows++;
            }

            AutoFit();
        }

        public void Sum<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = (MemberExpression)expression.Body;
            string name = body.Member.Name;
            var p = _props.Select((x, i) => new { prop = x, index = i }).FirstOrDefault(x => x.prop.Name == name);
            var c = p.index + 1;
            _ws.Cells[_rows, c].Formula = $"=SUM({ExcelCellBase.GetAddress(2, c, _rows - 1, c)})";

            var footer = ExcelCellBase.GetAddress(_rows, 1, _rows, _cols - 1);
            _ws.Cells[footer].Style.Fill.PatternType = ExcelFillStyle.Solid;
            _ws.Cells[footer].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ccff99"));
            _ws.Cells[footer].Style.Border.BorderAround(ExcelBorderStyle.Thin, ColorTranslator.FromHtml("#808080"));
            _ws.Cells[footer].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            _ws.Cells[footer].Style.Border.Left.Color.SetColor(ColorTranslator.FromHtml("#808080"));
            _ws.Cells[footer].Style.Font.Bold = true;

            AutoFit();
        }

        public void Format<TProperty>(Expression<Func<T, TProperty>> expression, string format)
        {
            var body = (MemberExpression)expression.Body;
            string name = body.Member.Name;
            var p = _props.Select((x, i) => new { prop = x, index = i }).FirstOrDefault(x => x.prop.Name == name);
            var c = p.index + 1;
            var column = ExcelCellBase.GetAddress(2, c, _rows, c);
            _ws.Cells[column].Style.Numberformat.Format = format;
        }

        public void FormatCurrency<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Format(expression, "\"$\"#,##0.00");
        }

        private void AutoFit()
        {
            if (!string.IsNullOrEmpty(_header))
                _ws.Cells[_header].AutoFitColumns();
        }
    }
}
